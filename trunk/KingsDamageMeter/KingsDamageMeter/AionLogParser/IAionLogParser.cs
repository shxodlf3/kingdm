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
    public interface IAionLogParser
    {
        bool Running
        {
            get;
        }

        event EventHandler Starting;
        event EventHandler Started;
        event EventHandler Stopping;
        event EventHandler Stopped;
        event EventHandler FileNotFound;
        event DamageInflictedEventHandler DamageInflicted;
        event DamageInflictedEventHandler CriticalInflicted;
        event SkillDamageInflictedEventHandler SkillDamageInflicted;
        event PlayerEventHandler PlayerJoinedGroup;
        event PlayerEventHandler PlayerLeftGroup;
        event DamageInflictedEventHandler DamageReceived;
        event ExpEventHandler ExpGained;
        event KinahEventHandler KinahEarned;

        void Start(string file);
        void Stop();
    }
}
