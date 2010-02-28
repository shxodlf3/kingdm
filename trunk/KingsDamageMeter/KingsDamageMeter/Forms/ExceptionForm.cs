using System;
using System.Windows.Forms;

namespace KingsDamageMeter.Forms
{
    public partial class ExceptionForm : Form
    {
        public string Exception
        {
            get
            {
                return TextException.Text;
            }

            set
            {
                TextException.Text = value;
            }
        }

        public ExceptionForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
