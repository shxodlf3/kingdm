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

        public void Populate(SkillCollection skills, long damage)
        {
            foreach (string skill in skills.Keys)
            {
                ListViewItem item;
                string[] info = new string[4];
                info[0] = skill;
                info[1] = skills.Get(skill).DamageFormatted;
                info[2] = GetPercent(skills.Get(skill).Damage, damage).ToString("0.0%");
                info[3] = skills.Get(skill).UsesFormatted;
                item = new ListViewItem(info);
                SkillList.Items.Add(item);
            }
        }

        private double GetPercent(int damage, long total)
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
