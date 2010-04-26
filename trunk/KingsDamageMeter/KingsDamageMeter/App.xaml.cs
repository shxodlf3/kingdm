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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using KingsDamageMeter.Forms;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;
using WPFLocalizeExtension.Engine;
using KingsDamageMeter.Windows;

namespace KingsDamageMeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DebugLogger.Write(Environment.NewLine + "Started KDM " + DateTime.Now.ToString());

            NotifyIcon.Icon = KingsDamageMeter.Properties.Resources.Lion;
            //NotifyIcon.Show();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                ExceptionForm f = new ExceptionForm();
                string exception = ex.Message + Environment.NewLine + ex.StackTrace;
                DebugLogger.Write("Unhandled Exception: " + exception);
                f.Exception = exception;
                f.ShowDialog();
            }

            finally
            {
                Application.Current.Shutdown();
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            Settings.Default.Debug = true;
#endif

            if (Settings.Default.IgnoreList == null)
            {
                Settings.Default.IgnoreList = new ObservableCollection<string>();
            }
            if (Settings.Default.FriendList == null)
            {
                Settings.Default.FriendList = new ObservableCollection<string>();
            }

            Settings.Default.PropertyChanged += SettingsChanged;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //NotifyIcon.Hide();
            Settings.Default.Save();
            base.OnExit(e);
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsHideOthers")
            {
                if(Settings.Default.IsHideOthers)
                {
                    Settings.Default.IsGroupOnly = false;
                }
            }
            else if (e.PropertyName == "IsGroupOnly")
            {
                if (Settings.Default.IsGroupOnly)
                {
                    Settings.Default.IsHideOthers = false;
                }
            }
        }

    }
}
