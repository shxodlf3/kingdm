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

        public int Damage
        {
            get
            {
                return _Damage;
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
