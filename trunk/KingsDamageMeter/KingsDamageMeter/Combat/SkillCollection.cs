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
