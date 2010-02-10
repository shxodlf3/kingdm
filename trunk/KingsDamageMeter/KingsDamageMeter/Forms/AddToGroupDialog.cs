using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KingsDamageMeter.Forms
{
    public partial class AddToGroupDialog : Form
    {
        public AddToGroupDialog()
        {
            InitializeComponent();

            Text = KingsDamageMeter.Languages.En.Default.AddGroupMemberTitle;
            OKButton.Text = KingsDamageMeter.Languages.En.Default.OKButton;
            CancelButton.Text = KingsDamageMeter.Languages.En.Default.CancelButton;
            _InvlaidNameMessage = KingsDamageMeter.Languages.En.Default.InvalidNameMessage;
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
