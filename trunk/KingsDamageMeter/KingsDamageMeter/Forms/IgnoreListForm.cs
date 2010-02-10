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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KingsDamageMeter.Forms
{
    /// <summary>
    /// A class that represents an ignore list form.
    /// </summary>
    public partial class IgnoreListForm : Form
    {
        /// <summary>
        /// A class that represents an ignore list form.
        /// </summary>
        public IgnoreListForm()
        {
            InitializeComponent();
            PopulateIgnoreList();
        }

        private void PopulateIgnoreList()
        {
            if (KingsDamageMeter.Properties.Settings.Default.IgnoreList != null)
            {
                foreach (string name in KingsDamageMeter.Properties.Settings.Default.IgnoreList)
                {
                    if (!String.IsNullOrEmpty(name))
                    {
                        ListIgnored.Items.Add(name);
                    }
                }
            }
        }

        private void MenuPlayersRemove_Click(object sender, EventArgs e)
        {
            if (ListIgnored.Items.Count > 0)
            {
                if (ListIgnored.SelectedItem == null)
                {
                    return;
                }

                KingsDamageMeter.Properties.Settings.Default.IgnoreList.Remove(ListIgnored.SelectedItem.ToString());
                ListIgnored.Items.Remove(ListIgnored.SelectedItem);
            }
        }

        private void ButtonFind_Click(object sender, EventArgs e)
        {
            SearchList();
        }

        private void TextPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SearchList();
            }
        }

        private void SearchList()
        {
            int index = ListIgnored.FindString(TextPlayer.Text);

            if (index > -1 && index <= ListIgnored.Items.Count)
            {
                ListIgnored.SelectedIndex = index;
            }
        }
    }
}
