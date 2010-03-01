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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Forms;
using KingsDamageMeter.Languages;
using KingsDamageMeter.Properties;
using WPFLocalizeExtension.Engine;

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

            if (Settings.Default.IgnoreList == null)
            {
                Settings.Default.IgnoreList = new ObservableCollection<string>();
            }
            if (Settings.Default.GroupList == null)
            {
                Settings.Default.GroupList = new ObservableCollection<string>();
            }

            Settings.Default.PropertyChanged += SettingsChnaged;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            Regex.Default.Save();
            base.OnExit(e);
        }

        private void SettingsChnaged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedLanguage")
            {
                LocalizeDictionary.Instance.Culture = Settings.Default.SelectedLanguage;
                Thread.CurrentThread.CurrentUICulture = Settings.Default.SelectedLanguage;
            }
        }

    }
}
