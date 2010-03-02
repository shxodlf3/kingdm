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

namespace KingsDamageMeter
{
    /// <summary>
    /// Represents the method that will handle the KingsDamageMeter.AionLogParser.DamageInflicted event of a KingsDamageMeter.AionLogParser
    /// </summary>
    public delegate void DamageInflictedEventHandler(object sender, PlayerDamageEventArgs e);

    public delegate void SkillDamageInflictedEventHandler(object sender, PlayerSkillDamageEventArgs e);

    public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);

    public delegate void ExpEventHandler(object sender, ExpEventArgs e);

    public delegate void KinahEventHandler(object sender, KinahEventArgs e);

    public delegate void AbyssPointsEventHandler(object sender, AbyssPointsEventArgs e);
}
