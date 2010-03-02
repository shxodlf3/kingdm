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

namespace KingsDamageMeter
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        private DateTime _Time;

        public DateTime Time
        {
            get
            {
                return _Time;
            }
        }

        public LogEventArgs(DateTime time)
        {
            if (time == null)
            {
                time = DateTime.Now;
            }

            _Time = time;
        }
    }

    public class ExpEventArgs : LogEventArgs
    {
        private int _Exp;

        public int Exp
        {
            get
            {
                return _Exp;
            }
        }

        public ExpEventArgs(DateTime time, int exp)
            : base(time)
        {
            if (exp < 0)
            {
                exp = 0;
            }

            _Exp = exp;
        }
    }

    public class KinahEventArgs : LogEventArgs
    {
        private int _Kinah;

        public int Kinah
        {
            get
            {
                return _Kinah;
            }
        }

        public KinahEventArgs(DateTime time, int kinah)
            : base(time)
        {
            if (kinah < 0)
            {
                kinah = 0;
            }

            _Kinah = kinah;
        }
    }

    public class AbyssPointsEventArgs : LogEventArgs
    {
        private int _Points;

        public int Points
        {
            get
            {
                return _Points;
            }
        }

        public AbyssPointsEventArgs(DateTime time, int points)
            : base(time)
        {
            if (points < 0)
            {
                points = 0;
            }

            _Points = points;
        }
    }

    /// <summary>
    /// Provides data for the KingsDamageMeter.AionLogParser.DamageInflicted event.
    /// </summary>
    public class PlayerDamageEventArgs : PlayerEventArgs
    {
        private int _Damage;

        /// <summary>
        /// Get the value indicating the amount of damage inflicted.
        /// </summary>
        public int Damage
        {
            get
            {
                return _Damage;
            }
        }

        /// <summary>
        /// Provides data for the KingsDamageMeter.AionLogParser.DamageInflicted event.
        /// </summary>
        /// <param name="name">The name of the player inflicting damage.</param>
        /// <param name="damage">The amount of damage inflicted.</param>
        public PlayerDamageEventArgs(DateTime time, string name, int damage)
            : base(time, name)
        {
            if (damage < 0)
            {
                damage = 0;
            }

            _Damage = damage;
        }
    }

    public class PlayerSkillDamageEventArgs : PlayerDamageEventArgs
    {
        private string _Skill;

        public string Skill
        {
            get
            {
                return _Skill;
            }
        }

        public PlayerSkillDamageEventArgs(DateTime time, string name, int damage, string skill)
            : base(time, name, damage)
        {
            if (String.IsNullOrEmpty(skill))
            {
                skill = "Unknown";
            }

            _Skill = skill;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlayerEventArgs : LogEventArgs
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public PlayerEventArgs(DateTime time, string name)
            : base(time)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "Unknown Player";
            }

            _Name = name;
        }
    }

    public class PetEventArgs : LogEventArgs
    {
        private string _Name;
        private string _Owner;

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public string Owner
        {
            get
            {
                return _Owner;
            }
        }

        public PetEventArgs(DateTime time, string name, string owner)
            : base(time)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "Unknown Pet";
            }
            if (String.IsNullOrEmpty(owner))
            {
                owner = "Unknown Owner";
            }

            _Name = name;
            _Owner = owner;
        }
    }

    public class PetDamageEventArgs : PetEventArgs
    {
        private int _Damage;

        public int Damage
        {
            get
            {
                return _Damage;
            }
        }

        public PetDamageEventArgs(DateTime time, string name, string owner, int damage)
            : base(time, name, owner)
        {
            if (damage < 0)
            {
                damage = 0;
            }

            _Damage = damage;
        }
    }
}
