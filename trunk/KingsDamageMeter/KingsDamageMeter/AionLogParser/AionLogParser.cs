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
using System.Text.RegularExpressions;
using System.Collections.Generic;

using KingsDamageMeter.Properties;

namespace KingsDamageMeter
{
    public class AionLogParser : LogReader
    {
        public const string You = "YOU";
        private string _TimeFormat = Settings.Default.LogTimeFormat;

        private Dictionary<string, string> _Dots = new Dictionary<string, string>();
        private Dictionary<string, string> _Pets = new Dictionary<string, string>();
        private Dictionary<string, string> _Effects = new Dictionary<string, string>();

        private string _NameGroupName = "name";
        private string _DamageGroupName = "damage";
        private string _SkillGroupName = "skill";
        private string _PetGroupName = "pet";
        private string _TimeGroupName = "time";
        private string _TargetGroupName = "target";
        private string _EffectGroupName = "effect";
        private string _ExpGroupName = "exp";
        private string _KinahGroupName = "kinah";
        private string _ApGroupName = "ap";
        private string _RegionGroupName = "region";

        private string _TimestampRegex;
        private Regex _ChatRegex;
        private Regex _YouInflictedRegex;
        private Regex _YouInflictedSkillRegex;
        private Regex _YouCriticalRegex;
        private Regex _YouEffectDamageRegex;
        private Regex _YouGainedEffectRegex;
        private Regex _YouInflictedBleedRegex;
        private Regex _OtherInflictedRegex;
        private Regex _OtherInflictedSkillRegex;
        private Regex _OtherInflictedBleedRegex;
        private Regex _YouReceivedRegex;
        private Regex _OtherReceivedRegex;
        private Regex _OtherReceivedSkillRegex;
        private Regex _OtherReceivedBleedRegex;
        private Regex _YouContinuousRegex;
        private Regex _OtherContinuousRegex;
        private Regex _YouSummonedRegex;
        private Regex _YouSummonedAttackRegex;
        private Regex _OtherSummonedRegex;
        private Regex _OtherSummonedAttackRegex;
        private Regex _JoinedGroupRegex;
        private Regex _LeftGroupRegex;
        private Regex _KickedFromGroupRegex;
        private Regex _YouGainedExpRegex;
        private Regex _YouEarnedKinahRegex;
        private Regex _YouSpentKinahRegex;
        private Regex _YouGainedApRegex;
        private Regex _YouHaveJoinedChannelRegex;

        public event KinahEventHandler KinahEarned;
        public event KinahEventHandler KinahSpent;
        public event AbyssPointsEventHandler AbyssPointsGained;
        public event ExpEventHandler ExpGained;

        public event SessionEventHandler SessionStarted;
        public event RegionEventHandler RegionChanged;
        public event PlayerEventHandler PlayerJoinedGroup;
        public event PlayerEventHandler PlayerLeftGroup;

        public event DamageEventHandler PlayerInflictedDamage;
        public event DamageEventHandler PlayerInflictedCriticalDamage;
        public event SkillDamageEventHandler PlayerInflictedSkillDamage;
        public event DamageEventHandler PlayerReceivedDamage;
        public event SkillDamageEventHandler PlayerReceivedSkillDamage;
        public event HealEventHandler PlayerHealed;
        public event HealOtherEventHandler PlayerHealedOther;

        public AionLogParser()
        {
            InitializeRegexes();
            DataRead += OnDataRead;
        }

