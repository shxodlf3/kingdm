using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Languages;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;
using WPFLocalizeExtension.Engine;
using Timer=System.Timers.Timer;

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

        public ObservableCollection<Player> Players { get; private set; }

        public ObservableCollection<CultureInfo> AvailableLanguages { get; private set; }
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

        private ObjectRelayCommand resetCountsCommand;
        public ObjectRelayCommand ResetCountsCommand
        {
            get
            {
                if (resetCountsCommand == null)
                {
                    resetCountsCommand = new ObjectRelayCommand(o => ResetDamage());
                }
                return resetCountsCommand;
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

        private RelayCommand<CultureInfo> selectLanguageCommand;
        public RelayCommand<CultureInfo> SelectLanguageCommand
        {
            get
            {
                if (selectLanguageCommand == null)
                {
                    selectLanguageCommand = new RelayCommand<CultureInfo>(SelectLanguage);
                }
                return selectLanguageCommand;
            }
        }

        #endregion

        public WindowMainData()
        {
            Players = new ObservableCollection<Player>();
            AvailableLanguages = new ObservableCollection<CultureInfo>();
            Settings.Default.PropertyChanged += OnSettingsChanged;

            InitializeLogParser();
            InitializeTimers();
            InitializeCommands();
            DetectAvailableLanguages();
        }

        private void InitializeCommands()
        {
            Commands.ClearAllCommand = new ObjectRelayCommand(o=>ClearAll());
            Commands.ResetCountsCommand = new ObjectRelayCommand(o => ResetDamage());
            Commands.RemovePlayerCommand = new RelayCommand<Player>(RemovePlayer, player => player != null);
            Commands.IgnorePlayerCommand = new RelayCommand<Player>(IgnorePlayer, player => player != null);
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
                    SetFilter();
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
            MessageBox.Show(string.Format(WindowMainRes.OpenLogError, WindowMainRes.OptionsBtnToolTip, WindowMainRes.LocateLogMenuHeader), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                UpdatePlayerDamage(e.Name, e.Damage, Resources.WhiteDamageSkillName);
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
                UpdatePlayerExp(Settings.Default.YouAlias, e.Exp);
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
                UpdatePlayerKinah(Settings.Default.YouAlias, e.Kinah);
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
            if (Settings.Default.IgnoreList.Contains(name) || Players.Any(o=>o.PlayerName == name))
            {
                return;
            }
            if(Settings.Default.IsHideOthers && name != Settings.Default.YouAlias)
            {
                return;
            }
            if (Settings.Default.IsGroupOnly && !Settings.Default.GroupList.Contains(name))
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
                //p.RemoveMe += RemovePlayer;

                Players.Add(p);

                //SetFilter();
                if (name == Settings.Default.YouAlias || Settings.Default.GroupList.Contains(name))
                {
                    p.IsGroupMember = true;
                    return;
                }
                UpdateSort();
            }
        }

        private void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }

        private void IgnorePlayer(Player player)
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

        private void SetFilter()
        {
            /*ICollectionView view = CollectionViewSource.GetDefaultView(Players);
            if (view != null)
            {
                if (Settings.Default.IsHideOthers)
                {
                    view.Filter = o => ((Player)o).PlayerName == Settings.Default.YouAlias;
                }
                else if (Settings.Default.IsGroupOnly)
                {
                    view.Filter = o => ((Player)o).IsGroupMember;
                }
                else
                {
                    view.Filter = null;
                }
            }*/
            if (Settings.Default.IsHideOthers)
            {
                //ToList() here is copy data to temp list for allow to us delete permission for Players collection
                foreach (var player in Players.ToList())
                {
                    if(player.PlayerName != Settings.Default.YouAlias)
                    {
                        Players.Remove(player);
                    }
                }
            }
            else if(Settings.Default.IsGroupOnly)
            {
                foreach (var player in Players.ToList())
                {
                    if (!player.IsGroupMember)
                    {
                        Players.Remove(player);
                    }
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
            // Think timer is elapsed while exit. This should be fix the problem.
            if (Application.Current == null)
            {
                return;
            }
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

        private void RemoveGroupMember(string name)
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

        private void DetectAvailableLanguages()
        {
            var dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            AvailableLanguages.Add(allCultures.First(o=>o.Name == "en")); //Default language
            foreach (var dir in dirs)
            {
                string dirName = Path.GetFileName(dir);
                var availableCulture = allCultures.FirstOrDefault(o => o.Name == dirName);
                if(availableCulture != null)
                {
                    AvailableLanguages.Add(availableCulture);
                }
            }

            if (Settings.Default.SelectedLanguage == CultureInfo.InvariantCulture)
            {
                SelectLanguage(Thread.CurrentThread.CurrentUICulture);
            }
            else
            {
                SelectLanguage(Settings.Default.SelectedLanguage);
                
            }
        }

        private void SelectLanguage(CultureInfo language)
        {
            var findedLanguage = AvailableLanguages.FirstOrDefault(o => o.Equals(language) || (language.Parent != null && o.Equals(language.Parent)));
            Settings.Default.SelectedLanguage = findedLanguage ?? AvailableLanguages[0];
        }

        #endregion

    }
}