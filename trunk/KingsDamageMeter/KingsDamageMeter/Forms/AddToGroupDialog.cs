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
using System.Windows.Forms;
using KingsDamageMeter.Localization;
using Regex=System.Text.RegularExpressions.Regex;

namespace KingsDamageMeter.Forms
{
    public partial class AddToGroupDialog : Form
    {
        public AddToGroupDialog()
        {
            InitializeComponent();

            Text = AddToGroupRes.WindowTitle;
            OKButton.Text = AddToGroupRes.OKButton;
            CancelButton.Text = AddToGroupRes.CancelButton;
            _InvlaidNameMessage = AddToGroupRes.InvalidNameMessage;
        }

        private Regex _NameRegex = new Regex(@"^[a-zA-Z]+$");
        private string _PlayerName;
        private string _InvlaidNameMessage = String.Empty;

        public string PlayerName
        {
            get
            {
                return _PlayerName;
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Finish();
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Finish()
        {
            string name = InputTextBox.Text;

            MatchCollection matches = _NameRegex.Matches(name);
            if (matches.Count > 0)
            {
                _PlayerName = name;
                DialogResult = DialogResult.OK;
            }

            else
            {
                MessageBox.Show(_InvlaidNameMessage, "Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
