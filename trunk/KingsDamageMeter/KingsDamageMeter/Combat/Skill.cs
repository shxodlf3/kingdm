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

namespace KingsDamageMeter.Combat
{
    public class Skill
    {
        private string _Name = String.Empty;
        private int _Uses = 0;
        private int _Damage = 0;

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public int Uses
        {
            get
            {
                return _Uses;
            }
        }

        public string UsesFormatted
        {
            get
            {
                return _Uses.ToString();
            }
        }

        public int Damage
        {
            get
            {
                return _Damage;
            }
        }

        public string DamageFormatted
        {
            get
            {
                return _Damage.ToString("#,#");
            }
        }

        public Skill(string name)
        {
            _Name = name;
        }

        public void Increment(int damage)
        {
            if (damage > 0)
            {
                _Damage += damage;
                _Uses++;
            }
        }
    }
}
