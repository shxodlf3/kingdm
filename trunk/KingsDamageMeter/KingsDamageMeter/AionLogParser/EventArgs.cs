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
    public class LogEventArgs : EventArgs
    {
        public DateTime Time
        {
            get;
            private set;
        }

        public LogEventArgs(DateTime time)
        {
            Time = time;
        }
    }

    public class ExpEventArgs : LogEventArgs
    {
        public int Exp
        {
            get;
            private set;
        }

        public ExpEventArgs(DateTime time, int exp)
            : base(time)
        {
            Exp = exp;
        }
    }

    public class KinahEventArgs : LogEventArgs
    {
        public int Kinah
        {
            get;
            private set;
        }

        public KinahEventArgs(DateTime time, int kinah)
            : base(time)
        {
            Kinah = kinah;
        }
    }

    public class AbyssPointsEventArgs : LogEventArgs
    {
        public int Points
        {
            get;
            private set;
        }

        public AbyssPointsEventArgs(DateTime time, int points)
            : base(time)
        {
            Points = points;
        }
    }

    public class RegionEventArgs : LogEventArgs
    {
        public string Region
        {
            get;
            private set;
        }

        public RegionEventArgs(DateTime time, string region)
            : base(time)
        {
            Region = region;
        }
    }

    public class PlayerEventArgs : LogEventArgs
    {
        public string Name
        {
            get;
            private set;
        }

        public PlayerEventArgs(DateTime time, string name)
            : base(time)
        {
            Name = name;
        }
    }

    public class DamageEventArgs : PlayerEventArgs
    {
        public int Damage
        {
            get;
            private set;
        }

        public string Target
        {
            get;
            private set;
        }

        public DamageEventArgs(DateTime time, string name, string target, int damage)
            : base(time, name)
        {
            Damage = damage;
            Target = target;
        }
    }

    public class SkillDamageEventArgs : DamageEventArgs
    {
        public string Skill
        {
            get;
            private set;
        }

        public SkillDamageEventArgs(DateTime time, string name, string target, int damage, string skill)
            : base(time, name, target, damage)
        {
            Skill = skill;
        }
    }

    public class HealEventArgs : PlayerEventArgs
    {
        public int Health
        {
            get;
            private set;
        }

        public HealEventArgs(DateTime time, string name, int health)
            : base(time, name)
        {
        }
    }

    public class HealOtherEventArgs : HealEventArgs
    {
        public string Target
        {
            get;
            private set;
        }

        public HealOtherEventArgs(DateTime time, string name, int health, string target)
            : base(time, name, health)
        {
            Target = target;
        }
    }
}
