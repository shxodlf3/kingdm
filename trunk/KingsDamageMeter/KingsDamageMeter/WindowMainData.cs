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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Enums;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;
using Timer=System.Timers.Timer;

namespace KingsDamageMeter
{
    public class WindowMainData : NotifyPropertyChangedBase
    {
        private readonly AionLogParser _LogParser = new AionLogParser();
        private readonly Timer expGainedTimer = new Timer(5000);
        private readonly Timer timeoutTimer = new Timer(20000);


        #region Properties

        public bool IsEnabled
        {
            get { return _LogParser.Running; }
        }

        public List<string> GroupMembers { get; private set; }
        public ObservableCollection<EncounterBase> Regions { get; private set; }

        public ObservableCollection<CultureInfo> AvailableLanguages { get; private set; }
        public ObservableCollection<CultureInfo> AvailableLogLanguages { get; private set; }

        private EncounterBase selectedEncounter;
        public EncounterBase SelectedEncounter
        {
            get { return selectedEncounter; }
            set
            {
                if(selectedEncounter != value)
                {
                    if(selectedEncounter != null)
                    {
                        selectedEncounter.IsSelected = false;
                    }
                    selectedEncounter = value;
                    if (selectedEncounter != null)
                    {
                        selectedEncounter.IsSelected = true;
                    }

                    NotifyPropertyChanged("SelectedEncounter");
                }
            }
        }

