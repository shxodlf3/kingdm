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
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
        }

        private bool _Running = false;
        private FileStream _FileStream;
        private StreamReader _StreamReader;

        private string _TimeFormat = KingsDamageMeter.Properties.Settings.Default.LogTimeFormat;

        private Dictionary<string, string> _Dots = new Dictionary<string, string>();
        private Dictionary<string, string> _Pets = new Dictionary<string,string>();

        private Thread _Worker;
        private object _LockObject = new object();

        private string _YouAlias;

        private string _LogPath = String.Empty;

        private string _NameGroupName = "name";
        private string _DamageGroupName = "damage";
        private string _SkillGroupName = "skill";
        private string _PetGroupName = "pet";
        private string _TimeGroupName = "time";
        private string _TargetGroupName = "target";
        private string _EffectGroupName = "effect";

        private string _TimestampRegex;
        private Regex _ChatRegex;
        private Regex _YouInflictedRegex;
        private Regex _YouInflictedSkillRegex;
        private Regex _YouCriticalRegex;
        private Regex _YouEffectDamageRegex;
        private Regex _OtherInflictedRegex;
        private Regex _OtherInflictedSkillRegex;
        private Regex _YouReceivedRegex;
        private Regex _OtherReceivedRegex;
        private Regex _OtherReceivedSkillRegex;
        private Regex _YouContinuousRegex;
        private Regex _OtherContinuousRegex;
        private Regex _YouSummonedRegex;
        private Regex _YouSummonedAttackRegex;
        private Regex _OtherSummonedRegex;
        private Regex _OtherSummonedAttackRegex;
        private Regex _JoinedGroupRegex;
        private Regex _LeftGroupRegex;

        /// <summary>
        /// Occurs when the parser is starting.
        /// </summary>
        public event EventHandler Starting;

        /// <summary>
        /// Occurs when the parser has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when the parser is stopping.
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Occurs when the parser has stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when KingsDamageMeter.AionLogParser is unable to find the specified log file.
        /// </summary>
        public event EventHandler FileNotFound;

        /// <summary>
        /// Occurs when the parser finds damage inflicted.
        /// </summary>
        public event DamageInflictedEventHandler DamageInflicted;

        /// <summary>
        /// Occurs when the parser finds critical damage inflicted.
        /// </summary>
        public event DamageInflictedEventHandler CriticalInflicted;

        /// <summary>
        /// Occurs when a player joins the group.
        /// </summary>
        public event PlayerEventHandler PlayerJoinedGroup;

        /// <summary>
        /// Occurs when a player leaved the group.
        /// </summary>
        public event PlayerEventHandler PlayerLeftGroup;

        /// <summary>
        /// Occurs when a player receives damage.
        /// </summary>
        public event DamageInflictedEventHandler DamageReceived;

        private void Initialize()
        {
            _YouAlias = KingsDamageMeter.Languages.Regex.Default.YouAlias;

            _TimestampRegex = @KingsDamageMeter.Languages.Regex.Default.TimestampRegex;
            _ChatRegex = new Regex(@KingsDamageMeter.Languages.Regex.Default.Chat, RegexOptions.Compiled);
            _YouInflictedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouInflictedRegex, RegexOptions.Compiled);
            _YouInflictedSkillRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouInflictedSkillRegex, RegexOptions.Compiled);
            _YouCriticalRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouCriticalRegex, RegexOptions.Compiled);
            _YouEffectDamageRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouEffectDamageRegex, RegexOptions.Compiled);
            _OtherInflictedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherInflictedRegex, RegexOptions.Compiled);
            _OtherInflictedSkillRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherInflictedSkillRegex, RegexOptions.Compiled);
            _YouReceivedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouReceivedRegex, RegexOptions.Compiled);
            _OtherReceivedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherReceivedRegex, RegexOptions.Compiled);
            _OtherReceivedSkillRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherReceivedSkillRegex, RegexOptions.Compiled);
            _YouContinuousRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouContinuousRegex, RegexOptions.Compiled);
            _OtherContinuousRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherContinuousRegex, RegexOptions.Compiled);
            _YouSummonedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouSummonedRegex, RegexOptions.Compiled);
            _YouSummonedAttackRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.YouSummonedAttackRegex, RegexOptions.Compiled);
            _OtherSummonedRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherSummonedRegex, RegexOptions.Compiled);
            _OtherSummonedAttackRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.OtherSummonedAttackRegex, RegexOptions.Compiled);
            _JoinedGroupRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.JoinedGroupRegex, RegexOptions.Compiled);
            _LeftGroupRegex = new Regex(_TimestampRegex + @KingsDamageMeter.Languages.Regex.Default.LeftGroupRegex, RegexOptions.Compiled);
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
                _StreamReader = GetStreamReader(_FileStream);
                _StreamReader.ReadToEnd(); // Skip the stuff in the file from the last session.
                StartWorker();
            }

            if (Started != null)
            {
                Started(this, EventArgs.Empty);
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

            matches = _YouInflictedRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;

                if (DamageInflicted != null)
                {
                    DamageInflicted(this, new PlayerDamageEventArgs(time, _YouAlias, damage));
                }

                matched = true;
                goto End;
            }

            matches = _YouInflictedSkillRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;
                string skill = matches[0].Groups[_SkillGroupName].Value;

                if (DamageInflicted != null)
                {
                    DamageInflicted(this, new PlayerDamageEventArgs(time, _YouAlias, damage));
                }

                matched = true;
                goto End;
            }

            matches = _YouCriticalRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;

                if (CriticalInflicted != null)
                {
                    CriticalInflicted(this, new PlayerDamageEventArgs(time, _YouAlias, damage));
                }

                matched = true;
                goto End;
            }

            matches = _YouEffectDamageRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;
                string effect = matches[0].Groups[_EffectGroupName].Value;

                if (DamageInflicted != null)
                {
                    DamageInflicted(this, new PlayerDamageEventArgs(time, _YouAlias, damage));
                }

                matched = true;
                goto End;
            }

            matches = _YouReceivedRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;

                if (DamageReceived != null)
                {
                    DamageReceived(this, new PlayerDamageEventArgs(time, target, damage));
                }

                matched = true;
                goto End;
            }

            matches = _YouContinuousRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                string target = matches[0].Groups[_TargetGroupName].Value;
                string skill = matches[0].Groups[_SkillGroupName].Value;
                
                if (_Dots.ContainsKey(skill))
                {
                    if (_Dots[skill] != _YouAlias)
                    {
                        _Dots[skill] = _YouAlias;
                    }
                }

                else
                {
                    _Dots.Add(skill, _YouAlias);
                }

                matched = true;
                goto End;
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
                    if (_Pets[pet] != _YouAlias)
                    {
                        _Pets[pet] = _YouAlias;
                    }
                }

                else
                {
                    _Pets.Add(pet, _YouAlias);
                }

                matched = true;
                goto End;
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
                    if (_Pets[pet] != _YouAlias)
                    {
                        _Pets[pet] = _YouAlias;
                    }
                }

                else
                {
                    _Pets.Add(pet, _YouAlias);
                }

                matched = true;
                goto End;
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
                        name = _Pets[name];

                        if (DamageInflicted != null)
                        {
                            DamageInflicted(this, new PlayerDamageEventArgs(time, name, damage));
                        }
                    }
                }

                else
                {
                    if (DamageInflicted != null)
                    {
                        DamageInflicted(this, new PlayerDamageEventArgs(time, name, damage));
                    }
                }

                matched = true;
                goto End;
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

                        if (DamageInflicted != null)
                        {
                            DamageInflicted(this, new PlayerDamageEventArgs(time, name, damage));
                        }
                    }
                }

                else
                {
                    if (DamageInflicted != null)
                    {
                        DamageInflicted(this, new PlayerDamageEventArgs(time, name, damage));
                    }
                }

                matched = true;
                goto End;
            }

            matches = _OtherReceivedRegex.Matches(line);
            if (matches.Count > 0)
            {
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                string name = matches[0].Groups[_NameGroupName].Value;
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string target = matches[0].Groups[_TargetGroupName].Value;

                matched = true;
                goto End;
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
                            if (DamageInflicted != null)
                            {
                                DamageInflicted(this, new PlayerDamageEventArgs(time, _Pets[_Dots[skill]], damage));
                            }
                        }
                    }

                    else
                    {
                        if (DamageInflicted != null)
                        {
                            DamageInflicted(this, new PlayerDamageEventArgs(time, _Dots[skill], damage));
                        }
                    }
                }

                matched = true;
                goto End;
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
                goto End;
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
                goto End;
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
                goto End;
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
                goto End;
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
                goto End;
            }

            End:

            if (!matched)
            {
                DebugLogger.Write("No match for: (\"" + line + "\")");
            }

            else
            {
                DebugLogger.Write("Matched: (\"" + line + "\")");
            }
        }
    }
}
