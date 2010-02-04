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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;

using KingsDamageMeter.Forms;

namespace KingsDamageMeter
{
    public partial class WindowMain : Window
    {
        private bool _Dragging = false;
        private Point _MousePoint;

        private AionLogParser _LogParser = new AionLogParser();

        private delegate void LogParserDamageInflicted_Callback(object sender, DamageInflictedEventArgs e);

        public WindowMain()
        {
            InitializeComponent();
            LoadSettings();
            _LogParser.FileNotFound += OnLogParserFileNotFound;
            _LogParser.DamageInflicted += OnLogParserDamageInflicted;
            _LogParser.CriticalInflicted += OnLogParserDamageInflicted;
            _LogParser.Start(KingsDamageMeter.Properties.Settings.Default.AionLogPath);
            PlayerViewer.IgnoreListChanged += OnPlayerViewerIgnoreListChanged;
        }

        private void LoadSettings()
        {
            MainContextMenuLocateLog.ToolTip = KingsDamageMeter.Properties.Settings.Default.AionLogPath;
            Left = KingsDamageMeter.Properties.Settings.Default.WindowMainX;
            Top = KingsDamageMeter.Properties.Settings.Default.WindowMainY;
            Opacity = KingsDamageMeter.Properties.Settings.Default.WindowMainOpacity;
            OpacitySlider.Value = Opacity;
            Topmost = KingsDamageMeter.Properties.Settings.Default.WindowMainTopMost;
            CheckTopMost.IsChecked = Topmost;
            PlayerViewer.IgnoreList = KingsDamageMeter.Properties.Settings.Default.IgnoreList;
        }

        private void SaveSettings()
        {
            KingsDamageMeter.Properties.Settings.Default.WindowMainX = Left;
            KingsDamageMeter.Properties.Settings.Default.WindowMainY = Top;
            KingsDamageMeter.Properties.Settings.Default.WindowMainOpacity = Opacity;
            KingsDamageMeter.Properties.Settings.Default.WindowMainTopMost = Topmost;

            KingsDamageMeter.Properties.Settings.Default.Save();
        }

        #region Window Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CaptureMouse();
                _MousePoint = e.GetPosition(this);
                _Dragging = true;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
           _Dragging = false;
           ReleaseMouseCapture();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Dragging && _MousePoint != null)
            {
                Point p = e.GetPosition(this);
                Left += p.X - _MousePoint.X;
                Top += p.Y - _MousePoint.Y;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _LogParser.Stop();
            SaveSettings();
        }

        #endregion

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = OpacitySlider.Value;
        }

        private void CheckTopMost_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void CheckTopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
        }

        private void CloseButton_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MenuButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainContextMenu.IsOpen = true;
        }

        #region MainContextMenu

        private void MainContextMenuIgnoreList_Click(object sender, RoutedEventArgs e)
        {
            IgnoreListForm i = new IgnoreListForm();
            i.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            i.TopMost = true;
            i.ShowDialog();
        }

        private void MainContextMenuHelp_Click(object sender, RoutedEventArgs e)
        {
            string path = KingsDamageMeter.Properties.Settings.Default.HelpFilePath;

            if (File.Exists(path))
            {
                Process.Start(path);
            }

            else
            {
                MessageBox.Show("Cannot find '" + path + "'", "Oops");
            }
        }

        private void MainContextMenuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutForm a = new AboutForm();
            a.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            a.TopMost = true;
            a.ShowDialog();
        }

        private void MainContextMenuLocateLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
            d.InitialDirectory = "C:\\";
            d.Filter = "Chat.log (Chat.log)|Chat.log|All files (*.*)|*.*";

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                KingsDamageMeter.Properties.Settings.Default.AionLogPath = d.FileName;
                _LogParser.Start(KingsDamageMeter.Properties.Settings.Default.AionLogPath);
                MainContextMenuLocateLog.ToolTip = KingsDamageMeter.Properties.Settings.Default.AionLogPath;
            }
        }

        #endregion

        private void OnLogParserDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            Dispatcher.Invoke(new LogParserDamageInflicted_Callback(DoLogParserDamageInflicted), sender, e);
        }

        private void DoLogParserDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            if (!PlayerViewer.PlayerExists(e.Name))
            {
                PlayerViewer.AddPlayer(e.Name);
            }

            PlayerViewer.UpdatePlayerDamage(e.Name, e.Damage);
        }

        private void OnLogParserFileNotFound(object sender, EventArgs e)
        {
            MessageBox.Show("Unable to find log file.", "Oops", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void OnPlayerViewerIgnoreListChanged(object sender, EventArgs e)
        {
            KingsDamageMeter.Properties.Settings.Default.IgnoreList = PlayerViewer.IgnoreList;
        }
    }
}
