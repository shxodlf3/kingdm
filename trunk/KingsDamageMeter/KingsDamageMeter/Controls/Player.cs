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
using KingsDamageMeter.Combat;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public class Player : NotifyPropertyChangedBase
    {
        public Player()
        {
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

        private ClassType playerClass;
        public ClassType PlayerClass
        {
            get { return playerClass; }
            set
            {
                playerClass = value;
                NotifyPropertyChanged("PlayerClass");
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

        private double percentFromGroupDamages;
        public double PercentFromGroupDamages
        {
            get
            {
                if (Double.IsNaN(percentFromGroupDamages))
                {
                    return 0;
                }
                else
                {
                    return percentFromGroupDamages;
                }
            }
            set
            {
                if (percentFromGroupDamages != value)
                {
                    if (value < 0)
                    {
                        percentFromGroupDamages = 0;
                    }
                    else if (value > 1)
                    {
                        percentFromGroupDamages = 1;
                    }
                    else
                    {
                        percentFromGroupDamages = value;
                    }
                    NotifyPropertyChanged("PercentFromGroupDamages");
                }
            }
        }

        private double percentFromTopDamage;
        public double PercentFromTopDamage
        {
            get
            {
                if (Double.IsNaN(percentFromTopDamage))
                {
                    return 0;
                }
                else
                {
                    return percentFromTopDamage;
                }
            }
            set
            {
                if (percentFromTopDamage != value)
                {
                    if (value < 0)
                    {
                        percentFromTopDamage = 0;
                    }
                    else if (value > 1)
                    {
                        percentFromTopDamage = 1;
                    }
                    else
                    {
                        percentFromTopDamage = value;
                    }
                    NotifyPropertyChanged("PercentFromTopDamage");
                }
            }
        }

        private int damagePerSecond;
        public int DamagePerSecond
        {
            get
            {
                return damagePerSecond;
            }
            set
            {
                if (damagePerSecond != value)
                {
                    damagePerSecond = value;
                    //if (value > PeakDps)
                    //{
                    //    PeakDps = value;
                    //}
                    NotifyPropertyChanged("DamagePerSecond");
                }
            }
        }

        //private int peakDps;
        //public int PeakDps
        //{
        //    get { return peakDps; }
        //    private set
        //    {
        //        if (peakDps != value)
        //        {
        //            peakDps = value;
        //            NotifyPropertyChanged("PeakDps");
        //        }
        //    }
        //}

        private bool isGroupMember;
        public bool IsGroupMember
        {
            get { return isGroupMember; }
            set
            {
                if (isGroupMember != value)
                {
                    isGroupMember = value;
                    if(Commands.IsGroupMemberChangedCommand != null)
                    {
                        Commands.IsGroupMemberChangedCommand.Execute(this);
                    }
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
                if (Commands.IsFriendChangedCommand != null)
                {
                    Commands.IsFriendChangedCommand.Execute(this);
                }
                NotifyPropertyChanged("IsFriend");
                NotifyPropertyChanged("IsGroupMember");
            }
        }

        public SkillCollection Skills { get; private set; }

        #endregion
    }
}