        public void InitializeRegexes()
        {
            _TimestampRegex = Localization.Regex.TimestampRegex;
            _ChatRegex = new Regex(Localization.Regex.Chat, RegexOptions.Compiled);
            _YouInflictedRegex = new Regex(_TimestampRegex + Localization.Regex.YouInflictedRegex, RegexOptions.Compiled);
            _YouInflictedSkillRegex = new Regex(_TimestampRegex + Localization.Regex.YouInflictedSkillRegex, RegexOptions.Compiled);
            _YouInflictedBleedRegex = new Regex(_TimestampRegex + Localization.Regex.YouInflictedBleedRegex, RegexOptions.Compiled);
            _YouCriticalRegex = new Regex(_TimestampRegex + Localization.Regex.YouCriticalRegex, RegexOptions.Compiled);
            _YouEffectDamageRegex = new Regex(_TimestampRegex + Localization.Regex.YouEffectDamageRegex, RegexOptions.Compiled);
            _YouGainedEffectRegex = new Regex(_TimestampRegex + Localization.Regex.YouGainedEffectRegex, RegexOptions.Compiled);
            _OtherInflictedRegex = new Regex(_TimestampRegex + Localization.Regex.OtherInflictedRegex, RegexOptions.Compiled);
            _OtherInflictedSkillRegex = new Regex(_TimestampRegex + Localization.Regex.OtherInflictedSkillRegex, RegexOptions.Compiled);
            _OtherInflictedBleedRegex = new Regex(_TimestampRegex + Localization.Regex.OtherInflictedBleedRegex, RegexOptions.Compiled);
            _YouReceivedRegex = new Regex(_TimestampRegex + Localization.Regex.YouReceivedRegex, RegexOptions.Compiled);
            _OtherReceivedRegex = new Regex(_TimestampRegex + Localization.Regex.OtherReceivedRegex, RegexOptions.Compiled);
            _OtherReceivedSkillRegex = new Regex(_TimestampRegex + Localization.Regex.OtherReceivedSkillRegex, RegexOptions.Compiled);
            _OtherReceivedBleedRegex = new Regex(_TimestampRegex + Localization.Regex.OtherReceivedBleedRegex, RegexOptions.Compiled);
            _YouContinuousRegex = new Regex(_TimestampRegex + Localization.Regex.YouContinuousRegex, RegexOptions.Compiled);
            _OtherContinuousRegex = new Regex(_TimestampRegex + Localization.Regex.OtherContinuousRegex, RegexOptions.Compiled);
            _YouSummonedRegex = new Regex(_TimestampRegex + Localization.Regex.YouSummonedRegex, RegexOptions.Compiled);
            _YouSummonedAttackRegex = new Regex(_TimestampRegex + Localization.Regex.YouSummonedAttackRegex, RegexOptions.Compiled);
            _OtherSummonedRegex = new Regex(_TimestampRegex + Localization.Regex.OtherSummonedRegex, RegexOptions.Compiled);
            _OtherSummonedAttackRegex = new Regex(_TimestampRegex + Localization.Regex.OtherSummonedAttackRegex, RegexOptions.Compiled);
            _JoinedGroupRegex = new Regex(_TimestampRegex + Localization.Regex.JoinedGroupRegex, RegexOptions.Compiled);
            _LeftGroupRegex = new Regex(_TimestampRegex + Localization.Regex.LeftGroupRegex, RegexOptions.Compiled);
            _KickedFromGroupRegex = new Regex(_TimestampRegex + Localization.Regex.KickedFromGroupRegex, RegexOptions.Compiled);
            _YouGainedExpRegex = new Regex(_TimestampRegex + Localization.Regex.YouGainedExpRegex, RegexOptions.Compiled);
            _YouEarnedKinahRegex = new Regex(_TimestampRegex + Localization.Regex.YouEarnedKinahRegex, RegexOptions.Compiled);
            _YouSpentKinahRegex = new Regex(_TimestampRegex + Localization.Regex.YouSpentKinahRegex, RegexOptions.Compiled);
            _YouGainedApRegex = new Regex(_TimestampRegex + Localization.Regex.YouGainedApRegex, RegexOptions.Compiled);
            _YouHaveJoinedChannelRegex = new Regex(_TimestampRegex + Localization.Regex.YouHaveJoinedChannelRegex, RegexOptions.Compiled);

            //string str = "Канибал использует: Стремительный удар V. Вулкан Панука получает 1 216 ед. урона.";
            //var regex = new Regex("(?<name>.+) использует: (?<skill>.+)\\. (?<target>.+) получает (?<damage>[^a-zA-Z]+) ед\\. урона\\.", RegexOptions.Compiled);
            //var result = regex.Match(str);
        }

        private void OnDataRead(object sender, ReadEventArgs e)
        {
            ParseLine(e.Data);
        }

        private void ParseLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                return;
            }

