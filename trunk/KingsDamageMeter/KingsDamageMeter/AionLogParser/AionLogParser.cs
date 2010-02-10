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
        private bool _Running = false;
        private FileStream _FileStream;
        private StreamReader _StreamReader;

        private string _TimeFormat = KingsDamageMeter.Properties.Settings.Default.LogTimeFormat;

        private Dictionary<string, string> _Dots = new Dictionary<string, string>();
        private Dictionary<string, string> _Pets = new Dictionary<string,string>();

        private Thread _Worker;
        private object _LockObject = new object();

        private string _NameGroupName = "name";
        private string _DamageGroupName = "damage";
        private string _SkillGroupName = "skill";
        private string _PetGroupName = "pet";
        private string _NpcGroupName = "npc";
        private string _TimeGroupName = "time";

        private Regex _DamageInflictedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) inflicted (?<damage>[^a-zA-Z]+) damage", RegexOptions.Compiled);
        private Regex _CriticalInflictedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : Critical Hit! (?<name>[a-zA-Z]+) inflicted (?<damage>[^a-zA-Z]+) critical damage", RegexOptions.Compiled);
        private Regex _ContinuousInflictedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) inflicted continuous damage on (?<target>.+) by using (?<skill>.+)\.", RegexOptions.Compiled);
        private Regex _SkillDamageInflictedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<target>.+) received (?<damage>[^a-zA-Z]+) damage due to the effect of (?<skill>.+)\.", RegexOptions.Compiled);
        private Regex _JoinedGroupRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) has joined your group\.", RegexOptions.Compiled);
        private Regex _LeftGroupRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) has left your group\.", RegexOptions.Compiled);
        private Regex _PetSummonedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) summoned (?<pet>(([a-zA-Z]+)\s)+)by using", RegexOptions.Compiled);
        private Regex _NpcInflictedDamageRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<npc>(([a-zA-Z]+)\s)+)inflicted (?<damage>[^a-zA-Z]+) damage", RegexOptions.Compiled);
        private Regex _DamageReceivedRegex = new Regex(@"(?<time>[^a-zA-Z]+) : (?<name>[a-zA-Z]+) received (?<damage>[^a-zA-Z]+) damage", RegexOptions.Compiled);
        private Regex _ChatRegex = new Regex(@"\[charname:", RegexOptions.Compiled);

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

            if ((_FileStream = OpenFileStream(file)) != null)
            {
                _Running = true;
                _StreamReader = GetStreamReader(_FileStream);
                _StreamReader.ReadToEnd(); // Skip the stuff in the file from the last session.
                StartWorker();
            }
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
        }

        /// <summary>
        /// Open a System.IO.FileStream for the specified file with the FileMode Open, FileAccess Read and FileShare ReadWrite
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private FileStream OpenFileStream(string file)
        {
            FileStream stream = null;

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
                return new StreamReader(stream);
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

        /// <summary>
        /// Parse a line a text.
        /// </summary>
        /// <param name="line">The line read from the file.</param>
        private void ParseLine(string line)
        {
            MatchCollection matches;

            // Ignore chat text. Put here to avoid matching stuff people may type in chat. (Unlikely)
            matches = _ChatRegex.Matches(line);
            if (matches.Count > 0)
            {
                return;
            }

            // Match normal damage being inflicted..
            matches = _DamageInflictedRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);
                
                if (DamageInflicted != null)
                {
                    DamageInflicted(this, new DamageEventArgs(time, name, damage));
                }

                return;
            }

            // Match critical damage inflicted.
            matches = _CriticalInflictedRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (CriticalInflicted != null)
                {
                    CriticalInflicted(this, new DamageEventArgs(time, name, damage));
                }

                return;
            }

            // Match damage received by a player.
            matches = _DamageReceivedRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (DamageReceived != null)
                {
                    DamageReceived(this, new DamageEventArgs(time, name, damage));
                }

                return;
            }

            // Match player joining group.
            matches = _JoinedGroupRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (PlayerJoinedGroup != null)
                {
                    PlayerJoinedGroup(this, new PlayerEventArgs(time, name));
                }

                return;
            }

            // Match player leaving group.
            matches = _LeftGroupRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (PlayerLeftGroup != null)
                {
                    PlayerLeftGroup(this, new PlayerEventArgs(time, name));
                }

                return;
            }

            // Match continuous damage inflicted and keep in dictionary which player gets credit for the damage done by which skill.
            // The same DoT from two different players do not stack in Aion (currently)
            matches = _ContinuousInflictedRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
                string skill = matches[0].Groups[_SkillGroupName].Value;

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

                return;
            }

            // Match damage inflicted by a skill and get attacking player from dictionary.
            matches = _SkillDamageInflictedRegex.Matches(line);
            if (matches.Count > 0)
            {
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                string skill = matches[0].Groups[_SkillGroupName].Value;
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (_Dots.ContainsKey(skill))
                {
                    string name = _Dots[skill];

                    if (DamageInflicted != null)
                    {
                        DamageInflicted(this, new DamageEventArgs(time, name, damage));
                    }
                }

                return;
            }

            // Match a summoned pet.
            matches = _PetSummonedRegex.Matches(line);
            if (matches.Count > 0)
            {
                string name = matches[0].Groups[_NameGroupName].Value;
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
            }

            // Matches npc damage. Checked against pets that have been summoned (_Pets). This will match ALL npc damage.
            matches = _NpcInflictedDamageRegex.Matches(line);
            if (matches.Count > 0)
            {
                string npc = matches[0].Groups[_NpcGroupName].Value;
                int damage = matches[0].Groups[_DamageGroupName].Value.GetDigits();
                DateTime time = matches[0].Groups[_TimeGroupName].Value.GetTime(_TimeFormat);

                if (_Pets.ContainsKey(npc))
                {
                    string name = _Pets[npc];

                    if (DamageInflicted != null)
                    {
                        DamageInflicted(this, new DamageEventArgs(time, name, damage));
                    }
                }
            }
        }
    }
}
