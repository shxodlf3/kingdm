using System;
using System.Collections;
using System.Windows.Forms;

namespace KingsDamageMeter.Forms
{
    public class SkillSorter : IComparer
    {
        private int _SortColumn;
        private SortOrder _SortOrder;

        public SkillSorter()
        {
            _SortColumn = 0;
            _SortOrder = SortOrder.None;
        }

        public int SortColumn
        {
            set
            {
                _SortColumn = value;
            }
            get
            {
                return _SortColumn;
            }
        }

        public SortOrder SortOrder
        {
            set
            {
                _SortOrder = value;
            }
            get
            {
                return _SortOrder;
            }
        }

        public int Compare(object x, object y)
        {
            int result = 0;
            ListViewItem itemx = (ListViewItem)x;
            ListViewItem itemy = (ListViewItem)y;

            if (_SortColumn > 0)
            {
                int a = Convert.ToInt32(itemx.SubItems[_SortColumn].Text.GetDigits());
                int b = Convert.ToInt32(itemy.SubItems[_SortColumn].Text.GetDigits());

                if (a > b)
                {
                    result = 1;
                }

                else
                {
                    result = -1;
                }
            }

            else
            {
                result = String.Compare(itemx.SubItems[_SortColumn].Text, itemy.SubItems[_SortColumn].Text);
            }

            if (_SortOrder == SortOrder.Ascending)
            {
                return result;
            }

            else if (_SortOrder == SortOrder.Descending)
            {
                return (-result);
            }

            else
            {
                return 0;
            }
        }
    }
}
