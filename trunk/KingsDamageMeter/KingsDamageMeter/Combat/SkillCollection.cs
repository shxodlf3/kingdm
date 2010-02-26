using System;
using System.Collections.Generic;

namespace KingsDamageMeter.Combat
{
    public class SkillCollection
    {
        private Dictionary<string, int> _Skills = new Dictionary<string, int>();

        public Dictionary<string, int>.KeyCollection Keys
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
                _Skills.Add(name, 0);
            }
        }

        public int Get(string name)
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
            _Skills[name] += damage;
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
