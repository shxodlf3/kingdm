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

        public static event EventHandler Clicked;
        public static event EventHandler ShowHideClicked;
        public static event EventHandler CloseClicked;

        static NotifyIcon()
        {
            _NotifyIcon.MouseClick += OnMouseClick;
            CreateMenu();
        }

        public static void Show()
        {
            _NotifyIcon.Visible = true;
        }

        public static void Hide()
        {
            _NotifyIcon.Visible = false;
        }

        private static void CreateMenu()
        {
            WinForms.ContextMenu menu = new WinForms.ContextMenu();
            menu.MenuItems.Add(new WinForms.MenuItem("Show/Hide", OnShowHideClicked));
            menu.MenuItems.Add(new WinForms.MenuItem("-"));
            menu.MenuItems.Add(new WinForms.MenuItem("Close", OnCloseClicked));
            _NotifyIcon.ContextMenu = menu;
        }

        private static void OnMouseClick(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button == WinForms.MouseButtons.Left)
            {
                if (Clicked != null)
                {
                    Clicked(null, EventArgs.Empty);
                }
            }
        }

        private static void OnShowHideClicked(object sender, EventArgs e)
        {
            if (ShowHideClicked != null)
            {
                ShowHideClicked(null, EventArgs.Empty);
            }
        }

        private static void OnCloseClicked(object sender, EventArgs e)
        {
            if (CloseClicked != null)
            {
                CloseClicked(null, EventArgs.Empty);
            }
        }
    }
}
