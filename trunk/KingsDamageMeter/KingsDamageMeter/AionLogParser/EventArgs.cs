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
            _Time = time;
        }
    }

    /// <summary>
    /// Provides data for the KingsDamageMeter.AionLogParser.DamageInflicted event.
    /// </summary>
    public class DamageEventArgs : LogEventArgs
    {
        private string _Name;
        private int _Damage;

        /// <summary>
        /// Gets the value indicating the name of the player inflicting damage.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

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
        public DamageEventArgs(DateTime time, string name, int damage) : base(time)
        {
            _Name = name;
            _Damage = damage;
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

        public PlayerEventArgs(DateTime time, string name) : base(time)
        {
            _Name = name;
        }
    }
}
