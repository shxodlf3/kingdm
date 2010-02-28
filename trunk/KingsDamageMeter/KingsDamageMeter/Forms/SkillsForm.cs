using System;
using System.Drawing;
using System.Windows.Forms;

using KingsDamageMeter.Combat;

namespace KingsDamageMeter.Forms
{
    public partial class SkillsForm : Form
    {
        private SkillSorter _SkillSorter = new SkillSorter();

        public SkillsForm()
        {
            InitializeComponent();
            SkillList.ListViewItemSorter = _SkillSorter;

            columnHeader1.Text = KingsDamageMeter.Languages.Gui.Default.SkillColumn;
            columnHeader2.Text = KingsDamageMeter.Languages.Gui.Default.DamageColumn;
            columnHeader3.Text = KingsDamageMeter.Languages.Gui.Default.PercentColumn;
            columnHeader4.Text = KingsDamageMeter.Languages.Gui.Default.HitsColumn;
        }

        public void Populate(SkillCollection skills, int damage)
        {
            foreach (string skill in skills.Keys)
            {
                ListViewItem item;
                string[] info = new string[4];
                info[0] = skill;
                info[1] = skills.Get(skill).Damage.ToString("#,#");
                info[2] = GetPercent(skills.Get(skill).Damage, damage).ToString("0.0%");
                info[3] = skills.Get(skill).Uses.ToString();
                item = new ListViewItem(info);
                SkillList.Items.Add(item);
            }
        }

        private double GetPercent(int damage, int total)
        {
            double percent;

            try
            {
                percent = (double)((double)(damage - total) / total) + 1;
            }

            catch
            {
                percent = 0;
            }

            return percent;
        }

        private void SkillList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _SkillSorter.SortColumn)
            {
                if (_SkillSorter.SortOrder == SortOrder.Ascending)
                {
                    _SkillSorter.SortOrder = SortOrder.Descending;
                }
                else
                {
                    _SkillSorter.SortOrder = SortOrder.Ascending;
                }
            }

            else
            {
                _SkillSorter.SortColumn = e.Column;
                _SkillSorter.SortOrder = SortOrder.Ascending;
            }
            
            SkillList.Sort();
        }
    }
}
