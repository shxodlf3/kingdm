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
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.ComponentModel;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A class that represents a player control.
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        /// <summary>
        /// A class that represents a player control.
        /// </summary>
        public PlayerControl()
        {
            InitializeComponent();
            SetStyle();
            InitializeTimers();

            DamageChanged += OnDamageChanged;
            DamagePercentChanged += OnDamagePercentChanged;
            GroupMemberChanged += OnGroupMemberChanged;
        }

        public void Reset()
        {
            Damage = 0;
            DamageTaken = 0;
            DamagePercent = 0;
            _DamagePerSecond = 0;
            _SecondsSinceStart = 0;
            ResetTimers();
        }

        private int _Damage = 0;
        private int _DamageTaken = 0;
        private double _DamagePercent = 0;

        private System.Windows.Forms.Timer _DamageTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer _DurationTimer = new System.Windows.Forms.Timer();
        private DateTime _TimeSinceDamage = DateTime.Now;
        private int _PeakDps = 0;
        private int _DamagePerSecond = 0;
        private int _SecondsSinceStart = 0;
        private int _BiggestHit = 0;
        private bool _GroupMember = false;

        public event EventHandler DamageChanged;
        public event EventHandler DamageTakenChanged;
        public event EventHandler DamagePercentChanged;
        public event EventHandler GroupMemberChanged;

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return LabelName.Content.ToString();
            }

            set
            {
                if (value == "You")
                {
                    LabelName.FontWeight = FontWeights.Bold;
                }

                LabelName.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage the player has dealt.
        /// </summary>
        public int Damage
        {
            get
            {
                return _Damage;
            }

            set
            {
                if (value > _Damage)
                {
                    int amount = value - _Damage;

                    if (amount > _BiggestHit)
                    {
                        _BiggestHit = amount;
                    }
                }

                _Damage = value;

                if (DamageChanged != null)
                {
                    DamageChanged(this, EventArgs.Empty);
                }
            }
        }

        public int DamageTaken
        {
            get
            {
                return _DamageTaken;
            }

            set
            {
                if (value < 0) value = 0;
                _DamageTaken = value;

                if (DamageTakenChanged != null)
                {
                    DamageTakenChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the percent value or the damage the player has dealt compared to other players.
        /// </summary>
        public double DamagePercent
        {
            get
            {
                return _DamagePercent;
            }

            set
            {
                if (value < 0) value = 0;
                if (value > 1) value = 1;
                _DamagePercent = value;

                if (DamagePercentChanged != null)
                {
                    DamagePercentChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the damage per second.
        /// </summary>
        public int DamagePerSecond
        {
            get
            {
                return _DamagePerSecond;
            }
        }

        /// <summary>
        /// Gets the peak damage per second.
        /// </summary>
        public int PeakDps
        {
            get
            {
                return _PeakDps;
            }
        }

        public bool GroupMember
        {
            get
            {
                return _GroupMember;
            }

            set
            {
                _GroupMember = value;

                if (GroupMemberChanged != null)
                {
                    GroupMemberChanged(this, EventArgs.Empty);
                }
            }
        }

        public new string ToString()
        {
            return String.Format("({0} {1}, {2} dps ({3}))", PlayerName, Damage, _DamagePerSecond, DamagePercent.ToString("0%"));
        }

        private void InitializeTimers()
        {
            if (_DamageTimer.Enabled)
            {
                _DamageTimer.Enabled = false;
            }

            _DamageTimer.Interval = 10;
            _DamageTimer.Tick += _DamageTimer_Tick;
            _DamageTimer.Start();

            if (_DurationTimer.Enabled)
            {
                _DurationTimer.Enabled = false;
            }

            _DurationTimer.Interval = 1000;
            _DurationTimer.Tick += _DurationTimer_Tick;
            //_DurationTimer.Start();
        }

        private void ResetTimers()
        {
            if (_DurationTimer.Enabled)
            {
                _DurationTimer.Enabled = false;
            }
        }

        private void SetStyle()
        {
            if (_GroupMember)
            {
                Style = (Style)FindResource("GroupControlTemplate");
            }

            else
            {
                Style = (Style)FindResource("PlayerControlTemplate");
            }
        }

        private void UpdateToolTip()
        {
            string message = String.Empty;

            message += PlayerName;
            message += Environment.NewLine;
            message += _Damage + " " + KingsDamageMeter.Languages.Gui.Default.PlayerToolTipTotal;
            message += Environment.NewLine;
            message += _DamagePercent.ToString("0%");
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += _DamagePerSecond + " " + KingsDamageMeter.Languages.Gui.Default.PlayerToolTipDps;
            message += Environment.NewLine;
            message += _PeakDps + " " + KingsDamageMeter.Languages.Gui.Default.PlayerToolTipPeak;
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += _BiggestHit + " " + KingsDamageMeter.Languages.Gui.Default.PlayerToolTipBiggestHit;

            ToolTip = message;
        }

        private void _DamageTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - _TimeSinceDamage;

            if (span.TotalMilliseconds >= 1100)
            {
                if (_DurationTimer.Enabled)
                {
                    _DurationTimer.Stop();
                }
            }

            else
            {
                if (!_DurationTimer.Enabled)
                {
                    _DurationTimer.Start();
                }
            }
        }

        private void _DurationTimer_Tick(object sender, EventArgs e)
        {
            if (_SecondsSinceStart > 3)
            {
                _DamagePerSecond = _Damage / _SecondsSinceStart;

                if (_DamagePerSecond > _PeakDps)
                {
                    _PeakDps = _DamagePerSecond;
                }
            }

            _SecondsSinceStart += 1;
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateToolTip();
        }

        protected void OnDamageChanged(object sender, EventArgs e)
        {
            _TimeSinceDamage = DateTime.Now;
            LabelDamage.Content = _Damage.ToString();
        }

        protected void OnDamagePercentChanged(object sender, EventArgs e)
        {
            if (_DamagePercent > 0)
            {
                DamageBar.Value = _DamagePercent * 100;
            }

            else
            {
                DamageBar.Value = 0;
            }
        }

        protected void OnGroupMemberChanged(object sender, EventArgs e)
        {
            SetStyle();
        }
    }
}
