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
using System.Collections.Generic;

namespace KingsDamageMeter.Combat
{
    public class SkillCollection
    {
        private Dictionary<string, Skill> _Skills = new Dictionary<string, Skill>();

        public Dictionary<string, Skill>.KeyCollection Keys
        {
            get
            {
                return _Skills.Keys;
            }
        }

        public void Add(string name)
        {
            if (_Skills.ContainsKey(name))
            {
                return;
            }

            else
            {
                _Skills.Add(name, new Skill(name));
            }
        }

        public Skill Get(string name)
        {
            if (!_Skills.ContainsKey(name))
            {
                Add(name);
            }

            return _Skills[name];
        }

        public void Incriment(string name, int damage)
        {
            Add(name);
            _Skills[name].Increment(damage);
        }

        public void Remove(string name)
        {
            if (_Skills.ContainsKey(name))
            {
                _Skills.Remove(name);
            }
        }

        public void Clear()
        {
            if (_Skills.Count > 0)
            {
                _Skills.Clear();
            }
        }
    }
}
