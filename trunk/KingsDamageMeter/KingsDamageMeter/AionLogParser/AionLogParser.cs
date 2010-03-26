﻿/**************************************************************************\
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
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter
{
    /// <summary>
    /// 
    /// </summary>
    public class AionLogParser : IAionLogParser
    {
        public AionLogParser()
        {
            Initialize();
            _OldYouAlias = Settings.Default.YouAlias;
            Settings.Default.PropertyChanged += SettingsChanged;
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "YouAlias")
            {
                //Here we change name of old YouAlias
                foreach (var pet in _Pets)
                {
                    if (pet.Value == _OldYouAlias)
                    {
                        _Pets[pet.Key] = Settings.Default.YouAlias;
                        _OldYouAlias = Settings.Default.YouAlias;
                        break;
                    }
                }
            }
        }

        private bool _Running = false;
        private FileStream _FileStream;
        private StreamReader _StreamReader;

        private string _TimeFormat = KingsDamageMeter.Properties.Settings.Default.LogTimeFormat;

        private Dictionary<string, string> _Dots = new Dictionary<string, string>();
        private Dictionary<string, string> _Pets = new Dictionary<string, string>();
        private Dictionary<string, string> _Effects = new Dictionary<string, string>();

        private Thread _Worker;
        private object _LockObject = new object();

        private string _OldYouAlias;

        private string _LogPath = String.Empty;

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

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;

        public event EventHandler FileNotFound;

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

        public void Initialize()
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
        }

        /// <summary>
        /// Gets the running status of the parser.
        /// </summary>
        public bool Running
        {
            get
            {
                return _Running;
            }
        }

        /// <summary>
        /// Start the parser.
        /// </summary>
        public void Start(string file)
        {
            if (_Running)
            {
                return;
            }

            if (Starting != null)
            {
                Starting(this, EventArgs.Empty);
            }

            if ((_FileStream = OpenFileStream(file)) != null)
            {
                _Running = true;
                _FileStream.Position = _FileStream.Length;
                _StreamReader = GetStreamReader(_FileStream);
                StartWorker();

                if (Started != null)
                {
                    Started(this, EventArgs.Empty);
                }
            }
            else
            {
                if (Stopped != null)
                {
                    Stopped(this, EventArgs.Empty);
                }
            }

            DebugLogger.Write("Log parser initialized: \"" + _LogPath + "\"");
        }

        /// <summary>
        /// Stop the parser.
        /// </summary>
        public void Stop()
        {
            if (!_Running)
            {
                return;
            }

            else
            {
                _Running = false;
            }

            if (Stopping != null)
            {
                Stopping(this, EventArgs.Empty);
            }

            // Working out how to avoid Abort()
            if (_Worker != null)
            {
                _Worker.Abort();
                _Worker = null;
            }

            if (_StreamReader != null)
            {
                _StreamReader.Close();
                _StreamReader = null;
            }

            if (_FileStream != null)
            {
                _FileStream.Close();
                _FileStream = null;
            }

            if (Stopped != null)
            {
                Stopped(this, EventArgs.Empty);
            }

            DebugLogger.Write("Log parser stopped.");
        }

        /// <summary>
        /// Open a System.IO.FileStream for the specified file with the FileMode Open, FileAccess Read and FileShare ReadWrite
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private FileStream OpenFileStream(string file)
        {
            FileStream stream = null;
            _LogPath = file;

            try
            {
                stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    if (FileNotFound != null)
                    {
                        FileNotFound(this, EventArgs.Empty);
                    }
                }

                DebugLogger.Write("Error opening Chat.Log: " + Environment.NewLine + e.Message);
            }

            return stream;
        }

        /// <summary>
        /// Initialize a new instance of System.IO.StreamReader for the specified stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <returns></returns>
        private StreamReader GetStreamReader(FileStream stream)
        {
            if (stream != null)
            {
                return new StreamReader(stream, System.Text.Encoding.Default);
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Start the parser's worker thread.
        /// </summary>
        private void StartWorker()
        {
            _Worker = new Thread
            (
                delegate()
                {
                    lock (_LockObject)
                    {
                        while (_Running)
                        {
                            string line = _StreamReader.ReadLine();

                            if (!String.IsNullOrEmpty(line))
                            {
                                ParseLine(line);
                            }

                            // Oops yah, it was eating up cpu without this.
                            Thread.Sleep(1);
                        }
                    }
                }
            );

            _Worker.IsBackground = true;
            _Worker.Start();
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
                                             new SkillDamageEventArgs(time, Settings.Default.YouAlias, target, damage,
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
                        PlayerInflictedDamage(this, new DamageEventArgs(time, Settings.Default.YouAlias, target, damage));
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
                        PlayerInflictedCriticalDamage(this, new DamageEventArgs(time, Settings.Default.YouAlias, target, damage));
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
                                             new SkillDamageEventArgs(time, Settings.Default.YouAlias, target, damage,
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
                        if (_Effects[effect] != Settings.Default.YouAlias)
                        {
                            _Effects[effect] = Settings.Default.YouAlias;
                        }
                    }

                    else
                    {
                        _Effects.Add(effect, Settings.Default.YouAlias);
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
                        PlayerReceivedDamage(this, new DamageEventArgs(time, target, Settings.Default.YouAlias, damage));
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
                        if (_Dots[skill] != Settings.Default.YouAlias)
                        {
                            _Dots[skill] = Settings.Default.YouAlias;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, Settings.Default.YouAlias);
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
                        if (_Dots[skill] != Settings.Default.YouAlias)
                        {
                            _Dots[skill] = Settings.Default.YouAlias;
                        }
                    }

                    else
                    {
                        _Dots.Add(skill, Settings.Default.YouAlias);
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
                        if (_Pets[pet] != Settings.Default.YouAlias)
                        {
                            _Pets[pet] = Settings.Default.YouAlias;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, Settings.Default.YouAlias);
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
                        if (_Pets[pet] != Settings.Default.YouAlias)
                        {
                            _Pets[pet] = Settings.Default.YouAlias;
                        }
                    }

                    else
                    {
                        _Pets.Add(pet, Settings.Default.YouAlias);
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
