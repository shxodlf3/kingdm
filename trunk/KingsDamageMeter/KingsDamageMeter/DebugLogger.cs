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