        private string currentRegionName;
        public string CurrentRegionName
        {
            get { return currentRegionName; }
            set
            {
                if(currentRegionName != value)
                {
                    currentRegionName = value;
                    NotifyPropertyChanged("CurrentRegionName");
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

        private DateTime lastExpGainedTime;
        private DateTime lastDamageInflictedTime;
        private DateTime startTime = DateTime.Now;
        private AllEncounters AllEncounters { get; set; }

        private int exp;
        public int Exp
        {
            get { return exp; }
            set
            {
                if (exp != value)
                {
                    exp = value;
                    NotifyPropertyChanged("Exp");
                    NotifyPropertyChanged("ExpPerHour");
                }
            }
        }

        public int ExpPerHour
        {
            get
            {
                if (Exp == 0)
                {
                    return 0;
                }
                TimeSpan span = DateTime.Now - startTime;
                return (int)((Exp / span.TotalSeconds) * 3600);
            }
        }

        private int kinahEarned;
        public int KinahEarned
        {
            get { return kinahEarned; }
            set
            {
                if (kinahEarned != value)
                {
                    kinahEarned = value;
                    NotifyPropertyChanged("KinahEarned");
                    NotifyPropertyChanged("TotalKinah");
                }
            }
        }

        private int kinahSpent;
        public int KinahSpent
        {
            get { return kinahSpent; }
            set
            {
                if (kinahSpent != value)
                {
                    kinahSpent = value;
                    NotifyPropertyChanged("KinahSpent");
                    NotifyPropertyChanged("TotalKinah");
                }
            }
        }

        public int TotalKinah
        {
            get
            {
                return KinahEarned + KinahSpent;
            }
        }

        private int ap;
        public int Ap
        {
            get { return ap; }
            set
            {
                if (ap != value)
                {
                    ap = value;
                    NotifyPropertyChanged("Ap");
                }
            }
        }
        
        private Region LastRegion { get; set; }
        private Encounter LastEncounter { get; set; }

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

        private RelayCommand<DisplayType> resetCommand;
        public RelayCommand<DisplayType> ResetCommand
        {
            get
            {
                if (resetCommand == null)
                {
                    resetCommand = new RelayCommand<DisplayType>(Reset);
                }
                return resetCommand;
            }
        }

        private ObjectRelayCommand resetAllCommand;
        public ObjectRelayCommand ResetAllCommand
        {
            get
            {
                if (resetAllCommand == null)
                {
                    resetAllCommand = new ObjectRelayCommand(o => ResetAll());
                }
                return resetAllCommand;
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
            Regions = new ObservableCollection<EncounterBase>();
            Regions.CollectionChanged += OnRegionsCollectionChanged;
            //Note: This is a test
            //Regions.Add(new Region { Name = "Some region" });
            //Regions[0].Encounters.Add(new Encounter { Name = "Encounter 1" });
            //Regions[0].Encounters[0].Players.Add(new Player
            //                {
            //                    PlayerName = "Memphistopheles",
            //                    Damage = 1000000,
            //                    PercentFromGroupDamages = 1,
            //                    BiggestHit = 3000
            //                });
            //Regions[0].Encounters[0].Players.Add(new YouPlayer
            //          {
            //              PlayerName = Settings.Default.YouAlias,
            //              Damage = 10000001,
            //              PercentFromGroupDamages = 1,
            //              Exp = 10000000,
            //              Ap = 10000,
            //              KinahEarned = 200000,
            //              KinahSpent = -100000,
            //              IsGroupMember = true,
            //              BiggestHit = 5000
            //          });
            //Regions[0].Encounters[0].You = (YouPlayer) Regions[0].Encounters[0].Players.Last();
            //Regions[0].Encounters.Add(new Encounter { Name = "Encounter 2" });
            //Regions[0].Encounters[1].Players.Add(new Player
            //{
            //    PlayerName = "Memphistopheles",
            //    Damage = 2000000,
            //    PercentFromGroupDamages = 1,
            //    BiggestHit = 4500
            //});
            //Regions[0].Encounters[1].Players.Add(new YouPlayer
            //{
            //    PlayerName = Settings.Default.YouAlias,
            //    Damage = 20000001,
            //    PercentFromGroupDamages = 1,
            //    Exp = 10000000,
            //    Ap = 20000,
            //    KinahEarned = 300000,
            //    KinahSpent = -100000,
            //    IsGroupMember = true,
            //    BiggestHit = 4500
            //});
            //Regions[0].Encounters[1].You = (YouPlayer)Regions[0].Encounters[1].Players.Last();

            GroupMembers = new List<string>();
            AvailableLanguages = new ObservableCollection<CultureInfo>();
            AvailableLogLanguages = new ObservableCollection<CultureInfo>();
            Settings.Default.PropertyChanged += OnSettingsChanged;

            CurrentRegionName = "Unknown";

            InitializeLogParser();
            InitializeTimers();
            InitializeCommands();
            DetectAvailableLanguages();

            AllEncounters = new AllEncounters();
            Regions.Add(AllEncounters);
        }

        private void OnRegionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (Region region in e.OldItems)
                        {
                            AllEncounters.Regions.Remove(region);
                        }
                    }
                    break;
            }
        }

        private void InitializeCommands()
        {
            Commands.RemoveEncounterCommand = new RelayCommand<EncounterBase>(RemoveEncounter,
                                                                           encounter => encounter != null && !(encounter is AllEncounters));
            Commands.RemoveAllEncountersCommand = new ObjectRelayCommand(o => RemoveAllEncounters(),
                                                                         o => Regions.Where(x => !(x is AllEncounters)).Count() > 0);
            Commands.RemovePlayerCommand = new RelayCommand<Player>(RemovePlayer, player => player != null);
            Commands.IgnorePlayerCommand = new RelayCommand<Player>(IgnorePlayer, player => player != null);
            Commands.CopyToClipboardCommand = new RelayCommand<ClipboardCopyType>(o=>CopyToClipboard(o, null));
            Commands.CopySelectedToClipboardCommand = new RelayCommand<Player>(o=>CopyToClipboard(ClipboardCopyType.OnlySelected, o));
            Commands.IsGroupMemberChangedCommand = new RelayCommand<Player>(IsGroupMemberChanged);
            Commands.IsFriendChangedCommand = new RelayCommand<Player>(IsFriendChanged);
        }

        private void InitializeTimers()
        {
            expGainedTimer.Elapsed += EndEncounter;
            timeoutTimer.Elapsed += EndEncounter;
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedLogLanguage":
                    _LogParser.InitializeRegexes();
                    break;
                case "SelectedLanguage":
                    NotifyPropertyChanged("PowerButtonToolTip");
                    break;
            }
        }

        #region Log parser

