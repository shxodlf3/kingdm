/**************************************************************************\
 * 
    This file is part of KingsDamageMeter.

    KingsDamageMeter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KingsDamageMeter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KingsDamageMeter. If not, see <http://www.gnu.org/licenses/>.
 * 
\**************************************************************************/

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Converters;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;
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
        public ObservableCollection<CultureInfo> AvailableLogLanguages { get; private set; }

        private YouPlayer you;
        public YouPlayer You
        {
            get { return you; }
            private set
            {
                if(you != value)
                {
                    you = value;
                    NotifyPropertyChanged("You");
                }
            }
        }

        public string PowerButtonToolTip
        {
            get
            {
                return IsEnabled ? WindowMainRes.PowerOnBtnTooltip : WindowMainRes.PowerOffBtnTooltip;
            }
        }

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

        #endregion

        public WindowMainData()
        {
            Players = new ObservableCollection<Player>();
            Players.CollectionChanged += PlayersCollectionChanged;
            AvailableLanguages = new ObservableCollection<CultureInfo>();
            AvailableLogLanguages = new ObservableCollection<CultureInfo>();
            Settings.Default.PropertyChanged += OnSettingsChanged;

            InitializeLogParser();
            InitializeTimers();
            InitializeCommands();

            //Note: This is a test
            //Players.Add(new Player
            //                {
            //                    PlayerName = "Memphistopheles",
            //                    Damage = 1000000,
            //                    FightTime = 1800,
            //                    PercentFromGroupDamages = 1,
            //                });
            //You = new YouPlayer
            //          {
            //              PlayerName = Settings.Default.YouAlias,
            //              Damage = 10000001,
            //              FightTime = 1800,
            //              PercentFromGroupDamages = 1,
            //              Exp = 10000000,
            //              Ap = 10000,
            //              KinahEarned = 200000,
            //              KinahSpent = -100000,
            //              IsGroupMember = true
            //          };
            //Players.Add(You);
            //UpdatePercents();
            //////////////////////////////////////
        }

        private void InitializeCommands()
        {
            Commands.ClearAllCommand = new ObjectRelayCommand(o=>ClearAll());
            Commands.ResetCountsCommand = new ObjectRelayCommand(o => ResetDamage());
            Commands.RemovePlayerCommand = new RelayCommand<Player>(RemovePlayer, player => player != null);
            Commands.IgnorePlayerCommand = new RelayCommand<Player>(IgnorePlayer, player => player != null);
            Commands.CopyToClipboardCommand = new RelayCommand<ClipboardCopyType>(o=>CopyToClipboard(o, null));
            Commands.CopySelectedToClipboardCommand = new RelayCommand<Player>(o=>CopyToClipboard(ClipboardCopyType.OnlySelected, o));
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
                case "SelectedLogLanguage":
                    _LogParser.Initialize();
                    break;
                case "SelectedLanguage":
                    NotifyPropertyChanged("PowerButtonToolTip");
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
            _LogParser.KinahSpent += OnKinahSpent;
            _LogParser.AbyssPointsGained += OnAbyssPointsGained;
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
                UpdatePlayerKinah(Settings.Default.YouAlias, e.Kinah, /* isEarned = */ true);
            }
        }

        private void OnKinahSpent(object sender, KinahEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, KinahEventArgs>(OnKinahSpent),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerKinah(Settings.Default.YouAlias, e.Kinah, false);
            }
        }

        private void OnAbyssPointsGained(object sender, AbyssPointsEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, AbyssPointsEventArgs>(OnAbyssPointsGained),
                                                      sender, e);
            }
            else
            {
                UpdatePlayerAp(Settings.Default.YouAlias, e.Points);
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
                UpdatePercents();

                if (!sortTimer.Enabled)
                {
                    sortTimer.Enabled = true;
                }
                UpdatePlayerClass(player.PlayerName, skill);
            }
        }

        public void UpdatePlayerClass(string name, string skill)
        {
            var player = Players.FirstOrDefault(o => o.PlayerName == name);

            if (player != null)
            {
                if (player.PlayerClass == ClassType.None)
                {
                    player.PlayerClass = SkillDictionary.GetClass(skill);
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

            var player = Players.FirstOrDefault(o => o.PlayerName == name) as YouPlayer;
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

        public void UpdatePlayerKinah(string name, int kinah, bool isEarned)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }

            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name) as YouPlayer;
            if (player != null)
            {
                if(isEarned)
                {
                    player.KinahEarned += kinah;
                }
                else
                {
                    player.KinahSpent -= kinah;
                }
            }
        }

        private void UpdatePlayerAp(string name, int points)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return;
            }

            if (!PlayerExists(name))
            {
                AddPlayer(name, false);
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name) as YouPlayer;
            if (player != null)
            {
                player.Ap += points;
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
                Player p;
                if (name == Settings.Default.YouAlias)
                {
                    p = new YouPlayer
                            {
                                PlayerName = name,
                                IsGroupMember = true
                            };
                }
                else
                {
                    p = new Player
                            {
                                PlayerName = name,
                                IsGroupMember = isGroupMember || Settings.Default.FriendList.Contains(name)
                            };
                }

                Players.Add(p);
                UpdateSort();
            }
        }

        private void RemovePlayer(Player player)
        {
            Players.Remove(player);
            UpdatePercents();
        }

        private void IgnorePlayer(Player player)
        {
            if(player.PlayerName == Settings.Default.YouAlias)
            {
                return;
            }

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
            CalculateGroupDamagePercents();
            CalculateTopDamagePercents();
        }

        private void CalculateTopDamagePercents()
        {
            if(Players.Count == 0)
            {
                return;
            }

            var topDamagePlayer = Players.Where(o => o.Damage == Players.Max(x => x.Damage)).First();
            topDamagePlayer.PercentFromTopDamage = 1;
            foreach (var player in Players)
            {
                if(topDamagePlayer != player)
                {
                    player.PercentFromTopDamage = (double)player.Damage / topDamagePlayer.Damage;
                }
            }
        }

        private void CalculateGroupDamagePercents()
        {
            long total = Players.Sum(o => o.Damage);

            foreach (Player p in Players)
            {
                //What for need that formula?
                //double percent = (((double) (p.Damage - total)/total) + 1);

                //Isn't player damage percent from total is:
                p.PercentFromGroupDamages = (double)p.Damage / total;
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
            if(Settings.Default.FriendList.Contains(name))
            {
                return;
            }

            var player = Players.FirstOrDefault(o => o.PlayerName == name);
            if (player != null)
            {
                player.IsGroupMember = false;
                SetFilter();
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
            NotifyPropertyChanged("PowerButtonToolTip");
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

        private void CopyToClipboard(ClipboardCopyType copyType, Player selectedPlayer)
        {
            if(copyType == ClipboardCopyType.OnlySelected)
            {
                if (selectedPlayer != null)
                {
                    //Clipboard.SetText(selectedPlayer.PlayerName + " " + selectedPlayer.Damage);
                    Clipboard.SetText(String.Format("{0} {1}, {2} ({3})", selectedPlayer.PlayerName, selectedPlayer.Damage, 
                        selectedPlayer.DamagePerSecond, selectedPlayer.PercentFromGroupDamages.ToString("0%")));
                }
            }
            else
            {
                string chatPrefix = string.Empty;
                switch (copyType)
                {
                    case ClipboardCopyType.ToPartyChat:
                        chatPrefix = "/p";
                        break;
                    case ClipboardCopyType.ToAllianceChat:
                        chatPrefix = "/a";
                        break;
                    case ClipboardCopyType.ToLegionChat:
                        chatPrefix = "/l";
                        break;
                }
                ICollectionView view = CollectionViewSource.GetDefaultView(Players);
                if (view != null)
                {
                    var sb = new StringBuilder();
                    foreach (Player player in view)
                    {
                        sb.AppendFormat("{0} {1} {2}{3}", chatPrefix, player.PlayerName, player.Damage, Environment.NewLine);
                    }
                    Clipboard.SetText(sb.ToString());
                }
            }
        }

        private void PlayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if(e.NewItems != null)
                    {
                        var youAre =
                            e.NewItems.Cast<Player>().FirstOrDefault(o => o.PlayerName == Settings.Default.YouAlias) as YouPlayer;
                        if(youAre != null)
                        {
                            You = youAre;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        var youAre =
                            e.OldItems.Cast<Player>().FirstOrDefault(o => o.PlayerName == Settings.Default.YouAlias);
                        if (youAre != null)
                        {
                            You = null;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    You = null;
                    break;
            }
        }

        #endregion

    }
}