            MatchCollection matches;

            matches = _ChatRegex.Matches(line);
            if (matches.Count > 0)
            {
                return;
            }

            bool matched = false;
            string regex = String.Empty;

            try
            {
                matches = _YouInflictedSkillRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (PlayerInflictedSkillDamage != null)
                    {
                        PlayerInflictedSkillDamage(this,
                                             new SkillDamageEventArgs(time, You, target, damage,
                                                                            skill));
                    }

                    matched = true;
                    regex = "_YouInflictedSkillRegex";
                    return;
                }

                matches = _YouInflictedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (PlayerInflictedDamage != null)
                    {
                        PlayerInflictedDamage(this, new DamageEventArgs(time, You, target, damage));
                    }

                    matched = true;
                    regex = "_YouInflictedRegex";
                    return;
                }

                matches = _YouCriticalRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (PlayerInflictedCriticalDamage != null)
                    {
                        PlayerInflictedCriticalDamage(this, new DamageEventArgs(time, You, target, damage));
                    }

                    matched = true;
                    regex = "_YouCriticalRegex";
                    return;
                }

                matches = _YouEffectDamageRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string effect = matches[0].Groups[_EffectGroupName].Value;

                    if (PlayerInflictedSkillDamage != null)
                    {
                        PlayerInflictedSkillDamage(this,
                                             new SkillDamageEventArgs(time, You, target, damage,
                                                                            effect));
                    }

                    matched = true;
                    regex = "_YouEffectDamageRegex";
                    return;
                }

                matches = _YouGainedEffectRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string effect = matches[0].Groups[_EffectGroupName].Value;

                    if (_Effects.ContainsKey(effect))
                    {
                        if (_Effects[effect] != You)
                        {
                            _Effects[effect] = You;
                        }
                    }

                    else
                    {
                        _Effects.Add(effect, You);
                    }

                    matched = true;
                    regex = "_YouGainedEffectRegex";
                    return;
                }

                matches = _YouReceivedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (PlayerReceivedDamage != null)
                    {
                        PlayerReceivedDamage(this, new DamageEventArgs(time, target, You, damage));
                    }

                    matched = true;
                    regex = "_YouReceivedRegex";
                    return;
                }

                matches = _YouContinuousRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill] != You)
                        {
                            _Dots[skill] = You;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, You);
                    }

                    matched = true;
                    regex = "_YouContinuousRegex";
                    return;
                }

                matches = _YouInflictedBleedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill] != You)
                        {
                            _Dots[skill] = You;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, You);
                    }

                    matched = true;
                    regex = "_YouContinuousRegex";
                    return;
                }

                matches = _YouSummonedAttackRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string pet = matches[0].Groups[_PetGroupName].Value;

                    if (_Pets.ContainsKey(pet))
                    {
                        if (_Pets[pet] != You)
                        {
                            _Pets[pet] = You;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, You);
                    }

                    matched = true;
                    regex = "_YouSummonedAttackRegex";
                    return;
                }

                matches = _YouSummonedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string pet = matches[0].Groups[_PetGroupName].Value;

                    if (_Pets.ContainsKey(pet))
                    {
                        if (_Pets[pet] != You)
                        {
                            _Pets[pet] = You;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, You);
                    }

                    matched = true;
                    regex = "_YouSummonedRegex";
                    return;
                }

                matches = _OtherInflictedSkillRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (String.IsNullOrEmpty(name))
                    {
                        return;
                    }

                    if (name.Contains(" "))
                    {
                        if (_Pets.ContainsKey(name))
                        {
                            name = _Pets[name];

                            if (PlayerInflictedSkillDamage != null)
                            {
                                PlayerInflictedSkillDamage(this, new SkillDamageEventArgs(time, name, target, damage, skill));
                            }
                        }
                    }

                    else
                    {
                        if (PlayerInflictedSkillDamage != null)
                        {
                            PlayerInflictedSkillDamage(this, new SkillDamageEventArgs(time, name, target, damage, skill));
                        }
                    }

                    matched = true;
                    regex = "_OtherInflictedSkillRegex";
                    return;
                }

                matches = _OtherInflictedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (String.IsNullOrEmpty(name))
                    {
                        return;
                    }

                    if (name.Contains(" "))
                    {
                        if (_Pets.ContainsKey(name))
                        {
                            string pet = name;
                            name = _Pets[pet];

                            if (PlayerInflictedSkillDamage != null)
                            {
                                PlayerInflictedSkillDamage(this, new SkillDamageEventArgs(time, name, target, damage, pet));
                            }
                        }
                    }

                    else
                    {
                        if (PlayerInflictedDamage != null)
                        {
                            PlayerInflictedDamage(this, new DamageEventArgs(time, name, target, damage));
                        }
                    }

                    matched = true;
                    regex = "_OtherInflictedRegex";
                    return;
                }

                matches = _OtherReceivedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (!name.Contains(" "))
                    {
                        if (PlayerReceivedDamage != null)
                        {
                            PlayerReceivedDamage(this, new DamageEventArgs(time, name, target, damage));
                        }
                    }

                    matched = true;
                    regex = "_OtherReceivedRegex";
                    return;
                }

                matches = _OtherReceivedSkillRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (!name.Contains(" "))
                    {
                        if (PlayerReceivedDamage != null)
                        {
                            PlayerReceivedDamage(this, new DamageEventArgs(time, skill, name, damage));
                        }
                    }

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill].Contains(" "))
                        {
                            if (_Pets.ContainsKey(_Dots[skill]))
                            {
                                if (PlayerInflictedSkillDamage != null)
                                {
                                    PlayerInflictedSkillDamage(this,
                                                         new SkillDamageEventArgs(time, _Pets[_Dots[skill]], name,
                                                                                        damage, skill));
                                }
                            }
                        }

                        else
                        {
                            if (PlayerInflictedSkillDamage != null)
                            {
                                PlayerInflictedSkillDamage(this,
                                                     new SkillDamageEventArgs(time, _Dots[skill], name, damage, skill));
                            }
                        }
                    }

                    if (_Effects.ContainsKey(skill))
                    {
                        if (PlayerInflictedSkillDamage != null)
                        {
                            PlayerInflictedSkillDamage(this,
                                                 new SkillDamageEventArgs(time, _Effects[skill], name, damage, skill));
                        }
                    }

                    if (_Effects.ContainsKey(skill.Replace(Localization.Regex.Effect, "")))
                    {
                        if (PlayerInflictedSkillDamage != null)
                        {
                            PlayerInflictedSkillDamage(this,
                                                 new SkillDamageEventArgs(time,
                                                                                _Effects[
                                                                                    skill.Replace(
                                                                                        Localization.Regex.Effect, "")], name,
                                                                                damage, skill));
                        }
                    }

                    matched = true;
                    regex = "_OtherReceivedSkillRegex";
                    return;
                }

                matches = _OtherReceivedBleedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                    string skill = matches[0].Groups[_SkillGroupName].Value;

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill].Contains(" "))
                        {
                            if (_Pets.ContainsKey(_Dots[skill]))
                            {
                                if (PlayerInflictedSkillDamage != null)
                                {
                                    PlayerInflictedSkillDamage(this,
                                                         new SkillDamageEventArgs(time, _Pets[_Dots[skill]], name,
                                                                                        damage, skill));
                                }
                            }
                        }

                        else
                        {
                            if (PlayerInflictedSkillDamage != null)
                            {
                                PlayerInflictedSkillDamage(this,
                                                     new SkillDamageEventArgs(time, _Dots[skill], name, damage, skill));
                            }
                        }
                    }

                    matched = true;
                    regex = "_OtherReceivedBleedRegex";
                    return;
                }

                matches = _OtherContinuousRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill] != name)
                        {
                            _Dots[skill] = name;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, name);
                    }

                    matched = true;
                    regex = "_OtherContinuousRegex";
                    return;
                }

                matches = _OtherInflictedBleedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (_Dots.ContainsKey(skill))
                    {
                        if (_Dots[skill] != name)
                        {
                            _Dots[skill] = name;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, name);
                    }

                    matched = true;
                    regex = "_OtherInflictedBleedRegex";
                    return;
                }

                matches = _OtherSummonedAttackRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    string target = matches[0].Groups[_TargetGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string pet = matches[0].Groups[_PetGroupName].Value;

                    if (_Pets.ContainsKey(pet))
                    {
                        if (_Pets[pet] != name)
                        {
                            _Pets[pet] = name;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, name);
                    }

                    matched = true;
                    regex = "_OtherSummonedAttackRegex";
                    return;
                }

                matches = _OtherSummonedRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string name = matches[0].Groups[_NameGroupName].Value;
                    string skill = matches[0].Groups[_SkillGroupName].Value;
                    string pet = matches[0].Groups[_PetGroupName].Value;

                    if (_Pets.ContainsKey(pet))
                    {
                        if (_Pets[pet] != name)
                        {
                            _Pets[pet] = name;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, name);
                    }

                    matched = true;
                    regex = "_OtherSummonedRegex";
                    return;
                }

                matches = _JoinedGroupRegex.Matches(line);
                if (matches.Count > 0)
                {
                    string name = matches[0].Groups[_NameGroupName].Value;
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                    if (PlayerJoinedGroup != null)
                    {
                        PlayerJoinedGroup(this, new PlayerEventArgs(time, name));
                    }

                    matched = true;
                    regex = "_JoinedGroupRegex";
                    return;
                }

                matches = _LeftGroupRegex.Matches(line);
                if (matches.Count > 0)
                {
                    string name = matches[0].Groups[_NameGroupName].Value;
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                    if (PlayerLeftGroup != null)
                    {
                        PlayerLeftGroup(this, new PlayerEventArgs(time, name));
                    }

                    matched = true;
                    regex = "_LeftGroupRegex";
                    return;
                }

                matches = _KickedFromGroupRegex.Matches(line);
                if (matches.Count > 0)
                {
                    string name = matches[0].Groups[_NameGroupName].Value;
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                    if (PlayerLeftGroup != null)
                    {
                        PlayerLeftGroup(this, new PlayerEventArgs(time, name));
                    }

                    matched = true;
                    regex = "_KickedFromGroupRegex";
                    return;
                }

                matches = _YouGainedExpRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int exp = matches[0].Groups[_ExpGroupName].Value.GetDigits();
                    string target = matches[0].Groups[_TargetGroupName].Value;

                    if (ExpGained != null)
                    {
                        ExpGained(this, new ExpEventArgs(time, exp));
                    }

                    matched = true;
                    regex = "_YouGainedExpRegex";
                    return;
                }

                matches = _YouEarnedKinahRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int kinah = matches[0].Groups[_KinahGroupName].Value.GetDigits();

                    if (KinahEarned != null)
                    {
                        KinahEarned(this, new KinahEventArgs(time, kinah));
                    }

                    matched = true;
                    regex = "_YouEarnedKinahRegex";
                    return;
                }

                matches = _YouSpentKinahRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int kinah = matches[0].Groups[_KinahGroupName].Value.GetDigits();

                    if (KinahSpent != null)
                    {
                        KinahSpent(this, new KinahEventArgs(time, kinah));
                    }

                    matched = true;
                    regex = "_YouSpentKinahRegex";
                    return;
                }

                matches = _YouGainedApRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    int ap = matches[0].Groups[_ApGroupName].Value.GetDigits();

                    if (AbyssPointsGained != null)
                    {
                        AbyssPointsGained(this, new AbyssPointsEventArgs(time, ap));
                    }

                    matched = true;
                    regex = "_YouGainedApRegex";
                    return;
                }

                matches = _YouHaveJoinedChannelRegex.Matches(line);
                if (matches.Count > 0)
                {
                    DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                    string regionName = matches[0].Groups[_RegionGroupName].Value;

                    if (RegionChanged != null)
                    {
                        RegionChanged(this, new RegionEventArgs(time, regionName));
                    }

                    matched = true;
                    regex = "_YouHaveJoinedChannelRegex";
                    return;
                }
            }
            finally
            {
                if (!matched)
                {
                    DebugLogger.Write("No match for: (\"" + line + "\")");
                }

                else
                {
                    DebugLogger.Write(regex + ": (\"" + line + "\")");
                }
            }
        }
    }
}
