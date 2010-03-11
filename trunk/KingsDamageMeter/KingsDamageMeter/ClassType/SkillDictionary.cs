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

using System.Collections.Generic;

namespace KingsDamageMeter
{
    public class SkillDictionary
    {
        public static SkillDictionary Instance { get; private set; }
        private static Dictionary<string, ClassType> _Skills = new Dictionary<string, ClassType>();
        private static char[] _Numerals = { ' ', 'I', 'V', 'X' };

        static SkillDictionary()
        {
            Instance = new SkillDictionary();
        }

        static ClassType GetClass(string skill)
        {
            if (_Skills.Count < 1)
            {
                PopulateDictionary();
            }

            skill = skill.TrimEnd(_Numerals);

            if (_Skills.ContainsKey(skill))
            {
                return _Skills[skill];
            }
            else
            {
                return ClassType.None;
            }
        }

        private static void PopulateDictionary()
        {
            // Cleric
            _Skills.Add("Light of Rejuvenation", ClassType.Cleric);
            _Skills.Add("Root", ClassType.Cleric);
            _Skills.Add("Thunderbolt", ClassType.Cleric);
            _Skills.Add("Dispel", ClassType.Cleric);
            _Skills.Add("Tranquility", ClassType.Cleric);
            _Skills.Add("Radiant Cure", ClassType.Cleric);
            _Skills.Add("Cure Mind", ClassType.Cleric);
            _Skills.Add("Penance", ClassType.Cleric);
            _Skills.Add("Divine Touch", ClassType.Cleric);
            _Skills.Add("Healing Wind", ClassType.Cleric);
            _Skills.Add("Rebirth", ClassType.Cleric);
            _Skills.Add("Divine Spark", ClassType.Cleric);
            _Skills.Add("Summer Circle", ClassType.Cleric);
            _Skills.Add("Winter Circle", ClassType.Cleric);
            _Skills.Add("Thorny Skin", ClassType.Cleric);
            _Skills.Add("Acquittal", ClassType.Cleric);
            _Skills.Add("Light of Recovery", ClassType.Cleric);
            _Skills.Add("Chastisement", ClassType.Cleric);
            _Skills.Add("Resurrection Loci", ClassType.Cleric);
            _Skills.Add("Prayer of Focus", ClassType.Cleric);
            _Skills.Add("Grace of Resurrection", ClassType.Cleric);
            _Skills.Add("Blessed Shield", ClassType.Cleric);

            // Chanter
            _Skills.Add("Victory Mantra", ClassType.Chanter);
            _Skills.Add("Meteor Strike", ClassType.Chanter);
            _Skills.Add("Shield Mantra", ClassType.Chanter);
            _Skills.Add("Booming Strike", ClassType.Chanter);
            _Skills.Add("Word of Revival", ClassType.Chanter);
            _Skills.Add("Promise of Earth", ClassType.Chanter);
            _Skills.Add("Magic Mantra", ClassType.Chanter);
            _Skills.Add("Incandescent Blow", ClassType.Chanter);
            _Skills.Add("Celerity Mantra", ClassType.Chanter);
            _Skills.Add("Focused Parry", ClassType.Chanter);
            _Skills.Add("Booming Smash", ClassType.Chanter);
            _Skills.Add("Revival Mantra", ClassType.Chanter);
            _Skills.Add("Clement Mind Mantra", ClassType.Chanter);
            _Skills.Add("Parrying Strike", ClassType.Chanter);
            _Skills.Add("Pentacle Shock", ClassType.Chanter);
            _Skills.Add("Word of Wind", ClassType.Chanter);
            _Skills.Add("Booming Assault", ClassType.Chanter);
            _Skills.Add("Intensity Mantra", ClassType.Chanter);
            _Skills.Add("Word of Quickness", ClassType.Chanter);
            _Skills.Add("Protection Mantra", ClassType.Chanter);
            _Skills.Add("Tremor", ClassType.Chanter);

            // Assassin
            _Skills.Add("Dash Attack", ClassType.Assassin);
            _Skills.Add("Killer's Eye", ClassType.Assassin);
            _Skills.Add("Rune Carve", ClassType.Assassin);
            _Skills.Add("Sprinting", ClassType.Assassin);
            _Skills.Add("Pain Rune", ClassType.Assassin);
            _Skills.Add("Apply Poison", ClassType.Assassin);
            _Skills.Add("Fang Strike", ClassType.Assassin);
            _Skills.Add("Rune Slash", ClassType.Assassin);
            _Skills.Add("Clear Focus", ClassType.Assassin);
            _Skills.Add("Whirlwind Slash", ClassType.Assassin);
            _Skills.Add("Binding Rune", ClassType.Assassin);
            _Skills.Add("Beast Kick", ClassType.Assassin);
            _Skills.Add("Searching Eye", ClassType.Assassin);
            _Skills.Add("Wind Walk", ClassType.Assassin);
            _Skills.Add("Assassination", ClassType.Assassin);
            _Skills.Add("Weakening Blow", ClassType.Assassin);
            _Skills.Add("Blood Rune", ClassType.Assassin);
            _Skills.Add("Beast Swipe", ClassType.Assassin);
            _Skills.Add("Spiral Slash", ClassType.Assassin);
            _Skills.Add("Throw Dagger", ClassType.Assassin);

            // Ranger
            _Skills.Add("Swift Shot", ClassType.Ranger);
            _Skills.Add("Entanglng Shot", ClassType.Ranger);
            _Skills.Add("Arrow Strke", ClassType.Ranger);
            _Skills.Add("Stunnng Shot", ClassType.Ranger);
            _Skills.Add("Aiming", ClassType.Ranger);
            _Skills.Add("Poison Arrow", ClassType.Ranger);
            _Skills.Add("Rupture Arrow", ClassType.Ranger);
            _Skills.Add("Strong Shots", ClassType.Ranger);
            _Skills.Add("Tactcal Retreat", ClassType.Ranger);
            _Skills.Add("Aerial Wild Shot", ClassType.Ranger);
            _Skills.Add("Hunter's Eye", ClassType.Ranger);
            _Skills.Add("Arrow Flurry", ClassType.Ranger);
            _Skills.Add("Fleshcutter Arrow", ClassType.Ranger);
            _Skills.Add("Arrow Storm", ClassType.Ranger);

            // Templar
            _Skills.Add("Taunt", ClassType.Templar);
            _Skills.Add("Shield Bash", ClassType.Templar);
            _Skills.Add("Provoking Shield Counter", ClassType.Templar);
            _Skills.Add("Empyrean Armor", ClassType.Templar);
            _Skills.Add("Shining Slash", ClassType.Templar);
            _Skills.Add("Wrath Strike", ClassType.Templar);
            _Skills.Add("Avenging Blow", ClassType.Templar);
            _Skills.Add("Steel Wall Defense", ClassType.Templar);
            _Skills.Add("Divine Blow", ClassType.Templar);
            _Skills.Add("Dazing Severe Blow", ClassType.Templar);
            _Skills.Add("Face Smash", ClassType.Templar);
            _Skills.Add("Hand of Healing", ClassType.Templar);
            _Skills.Add("Blunting Severe Blow", ClassType.Templar);
            _Skills.Add("Judgment", ClassType.Templar);
            _Skills.Add("Iron Skin", ClassType.Templar);
            _Skills.Add("Divine Grasp", ClassType.Templar);

            // Gladiator
            _Skills.Add("", ClassType.Gladiator);
            _Skills.Add("Seismic Wave", ClassType.Gladiator);
            _Skills.Add("Wrathful Strike", ClassType.Gladiator);
            _Skills.Add("Aion's Strength", ClassType.Gladiator);
            _Skills.Add("Rupture", ClassType.Gladiator);
            _Skills.Add("Cleave", ClassType.Gladiator);
            _Skills.Add("Aerial Lockdown", ClassType.Gladiator);
            _Skills.Add("Body Smash", ClassType.Gladiator);
            _Skills.Add("Crashing Blow", ClassType.Gladiator);
            _Skills.Add("Shock Wave", ClassType.Gladiator);
            _Skills.Add("Seismic Billow", ClassType.Gladiator);
            _Skills.Add("Reckless Strike", ClassType.Gladiator);
            _Skills.Add("Stamina Recovery", ClassType.Gladiator);
            _Skills.Add("Wall of Steel", ClassType.Gladiator);
            _Skills.Add("Pressure Wave", ClassType.Gladiator);
            _Skills.Add("Great Cleave", ClassType.Gladiator);
            _Skills.Add("Wrathful Wave", ClassType.Gladiator);
            _Skills.Add("Righteous Cleave", ClassType.Gladiator);
            _Skills.Add("Strengthen Wings", ClassType.Gladiator);

            // Sorcerer
            _Skills.Add("Robe of Earth", ClassType.Sorcerer);
            _Skills.Add("Flame Harpoon", ClassType.Sorcerer);
            _Skills.Add("Blind Leap", ClassType.Sorcerer);
            _Skills.Add("Winter Binding", ClassType.Sorcerer);
            _Skills.Add("Flame Cage", ClassType.Sorcerer);
            _Skills.Add("Delayed Blast", ClassType.Sorcerer);
            _Skills.Add("Robe of Flame", ClassType.Sorcerer);
            _Skills.Add("Freezing Wind", ClassType.Sorcerer);
            _Skills.Add("Boon of Peace", ClassType.Sorcerer);
            _Skills.Add("Aether's Hold", ClassType.Sorcerer);
            _Skills.Add("Aether Flame", ClassType.Sorcerer);
            _Skills.Add("Inferno", ClassType.Sorcerer);
            _Skills.Add("Robe of Cold", ClassType.Sorcerer);
            _Skills.Add("Tranquilizing Cloud", ClassType.Sorcerer);
            _Skills.Add("Gain Mana", ClassType.Sorcerer);
            _Skills.Add("Flame Fusion", ClassType.Sorcerer);
            _Skills.Add("Magic Fist", ClassType.Sorcerer);
            _Skills.Add("Soul Absorption", ClassType.Sorcerer);
            _Skills.Add("Soul Freeze", ClassType.Sorcerer);
            _Skills.Add("Flaming Meteor", ClassType.Sorcerer);
            _Skills.Add("Cometfall", ClassType.Sorcerer);

            // Spiritmaster
            _Skills.Add("Root of Enervation", ClassType.Spiritmaster);
            _Skills.Add("Summon Fire Spirit", ClassType.Spiritmaster);
            _Skills.Add("Summon Wind Spirit", ClassType.Spiritmaster);
            _Skills.Add("Chain of Earth", ClassType.Spiritmaster);
            _Skills.Add("Summon Fire Energy", ClassType.Spiritmaster);
            _Skills.Add("Dispel Magic", ClassType.Spiritmaster);
            _Skills.Add("Summon Water Spirit", ClassType.Spiritmaster);
            _Skills.Add("Summon Wind Servant", ClassType.Spiritmaster);
            _Skills.Add("Spirit Erosion", ClassType.Spiritmaster);
            _Skills.Add("Stone Shock", ClassType.Spiritmaster);
            _Skills.Add("Sandblaster", ClassType.Spiritmaster);
            _Skills.Add("Summon Group Member", ClassType.Spiritmaster);
            _Skills.Add("Disenchant", ClassType.Spiritmaster);
            _Skills.Add("Summoning Alacrity", ClassType.Spiritmaster);
            _Skills.Add("Spirit Absorption", ClassType.Spiritmaster);
            _Skills.Add("Blade of Earth", ClassType.Spiritmaster);
            _Skills.Add("Wing Root", ClassType.Spiritmaster);
            _Skills.Add("Erosion", ClassType.Spiritmaster);
            _Skills.Add("Stone Skin", ClassType.Spiritmaster);
        }
    }
}
