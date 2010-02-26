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
        }

        public void Populate(SkillCollection skills, int damage)
        {
            foreach (string skill in skills.Keys)
            {
                ListViewItem item;
                string[] info = new string[3];
                info[0] = skill;
                info[1] = skills.Get(skill).ToString();
                info[2] = GetPercent(skills.Get(skill), damage).ToString("0.0%");
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
