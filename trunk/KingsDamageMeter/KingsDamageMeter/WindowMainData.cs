using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Languages;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter
{
    public class WindowMainData : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private readonly AionLogParser _LogParser = new AionLogParser();
        private readonly Timer sortTimer = new Timer();
        //private readonly Timer dpsTimeoutTimer = new Timer();


        #region Properties

        public bool IsEnabled
        {
            get { return _LogParser.Running; }
        }

        public SafeNotifiedCollection<Player> Players { get; private set; }

        //private DateTime? TimeWhenFightIsStarted { get; set; }
        #endregion

        #region Commands

        private ObjectRelayCommand changePowerCommand;
        public ObjectRelayCommand ChangePowerCommand
        {
            get
            {
                if (changePowerCommand == null)
                {
                    changePowerCommand = new ObjectRelayCommand(o => ChangePower());
                }
                return changePowerCommand;
            }
        }

        private ObjectRelayCommand resetDamageCommand;
        public ObjectRelayCommand ResetDamageCommand
        {
            get
            {
                if (resetDamageCommand == null)
                {
                    resetDamageCommand = new ObjectRelayCommand(o => ResetDamage());
                }
                return resetDamageCommand;
            }
        }

        private ObjectRelayCommand clearAllCommand;
        public ObjectRelayCommand ClearAllCommand
        {
            get
            {
                if (clearAllCommand == null)
                {
                    clearAllCommand = new ObjectRelayCommand(o => ClearAll());
                }
                return clearAllCommand;
            }
        }

        #endregion

        public WindowMainData()
        {
            InitializeLogParser();
            InitializeTimers();

            Players = new SafeNotifiedCollection<Player>();

            Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private void InitializeTimers()
        {
            sortTimer.Interval = 5000; //Update sort every 5 seconds
            sortTimer.Elapsed += NeedToSort;
            //dpsTimeoutTimer.Interval = 30000;
            //dpsTimeoutTimer.Elapsed += NeedToUpdateDps;
        }

        private void NeedToSort(object sender, ElapsedEventArgs e)
        {
            UpdateSort();
            sortTimer.Enabled = false;
        }

        //private void NeedToUpdateDps(object sender, ElapsedEventArgs e)
        //{
        //    UpdateDps(0);
        //    dpsTimeoutTimer.Enabled = false;
        //}

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsGroupOnly":
                case "IsHideOthers":
                    UpdateFilter();
                    UpdatePercents();
                    break;
                case "SortType":
                    UpdateSort();
                    break;
            }
        }

        #region Log parser

        private void InitializeLogParser()
        {
            _LogParser.DamageInflicted += OnDamageInflicted;
            _LogParser.CriticalInflicted += OnDamageInflicted;
            _LogParser.SkillDamageInflicted += OnSkillDamageInflicted;
            _LogParser.FileNotFound += OnFileNotFound;
            _LogParser.PlayerJoinedGroup += OnPlayerJoinedGroup;
            _LogParser.PlayerLeftGroup += OnPlayerLeftGroup;
            _LogParser.Started += delegate { NotifyPropertyChanged("IsEnabled"); };
            _LogParser.Stopped += delegate { NotifyPropertyChanged("IsEnabled"); };
            _LogParser.ExpGained += OnExpGained;
            _LogParser.KinahEarned += OnKinahEarned;
            _LogParser.Start(Settings.Default.AionLogPath);
        }

        private static void OnFileNotFound(object sender, EventArgs e)
        {
            MessageBox.Show(Gui.Default.OpenLogError, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        private void OnDamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, PlayerDamageEventArgs>(OnDamageInflicted),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerDamage(e.Name, e.Damage, Gui.Default.WhiteDamage);
            }
        }

        private void OnSkillDamageInflicted(object sender, PlayerSkillDamageEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, PlayerSkillDamageEventArgs>(OnSkillDamageInflicted),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerDamage(e.Name, e.Damage, e.Skill);
            }
        }

        private void OnPlayerJoinedGroup(object sender, PlayerEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, PlayerEventArgs>(OnPlayerJoinedGroup),
                                                      sender, e);
            }
            else
            {
                AddPlayer(e.Name, /* isGroupMember = */true);
            }
        }

        private void OnPlayerLeftGroup(object sender, PlayerEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, PlayerEventArgs>(OnPlayerLeftGroup),
                                                      sender, e);
            }
            else
            {
                RemoveGroupMember(e.Name);
            }
        }

        private void OnExpGained(object sender, ExpEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, ExpEventArgs>(OnExpGained),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerExp(Regex.Default.YouAlias, e.Exp);
            }
        }

        private void OnKinahEarned(object sender, KinahEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, KinahEventArgs>(OnKinahEarned),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerKinah(Regex.Default.YouAlias, e.Kinah);
            }
        }

        public void ChangeLogFile(string fileName)
        {
            _LogParser.Stop();
            Settings.Default.AionLogPath = fileName;
            _LogParser.Start(Settings.Default.AionLogPath);
        }

        #endregion

        #region General Methods

        public void OnClose()
        {
            _LogParser.Stop();
        }

        /// <summary>
        /// Update a player's damage.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="damage">The total damage the player has dealt</param>
        public void UpdatePlayerDamage(string name, int damage, string skill)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }

            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            //if(TimeWhenFightIsStarted == null)
            //{
            //    TimeWhenFightIsStarted = DateTime.Now;
            //}

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.Damage += damage;
                player.Skills.Incriment(skill, damage);
                if (!Settings.Default.IgnoreList.Contains(name))
                {
                    UpdatePercents();
                    if (!sortTimer.Enabled)
                    {
                        sortTimer.Enabled = true;
                    }
                }
            }
        }

        public void UpdatePlayerExp(string name, int exp)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }

            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.Exp += exp;
                //if(TimeWhenFightIsStarted != null)
                //{
                //    UpdateDps((int)(DateTime.Now - TimeWhenFightIsStarted.Value).TotalSeconds);
                //    TimeWhenFightIsStarted = null;
                //}
            }
        }

        public void UpdatePlayerKinah(string name, int kinah)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }

            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.Kinah += kinah;
            }
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

            if (!String.IsNullOrEmpty(name))
            {
                var p = new Player
                {
                    PlayerName = name,
                    IsGroupMember = isGroupMember
                };
                p.RemoveMe += RemovePlayer;

                Players.Add(p);

                UpdateFilter();
                if (name == Regex.Default.YouAlias || Settings.Default.GroupList.Contains(name))
                {
                    p.IsGroupMember = true;
                    return;
                }
                UpdateSort();
            }
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }

        public void IgnorePlayer(Player player)
        {
            if (!Settings.Default.IgnoreList.Contains(player.PlayerName))
            {
                Settings.Default.IgnoreList.Add(player.PlayerName);
            }

            RemovePlayer(player);
        }

        /// <summary>
        /// Indicates whether the player already exists in the scroll viewer.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <returns>bool</returns>
        public bool PlayerExists(string name)
        {
            return Players.Any(o => o.PlayerName == name);
        }

        private void UpdateFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Players);
            if (view != null)
            {
                if (Settings.Default.IsHideOthers)
                {
                    view.Filter = o => ((Player)o).PlayerName == Regex.Default.YouAlias;
                }
                else if (Settings.Default.IsGroupOnly)
                {
                    view.Filter = o => ((Player)o).IsGroupMember;
                }
                else
                {
                    view.Filter = null;
                }
            }
        }

        private void UpdatePercents()
        {
            long total;
            if (Settings.Default.IsGroupOnly)
            {
                total = Players.Where(o => o.IsGroupMember).Sum(o => o.Damage);
            }
            else
            {
                total = Players.Sum(o => o.Damage);
            }

            foreach (Player p in Players)
            {
                double percent = (((double)(p.Damage - total) / total) + 1);

                if (Settings.Default.IsGroupOnly)
                {
                    if (p.IsGroupMember)
                    {
                        p.DamagePercent = percent;
                    }

                    else
                    {
                        p.DamagePercent = 0;
                    }
                }

                else
                {
                    p.DamagePercent = percent;
                }
            }
        }

        private void UpdateSort()
        {
            // Null reference exception here when I closed the app. Difficult to reproduce.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(UpdateSort));
            }
            else
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
                            view.SortDescriptions.Add(new SortDescription("PlayerName", ListSortDirection.Descending));
                            break;
                        case PlayerSortType.DamagePerSecond:
                            view.SortDescriptions.Add(new SortDescription("DamagePerSecond",
                                                                          ListSortDirection.Descending));
                            break;
                    }
                    view.Refresh();
                }
            }
        }

        private void UpdateDps(int fightTime)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<int>(UpdateDps), fightTime);
            }
            else
            {
                foreach (var player in Players)
                {
                    player.FightTime += fightTime;
                }
            }
        }

        public void RemoveGroupMember(string name)
        {
            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.IsGroupMember = false;
                UpdatePercents();
                UpdateSort();
            }
        }

        private void ResetDamage()
        {
            foreach (var player in Players)
            {
                player.Reset();
            }
        }

        private void ChangePower()
        {
            if (_LogParser.Running)
            {
                _LogParser.Stop();
            }
            else
            {
                _LogParser.Start(Settings.Default.AionLogPath);
            }
            NotifyPropertyChanged("IsEnabled");
        }

        private void ClearAll()
        {
            Players.Clear();
        }

        public void Rename(string newName, string oldName)
        {
            var player = Players.FirstOrDefault(o => o.PlayerName == oldName);
            if (player != null)
            {
                player.PlayerName = newName;
            }
        }

        public void CopyPlayerToClipboard(string name)
        {
            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                
            }
        }

        #endregion

    }
}