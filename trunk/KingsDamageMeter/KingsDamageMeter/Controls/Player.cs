using System;
using System.ComponentModel;
using KingsDamageMeter.Combat;
using KingsDamageMeter.Properties;
using Timer=System.Timers.Timer;

namespace KingsDamageMeter.Controls
{
    public class Player : INotifyPropertyChanged
    {
        private readonly Timer fightTimer = new Timer(1000);
        private readonly Timer endOfFightTimer = new Timer(5000);
        private DateTime timeSinceLastDamage;

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

            Skills = new SkillCollection();
        }

        #region Properties

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                if (playerName != value)
                {
                    playerName = value;
                    NotifyPropertyChanged("PlayerName");
                }
            }
        }

        private long damage;
        public long Damage
        {
            get { return damage; }
            set
            {
                if (damage != value)
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
                }
            }
        }

        private int biggestHit;
        public int BiggestHit
        {
            get { return biggestHit; }
            set
            {
                if (biggestHit != value)
                {
                    biggestHit = value;
                    NotifyPropertyChanged("BiggestHit");
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
                if (damagePercent != value)
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
                }
            }
        }

        public int DamagePerSecond
        {
            get
            {
                if (FightTime <= 3)
                {
                    return 0;
                }
                var value = (int)(Damage / FightTime);
                if (value > PeakDps)
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
                    NotifyPropertyChanged("IsGroupMember");
                }
            }
        }

        public bool IsFriend
        {
            get { return PlayerName == Settings.Default.YouAlias || Settings.Default.FriendList.Contains(PlayerName); }
            set
            {
                if (value)
                {
                    IsGroupMember = true;
                    if (!Settings.Default.FriendList.Contains(PlayerName))
                    {
                        Settings.Default.FriendList.Add(PlayerName);
                    }
                }
                else
                {
                    if (Settings.Default.FriendList.Contains(PlayerName))
                    {
                        Settings.Default.FriendList.Remove(PlayerName);
                    }
                }
                NotifyPropertyChanged("IsFriend");
                NotifyPropertyChanged("IsGroupMember");
            }
        }

        public SkillCollection Skills { get; private set; }

        #endregion

        public virtual void Reset()
        {
            Damage = 0;
            DamageTaken = 0;
            DamagePercent = 0;
            FightTime = 0;
            PeakDps = 0;

            Skills.Clear();
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
