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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.ComponentModel;
using KingsDamageMeter.Forms;
using KingsDamageMeter.Localization;

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
        }

        private void MenuItemViewSkills_Click(object sender, RoutedEventArgs e)
        {
            SkillsForm s = new SkillsForm();
            s.Text = string.Format(SkillsFormRes.WindowTitle, ((Player) DataContext).PlayerName);
            s.Populate(((Player)DataContext).Skills, ((Player)DataContext).Damage);
            s.ShowDialog();
        }
    }
}
