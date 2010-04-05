using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KingsDamageMeter.Enums;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public abstract class EncounterBase : NotifyPropertyChangedBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<Player> Players { get; private set; }

        private bool isSelected;
        /// <summary>
        /// This property is used for synchorize visual selection in TreeView
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;
        /// <summary>
        /// This property is used for synchorize visual expanding in TreeView
        /// </summary>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if(IsExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        protected DateTime LastDamageInflictedTime { get; set; }

        protected EncounterBase()
        {
            Players = new ObservableCollection<Player>();

            Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SortType")
            {
                UpdateSort();
            }
        }


        /// <summary>
        /// Should be implemented in inheritors. Here must be updated Time based parameters, like DPS or HPS.
        /// </summary>
        public abstract void UpdateData();

        /// <summary>
        /// Indicates whether the player already exists.
        /// </summary>
        /// <param name="playerName">The name of the player</param>
        /// <returns>bool</returns>
        public bool PlayerExists(string playerName)
        {
            return Players.Any(o => o.PlayerName == playerName);
        }

        /// <summary>
        /// Add a player to the scroll viewer.
        /// </summary>
        public void AddPlayer(string playerName, bool isGroupMember)
        {
            if (Settings.Default.IgnoreList.Contains(playerName))
            {
                return;
            }
            if (isGroupMember && Players.Any(o => o.PlayerName == playerName))
            {
                Players.First(o => o.PlayerName == playerName).IsGroupMember = true;
                return;
            }
            if (Settings.Default.IsHideOthers && playerName != Settings.Default.YouAlias)
            {
                return;
            }
            if (Settings.Default.IsGroupOnly && !isGroupMember && !Settings.Default.FriendList.Contains(playerName) &&
                playerName != Settings.Default.YouAlias)
            {
                return;
            }

            if (!String.IsNullOrEmpty(playerName))
            {
                var p = new Player
                {
                    PlayerName = playerName,
                    IsGroupMember = isGroupMember
                };

                Players.Add(p);
                UpdateSort();
            }
        }

        public virtual void RemovePlayer(string playerName)
        {
            var findedPlayer = Players.FirstOrDefault(o => o.PlayerName == playerName);
            if (findedPlayer != null)
            {
                Players.Remove(findedPlayer);
                UpdatePercents();
            }
        }

        public void UpdateSort()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Players);
            if (view != null)
            {
                view.SortDescriptions.Clear();
                switch (Settings.Default.SortType)
                {
                    case PlayerSortType.Damage:
                        view.SortDescriptions.Add(new SortDescription("Damage", ListSortDirection.Descending));
                        break;
                    case PlayerSortType.Name:
                        view.SortDescriptions.Add(new SortDescription("PlayerName", ListSortDirection.Ascending));
                        break;
                    case PlayerSortType.DamagePerSecond:
                        view.SortDescriptions.Add(new SortDescription("DamagePerSecond",
                                                                      ListSortDirection.Descending));
                        break;
                }
                view.Refresh();
            }
        }

        public void UpdatePercents()
        {
            PlayerCalculationHelper.CalculateTopDamagePercents(Players);
            PlayerCalculationHelper.CalculateGroupDamagePercents(Players);
        }

        /// <summary>
        /// Update a player's damage.
        /// </summary>
        /// <param name="playerName">The name of the player</param>
        /// <param name="damage">The total damage the player has dealt</param>
        /// <param name="skill">The skill</param>
        /// <param name="isGroupMember">Flag for indicate a group member</param>
        public virtual void UpdatePlayerDamage(string playerName, int damage, string skill, bool isGroupMember)
        {
            if (!PlayerExists(playerName))
            {
                AddPlayer(playerName, isGroupMember);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == playerName);
            if (player != null)
            {
                player.IsGroupMember = isGroupMember;

                LastDamageInflictedTime = DateTime.Now;
                player.Damage += damage;
                player.Skills.Incriment(skill, damage);
                UpdatePercents();

                if (player.PlayerClass == ClassType.None)
                {
                    player.PlayerClass = SkillDictionary.GetClass(skill);
                }
            }
        }

        public virtual void UpdatePlayerReceivedDamage(string playerName, int damage)
        {
            if (!PlayerExists(playerName))
            {
                AddPlayer(playerName, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == playerName);
            if (player != null)
            {
                player.DamageTaken += damage;
            }
        }
    }
}
