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

        void Start(string file);
        void Stop();

        event EventHandler Starting;
        event EventHandler Started;
        event EventHandler Stopping;
        event EventHandler Stopped;

        event EventHandler FileNotFound;

        event KinahEventHandler KinahEarned;
        event KinahEventHandler KinahSpent;
        event AbyssPointsEventHandler AbyssPointsGained;
        event ExpEventHandler ExpGained;

        event SessionEventHandler SessionStarted;
        event RegionEventHandler RegionChanged;
        event PlayerEventHandler PlayerJoinedGroup;
        event PlayerEventHandler PlayerLeftGroup;

        event DamageEventHandler PlayerInflictedDamage;
        event DamageEventHandler PlayerInflictedCriticalDamage;
        event SkillDamageEventHandler PlayerInflictedSkillDamage;
        event DamageEventHandler PlayerReceivedDamage;
        event SkillDamageEventHandler PlayerReceivedSkillDamage;
        event HealEventHandler PlayerHealed;
        event HealOtherEventHandler PlayerHealedOther;
    }
}
