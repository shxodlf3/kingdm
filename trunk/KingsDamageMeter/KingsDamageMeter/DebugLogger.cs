using System;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace KingsDamageMeter
{
    public static class DebugLogger
    {
        private static string _DebugLogPath = KingsDamageMeter.Properties.Settings.Default.DebugFile;

        private static bool DebugEnabled
        {
            get
            {
                return KingsDamageMeter.Properties.Settings.Default.Debug;
            }
        }

        public static void Write(string message)
        {
            if (!DebugEnabled)
            {
                return;
            }

            try
            {
                using (StreamWriter writer = File.AppendText(_DebugLogPath))
                {
                    writer.WriteLine(message);
                }
            }

            catch (Exception e)
            {
                MessageBox.Show("Unable to write to debug log (" + _DebugLogPath + "):" + Environment.NewLine + e.Message);
            }
        }
    }
}
