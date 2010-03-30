using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Enums;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public class Encounter : NotifyPropertyChangedBase, IEncounter
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ICollection<Player> Players { get; private set; }

        public Region Parent { get; private set; }

        private double time;
        public double Time
        {
            get { return time; }
            private set
            {
                if(time != value)
                {
                    time = value;
                    NotifyPropertyChanged("Time");

                    PlayerCalculationHelper.CalculateDps(Players, Time);
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsEnded { get; private set; }

        private readonly DateTime startEncounterTime;
        private DateTime lastDamageInflictedTime;
        private readonly Timer updateDataTimer = new Timer(5000);
        public Encounter(Region parent)
        {
            Parent = parent;
            Players = new ObservableCollection<Player>();

            startEncounterTime = DateTime.Now;
            updateDataTimer.Elapsed += delegate
                                           {
                                               updateDataTimer.Stop();
                                               UpdateData();
                                           };
        }

        /// <summary>
        /// Indicates whether the player already exists.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <returns>bool</returns>
        public bool PlayerExists(string name)
        {
            return Players.Any(o => o.PlayerName == name);
        }

        /// <summary>
        /// Add a player to the scroll viewer.
        /// </summary>
        public void AddPlayer(string name, bool isGroupMember)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }
            if (isGroupMember && Players.Any(o => o.PlayerName == name))
            {
                Players.First(o => o.PlayerName == name).IsGroupMember = true;
                return;
            }
            if (Settings.Default.IsHideOthers && name != Settings.Default.YouAlias)
            {
                return;
            }
            if (Settings.Default.IsGroupOnly && !isGroupMember && !Settings.Default.FriendList.Contains(name) &&
                name != Settings.Default.YouAlias)
            {
                return;
            }

            if (!String.IsNullOrEmpty(name))
            {
                var p = new Player
                            {
                                PlayerName = name,
                                IsGroupMember = isGroupMember
                            };

                Players.Add(p);
                UpdateSort();
            }
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            UpdatePercents();
        }

        private void UpdateData()
        {
            // Think timer is elapsed while exit. This should be fix the problem.
            if (Application.Current == null)
            {
                return;
            }
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(UpdateData));
            }
            else
            {
                Time = (lastDamageInflictedTime - startEncounterTime).TotalSeconds;
                UpdateSort();
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
        /// <param name="name">The name of the player</param>
        /// <param name="damage">The total damage the player has dealt</param>
        /// <param name="skill">The skill</param>
        /// <param name="isGroupMember">Flag for indicate a group member</param>
        public void UpdatePlayerDamage(string name, int damage, string skill, bool isGroupMember)
        {
            if (!PlayerExists(name))
            {
                AddPlayer(name, isGroupMember);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.IsGroupMember = isGroupMember;

                lastDamageInflictedTime = DateTime.Now;
                player.Damage += damage;
                player.Skills.Incriment(skill, damage);
                UpdatePercents();

                if (player.PlayerClass == ClassType.None)
                {
                    player.PlayerClass = SkillDictionary.GetClass(skill);
                }

                if (!updateDataTimer.Enabled)
                {
                    updateDataTimer.Start();
                }
            }
        }

        public void EndEncounter(DateTime encounterEndTime)
        {
            IsEnded = true;
            Time = (encounterEndTime - startEncounterTime).TotalSeconds;
        }

        public void UpdatePlayerReceivedDamage(string name, int damage)
        {
            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.DamageTaken += damage;
            }
        }
    }
}
