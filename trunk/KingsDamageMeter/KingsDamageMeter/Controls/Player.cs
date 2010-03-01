using System;
using System.ComponentModel;
using System.Text;
using System.Timers;
using KingsDamageMeter.Combat;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public class Player : INotifyPropertyChanged
    {
        private readonly Timer fightTimer = new Timer(1000);
        private readonly Timer endOfFightTimer = new Timer(5000);
        private DateTime timeSinceLastDamage;
        private DateTime startTime = DateTime.Now;

        public Player()
        {
            fightTimer.Elapsed += delegate
                                      {
                                          FightTime += fightTimer.Interval / 1000;
                                          //System.Diagnostics.Debug.WriteLine("FightTime: " + FightTime);
                                      };
            endOfFightTimer.Elapsed += delegate
                                           {
                                               fightTimer.Enabled = false;
                                               endOfFightTimer.Enabled = false;
                                               FightTime -= (DateTime.Now - timeSinceLastDamage).TotalSeconds;
                                               //System.Diagnostics.Debug.WriteLine("Fight has ended: " + FightTime);
                                           };

            Settings.Default.PropertyChanged += SettingsChanged;
            Skills = new SkillCollection();
        }

        #region Properties

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                if(playerName != value)
                {
                    playerName = value;
                    NotifyPropertyChanged("PlayerName");
                }
            }
        }

        public string DisplayData
        {
            get
            {
                switch (Settings.Default.DisplayType)
                {
                    case DisplayType.Damage:
                        return Damage.ToString("#,#");
                    case DisplayType.DamagePerSecond:
                        return DamagePerSecond.ToString();
                    case DisplayType.Percent:
                        return DamagePercent.ToString("0%");
                    case DisplayType.Experience:
                        return Exp.ToString();
                    case DisplayType.Kinah:
                        return Kinah.ToString("#,#");
                    default:
                        return string.Empty;
                }
            }
        }
        private long damage;
        public long Damage
        {
            get { return damage; }
            set
            {
                if(damage != value)
                {
                    if (value > damage)
                    {
                        long amount = value - damage;

                        if (amount > BiggestHit)
                        {
                            BiggestHit = (int)amount;
                        }
                    } 
                    
                    damage = value;
                    timeSinceLastDamage = DateTime.Now;
                    if (!fightTimer.Enabled)
                    {
                        fightTimer.Start();
                    }
                    endOfFightTimer.Stop();
                    endOfFightTimer.Start();

                    NotifyPropertyChanged("Damage");
                    NotifyPropertyChanged("DisplayData");
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

        private int biggestHit;
        public int BiggestHit
        {
            get { return biggestHit; }
            set
            {
                if(biggestHit != value)
                {
                    biggestHit = value;
                    NotifyPropertyChanged("BiggestHit");
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

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
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

        private int kinah;
        public int Kinah
        {
            get { return kinah; }
            set
            {
                if (kinah != value)
                {
                    kinah = value;
                    NotifyPropertyChanged("Kinah");
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

        private int damageTaken;
        public int DamageTaken
        {
            get { return damageTaken; }
            set
            {
                if (damageTaken != value)
                {
                    damageTaken = value < 0 ? 0 : value;
                    NotifyPropertyChanged("DamageTaken");
                }
            }
        }

        private double damagePercent;
        public double DamagePercent
        {
            get { return damagePercent; }
            set
            {
                if(damagePercent != value)
                {
                    if (value < 0)
                    {
                        damagePercent = 0;
                    }
                    else if (value > 100)
                    {
                        damagePercent = 100;
                    }
                    else
                    {
                        damagePercent = value;
                    }
                    NotifyPropertyChanged("DamagePercent");
                    NotifyPropertyChanged("DamagePercentFormatted");
                }
            }
        }

        public string DamagePercentFormatted
        {
            get { return DamagePercent.ToString("0%"); }
        }

        public int DamagePerSecond
        {
            get
            {
                if(FightTime <= 3)
                {
                    return 0;
                }
                var value = (int) (Damage/FightTime);
                if(value > PeakDps)
                {
                    PeakDps = value;
                }
                return value;
            }
        }

        private double fightTime;
        public double FightTime
        {
            get { return fightTime; }
            set
            {
                if (fightTime != value)
                {
                    fightTime = value;
                    NotifyPropertyChanged("FightTime");
                    NotifyPropertyChanged("DamagePerSecond");
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

        private int peakDps;
        public int PeakDps
        {
            get { return peakDps; }
            private set
            {
                if (peakDps != value)
                {
                    peakDps = value;
                    NotifyPropertyChanged("PeakDps");
                    NotifyPropertyChanged("ToolTipInfo");
                }
            }
        }

        private bool isGroupMember;
        public bool IsGroupMember
        {
            get { return isGroupMember; }
            set
            {
                if (isGroupMember != value)
                {
                    isGroupMember = value;
                    if(value)
                    {
                        if(!Settings.Default.GroupList.Contains(PlayerName))
                        {
                            Settings.Default.GroupList.Add(PlayerName);
                        }
                    }
                    else
                    {
                        if (Settings.Default.GroupList.Contains(PlayerName))
                        {
                            Settings.Default.GroupList.Remove(PlayerName);
                        }
                    }
                    NotifyPropertyChanged("IsGroupMember");
                }
            }
        }

        public string ToolTipInfo
        {
            get
            {
                var message = new StringBuilder();

                message.AppendLine(PlayerName);
                message.AppendLine(Damage + " " + PlayerToolTipRes.PlayerToolTipTotal);
                message.AppendLine(DamagePercent.ToString("0%"));
                message.AppendLine();
                message.AppendLine(DamagePerSecond + " " + PlayerToolTipRes.PlayerToolTipDps);
                message.AppendLine(PeakDps + " " + PlayerToolTipRes.PlayerToolTipPeak);
                message.AppendLine();
                message.Append(BiggestHit + " " + PlayerToolTipRes.PlayerToolTipBiggestHit);

                if(Kinah > 0 || Exp > 0)
                {
                    message.AppendLine();
                }
                if(Kinah > 0)
                {
                    message.AppendLine();
                    message.Append(Kinah + " " + PlayerToolTipRes.PlayerToolTipKinah);
                }
                if (Exp > 0)
                {
                    message.AppendLine();
                    message.Append(ExpPerHour + " " + PlayerToolTipRes.PlayerToolTipExpPerHour);
                }

                return message.ToString();
            }
        }

        public int ExpPerHour
        {
            get
            {
                if(Exp == 0)
                {
                    return 0;
                }
                TimeSpan span = DateTime.Now - startTime;
                return (int)((Exp / span.TotalSeconds) * 3600);
            }
        }

        public SkillCollection Skills { get; private set; }

        #endregion

        #region RemovePlayerCommand

        public event Action<Player> RemoveMe;

        private ObjectRelayCommand removePlayerCommand;
        public ObjectRelayCommand RemovePlayerCommand
        {
            get
            {
                if (removePlayerCommand == null)
                {
                    removePlayerCommand = new ObjectRelayCommand(o=>RemovePlayer());
                }
                return removePlayerCommand;
            }
        }

        private void RemovePlayer()
        {
            if(RemoveMe != null)
            {
                RemoveMe(this);
            }
        }

        #endregion

        #region IgnorePlayerCommand

        private ObjectRelayCommand ignorePlayerCommand;
        public ObjectRelayCommand IgnorePlayerCommand
        {
            get
            {
                if (ignorePlayerCommand == null)
                {
                    ignorePlayerCommand = new ObjectRelayCommand(o=>IgnorePlayer());
                }
                return ignorePlayerCommand;
            }
        }

        private void IgnorePlayer()
        {
            Settings.Default.IgnoreList.Add(PlayerName);
            RemovePlayer();
        }

        #endregion
        
        public void Reset()
        {
            Damage = 0;
            DamageTaken = 0;
            DamagePercent = 0;
            FightTime = 0;
            PeakDps = 0;
            Exp = 0;
            Kinah = 0;

            startTime = DateTime.Now;
            Skills.Clear();
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "DisplayType")
            {
                NotifyPropertyChanged("DisplayData");
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
