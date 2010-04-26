using System;
using System.Drawing;
using WinForms = System.Windows.Forms;

namespace KingsDamageMeter
{
    /// <summary>
    /// A wrapper class for the Windows Forms NotifyIcon.
    /// </summary>
    public static class NotifyIcon
    {
        private static WinForms.NotifyIcon _NotifyIcon = new WinForms.NotifyIcon();

        public static Icon Icon
        {
            get
            {
                return _NotifyIcon.Icon;
            }

            set
            {
                _NotifyIcon.Icon = value;
            }
        }

        public static void Show()
        {
            _NotifyIcon.Visible = true;
        }

        public static void Hide()
        {
            _NotifyIcon.Visible = false;
        }
    }
}
