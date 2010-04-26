using System.Windows.Forms;
using System.IO;

namespace KingsDamageMeter.Shortcuts
{
    public class ShortcutKey
    {
        public bool Control
        {
            get;
            private set;
        }

        public bool Alt
        {
            get;
            private set;
        }

        public bool Shift
        {
            get;
            private set;
        }

        public Keys Key
        {
            get;
            private set;
        }

        public ShortcutKey(bool control, bool alt, bool shift, Keys key)
        {
            Control = control;
            Alt = alt;
            Shift = shift;
            Key = key;
        }
    }
}