        private void InitializeLogParser()
        {
            _LogParser.PlayerInflictedDamage += OnDamageInflicted;
            _LogParser.PlayerInflictedCriticalDamage += OnDamageInflicted;
            _LogParser.PlayerInflictedSkillDamage += OnSkillDamageInflicted;
            _LogParser.PlayerReceivedDamage += OnReceivedDamage;
            _LogParser.FileNotFound += OnFileNotFound;
            _LogParser.PlayerJoinedGroup += OnPlayerJoinedGroup;
            _LogParser.PlayerLeftGroup += OnPlayerLeftGroup;
            _LogParser.Started += delegate { NotifyPropertyChanged("IsEnabled"); };
            _LogParser.Stopped += delegate { NotifyPropertyChanged("IsEnabled"); };
            _LogParser.ExpGained += OnExpGained;
            _LogParser.KinahEarned += OnKinahEarned;
            _LogParser.KinahSpent += OnKinahSpent;
            _LogParser.AbyssPointsGained += OnAbyssPointsGained;
            _LogParser.RegionChanged += OnJoinedRegionChannel;
            _LogParser.Open(Settings.Default.AionLogPath);
        }

        private void OnReceivedDamage(object sender, DamageEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, DamageEventArgs>(OnReceivedDamage),
                                                      sender, e);
            }
            else
            {
                string target = (e.Target == AionLogParser.You) ? Settings.Default.YouAlias : e.Target;
                UpdateReceivedDamage(target, e.Damage, e.Name);
            }
        }

        private static void OnFileNotFound(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(WindowMainRes.OpenLogError, WindowMainRes.OptionsBtnToolTip, WindowMainRes.LocateLogMenuHeader), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        private void OnDamageInflicted(object sender, DamageEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, DamageEventArgs>(OnDamageInflicted),
                                                      sender, e);
            }
            else
            {
                string name = (e.Name == AionLogParser.You) ? Settings.Default.YouAlias : e.Name;
                UpdatePlayerDamage(name, e.Target, e.Damage, Resources.WhiteDamageSkillName);
            }
        }

        private void OnSkillDamageInflicted(object sender, SkillDamageEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, SkillDamageEventArgs>(OnSkillDamageInflicted),
                                                      sender, e);
            }
            else
            {
                string name = (e.Name == AionLogParser.You) ? Settings.Default.YouAlias : e.Name;
                UpdatePlayerDamage(name, e.Target, e.Damage, e.Skill);
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
                if(!GroupMembers.Contains(e.Name))
                {
                    GroupMembers.Add(e.Name);
                }
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
                GroupMembers.Remove(e.Name);
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
                lastExpGainedTime = DateTime.Now;
                Exp += e.Exp;
                expGainedTimer.Start();
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
                KinahEarned += e.Kinah;
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
                KinahSpent -= e.Kinah;
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
                Ap += e.Points;
            }
        }

        private void OnJoinedRegionChannel(object sender, RegionEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, RegionEventArgs>(OnJoinedRegionChannel),
                                                      sender, e);
            }
            else
            {
                CurrentRegionName = e.Region;
            }
        }

        public void ChangeLogFile(string fileName)
        {
            _LogParser.Close();
            Settings.Default.AionLogPath = fileName;
            _LogParser.Open(Settings.Default.AionLogPath);
        }

        #endregion

        #region General Methods

        public void OnClose()
        {
            _LogParser.Close();
        }

        /// <summary>
        /// Update a player's damage.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="damage">The total damage the player has dealt</param>
        public void UpdatePlayerDamage(string name, string target, int damage, string skill)
        {
            if(!IsValidPlayer(name))
            {
                return;
            }

            StartEncounterIfNeeded(target);

            if(LastEncounter != null)
            {
                bool isGroupMember = name == Settings.Default.YouAlias ||
                                     GroupMembers.Contains(name) ||
                                     Settings.Default.FriendList.Contains(name);
                LastEncounter.UpdatePlayerDamage(name, damage, skill, isGroupMember);
            }

            lastDamageInflictedTime = DateTime.Now;
            expGainedTimer.Stop();

            timeoutTimer.Stop();
            timeoutTimer.Start();
        }

        private void UpdateReceivedDamage(string name, int damage, string from)
        {
            if (!IsValidPlayer(name))
            {
                return;
            }

            StartEncounterIfNeeded(from);

            if (LastEncounter != null)
            {
                LastEncounter.UpdatePlayerReceivedDamage(name, damage);
            }

            lastDamageInflictedTime = DateTime.Now;
            expGainedTimer.Stop();

            timeoutTimer.Stop();
            timeoutTimer.Start();
        }

        private bool IsValidPlayer(string name)
        {
            if (Settings.Default.IgnoreList.Contains(name))
            {
                return false;
            }

            bool isGroupMember = name == Settings.Default.YouAlias ||
                                 GroupMembers.Contains(name) ||
                                 Settings.Default.FriendList.Contains(name);
            if (Settings.Default.IsHideOthers && name != Settings.Default.YouAlias)
            {
                return false;
            }
            if (Settings.Default.IsGroupOnly && !isGroupMember)
            {
                return false;
            }
            return true;
        }

        private void StartEncounterIfNeeded(string encounterName)
        {
            if (LastRegion == null || LastRegion.Name != CurrentRegionName)
            {
                LastRegion = new Region(AllEncounters) { Name = CurrentRegionName, IsExpanded = true };
                Regions.Add(LastRegion);
            }
            var lastEncounter = LastEncounter;
            if (LastEncounter == null || LastEncounter.IsEnded)
            {
                LastEncounter = new Encounter(LastRegion);
                timeoutTimer.Start();

                if (!string.IsNullOrEmpty(encounterName))
                {
                    LastEncounter.Name = encounterName;
                }
                else
                {
                    LastEncounter.Name = "Encounter " + LastRegion.Encounters.Count + 1;
                }
            }
            if (SelectedEncounter == null || SelectedEncounter == lastEncounter)
            {
                SelectedEncounter = LastEncounter;
            }
            if (LastEncounter.Name == "Unknown" && encounterName != "Unknown")
            {
                LastEncounter.Name = encounterName;
            }
        }

        private void EndEncounter(object sender, ElapsedEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<object, ElapsedEventArgs>(EndEncounter), sender, e);
            }
            else
            {
                expGainedTimer.Stop();
                timeoutTimer.Stop();
                if (LastEncounter != null && !LastEncounter.IsEnded)
                {
                    if (sender == expGainedTimer)
                    {
                        LastEncounter.EndEncounter(lastExpGainedTime);
                    }
                    else if (sender == timeoutTimer)
                    {
                        LastEncounter.EndEncounter(lastDamageInflictedTime);
                    }
                }
            }
        }

        private void RemovePlayer(Player player)
        {
            SelectedEncounter.RemovePlayer(player.PlayerName);
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

            AllEncounters.RemovePlayer(player.PlayerName);
        }

        /// <summary>
        /// Indicates whether the player already exists in the scroll viewer.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <returns>bool</returns>
        public bool PlayerExists(string name)
        {
            return AllEncounters.Players.Any(o => o.PlayerName == name);
        }

        private void ChangePower()
        {
            if (_LogParser.Running)
            {
                _LogParser.Close();
            }
            else
            {
                _LogParser.Open(Settings.Default.AionLogPath);
            }
            NotifyPropertyChanged("IsEnabled");
            NotifyPropertyChanged("PowerButtonToolTip");
        }

        public void Rename(string newName, string oldName)
        {
            var player = AllEncounters.Players.FirstOrDefault(o => o.PlayerName == oldName);
            if (player != null)
            {
                player.PlayerName = newName;
            }

            foreach (var region in AllEncounters.Regions)
            {
                player = region.Players.FirstOrDefault(o => o.PlayerName == oldName);
                if (player != null)
                {
                    player.PlayerName = newName;
                }
                foreach (var encounter in region.Encounters)
                {
                    player = encounter.Players.FirstOrDefault(o => o.PlayerName == oldName);
                    if (player != null)
                    {
                        player.PlayerName = newName;
                    }
                }
            }
        }

        public void AddGroupMemberPlayer(string name)
        {
            if (!GroupMembers.Contains(name))
            {
                GroupMembers.Add(name);
            }
        }
        
        private void DetectAvailableLanguages()
        {
            var dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            var defaultLanguage = allCultures.First(o => o.Name == "en");
            AvailableLanguages.Add(defaultLanguage);
            AvailableLogLanguages.Add(defaultLanguage);

            foreach (var dir in dirs)
            {
                string dirName = Path.GetFileName(dir);
                var availableCulture = allCultures.FirstOrDefault(o => o.Name == dirName);
                if(availableCulture != null)
                {
                    AvailableLanguages.Add(availableCulture);
                    FindRegextLanguage(dir, availableCulture);
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
            if(Settings.Default.SelectedLogLanguage == CultureInfo.InvariantCulture)
            {
                Settings.Default.SelectedLogLanguage = AvailableLogLanguages[0];
            }
        }

        private void FindRegextLanguage(string dirPath, CultureInfo culture)
        {
            try
            {
                foreach (var file in Directory.GetFiles(dirPath, "*.dll"))
                {
                    if (Path.GetFileName(file).Contains(Assembly.GetEntryAssembly().GetName().Name))
                    {
                        Assembly resourceAssembly = Assembly.LoadFrom(file);
                        foreach (string resourceName in resourceAssembly.GetManifestResourceNames())
                        {
                            if (resourceName.EndsWith(".resources"))
                            {
                                ManifestResourceInfo info = resourceAssembly.GetManifestResourceInfo(resourceName);
                                // if this resource is in another assemlby, we will skip it
                                if (info == null || (info.ResourceLocation & ResourceLocation.ContainedInAnotherAssembly) != 0)
                                {
                                    continue; // in resource assembly, we don't have resource that is contained in another assembly
                                }

                                if(resourceName.Contains("Regex"))
                                {
                                    AvailableLogLanguages.Add(culture);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Write(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void SelectLanguage(CultureInfo language)
        {
            var findedLanguage = AvailableLanguages.FirstOrDefault(o => o.Equals(language) || (language.Parent != null && o.Equals(language.Parent)));
            Settings.Default.SelectedLanguage = findedLanguage ?? AvailableLanguages[0];
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
                if (SelectedEncounter != null)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(SelectedEncounter.Players);
                    if (view != null)
                    {
                        var sb = new StringBuilder();
                        foreach (Player player in view)
                        {
                            sb.AppendFormat("{0} {1} {2}{3}", chatPrefix, player.PlayerName, player.Damage,
                                            Environment.NewLine);
                        }
                        Clipboard.SetText(sb.ToString());
                    }
                }
            }
        }

        private void IsGroupMemberChanged(Player groupMember)
        {
            if (groupMember.IsGroupMember)
            {
                if (!GroupMembers.Contains(groupMember.PlayerName))
                {
                    GroupMembers.Add(groupMember.PlayerName);
                }
            }
            else
            {
                GroupMembers.Remove(groupMember.PlayerName);
            }
            
            var player = AllEncounters.Players.FirstOrDefault(o => o.PlayerName == groupMember.PlayerName);
            if (player != null)
            {
                player.IsGroupMember = groupMember.IsGroupMember;
            }

            foreach (var region in AllEncounters.Regions)
            {
                player = region.Players.FirstOrDefault(o => o.PlayerName == groupMember.PlayerName);
                if (player != null)
                {
                    player.IsGroupMember = groupMember.IsGroupMember;
                }
                foreach (var encounter in region.Encounters)
                {
                    player = encounter.Players.FirstOrDefault(o => o.PlayerName == groupMember.PlayerName);
                    if (player != null)
                    {
                        player.IsGroupMember = groupMember.IsGroupMember;
                    }
                }
            }
        }

        private void IsFriendChanged(Player player)
        {
            if(player.IsFriend)
            {
                IsGroupMemberChanged(player);
            }
        }

        private void RemoveEncounter(EncounterBase encounter)
        {
            if(encounter is Region)
            {
                Regions.Remove(encounter);
                if(LastRegion == encounter)
                {
                    LastRegion = null;
                    LastEncounter = null;
                }
            }
            else if(encounter is Encounter)
            {
                ((Encounter) encounter).Parent.Encounters.Remove((Encounter) encounter);
                if (LastEncounter == encounter)
                {
                    LastEncounter = null;
                }
            }
            SelectedEncounter = null;
        }

        private void RemoveAllEncounters()
        {
            Regions.Clear();
            LastRegion = null;
            LastEncounter = null;
            SelectedEncounter = null;
            AllEncounters = new AllEncounters();
            Regions.Add(AllEncounters);
        }

        private void Reset(DisplayType displayType)
        {
            switch (displayType)
            {
                case DisplayType.Experience:
                    Exp = 0;
                    break;
                case DisplayType.Kinah:
                    KinahEarned = 0;
                    KinahSpent = 0;
                    break;
                case DisplayType.AbyssPoints:
                    Ap = 0;
                    break;
            }
        }

        private void ResetAll()
        {
            Reset(DisplayType.Experience);
            Reset(DisplayType.Kinah);
            Reset(DisplayType.AbyssPoints);
        }

        #endregion
    }
}