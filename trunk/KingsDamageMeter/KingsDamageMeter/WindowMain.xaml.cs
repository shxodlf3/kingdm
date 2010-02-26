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
using System.Collections;

using KingsDamageMeter.Forms;

namespace KingsDamageMeter
{
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();
            LoadSettings();
            InitializeLogParser();
            SetToolTips();
            SetMainContextMenuHeaders();
            
            PlayerViewer.IgnoreListChanged += OnPlayerViewerIgnoreListChanged;
        }

        private bool _Dragging = false;
        private Point _MousePoint;

        private AionLogParser _LogParser = new AionLogParser();

        private delegate void DamageInflicted_Callback(object sender, PlayerDamageEventArgs e);
        private delegate void CriticalInflicted_Callback(object sender, PlayerDamageEventArgs e);
        private delegate void SkillDamageInflicted_Callback(object sender, PlayerSkillDamageEventArgs e);
        private delegate void PlayerJoinedGroup_Callback(object sender, PlayerEventArgs e);
        private delegate void PlayerLeftGroup_Callback(object sender, PlayerEventArgs e);
        private delegate void ExpGained_Callback(object sender, ExpEventArgs e);
        private delegate void KinahEarned_Callback(object sender, KinahEventArgs e);

        #region Initialization

        private void LoadSettings()
        {
            MainContextMenuLocateLog.ToolTip = KingsDamageMeter.Properties.Settings.Default.AionLogPath;

            Left = KingsDamageMeter.Properties.Settings.Default.WindowMainX;
            Top = KingsDamageMeter.Properties.Settings.Default.WindowMainY;
            Opacity = KingsDamageMeter.Properties.Settings.Default.WindowMainOpacity;
            OpacitySlider.Value = Opacity;
            Topmost = KingsDamageMeter.Properties.Settings.Default.WindowMainTopMost;
            CheckTopMost.IsChecked = Topmost;

            int width = KingsDamageMeter.Properties.Settings.Default.WindowMainWidth;
            int height = KingsDamageMeter.Properties.Settings.Default.WindowMainHeight;
            Width = (width < MinWidth) ? MinWidth : width;
            Height = (height < MinHeight) ? MinHeight : height;

            PlayerViewer.IgnoreList = KingsDamageMeter.Properties.Settings.Default.IgnoreList;
            PlayerViewer.HideAllOthers = KingsDamageMeter.Properties.Settings.Default.HideOthers;
            PlayerViewer.GroupOnly = KingsDamageMeter.Properties.Settings.Default.GroupOnly;
        }

        private void InitializeLogParser()
        {
            _LogParser.FileNotFound += OnFileNotFound;
            _LogParser.DamageInflicted += OnDamageInflicted;
            _LogParser.CriticalInflicted += OnCriticalDamageInflicted;
            _LogParser.SkillDamageInflicted += OnSkillDamageInflicted;
            _LogParser.PlayerJoinedGroup += OnPlayerJoinedGroup;
            _LogParser.PlayerLeftGroup += OnPlayerLeftGroup;
            _LogParser.ExpGained += OnExpGained;
            _LogParser.KinahEarned += OnKinahEarned;
            _LogParser.Started += OnParserStarted;
            _LogParser.Stopped += OnParserStopped;
            _LogParser.Start(KingsDamageMeter.Properties.Settings.Default.AionLogPath);
        }

        private void SetToolTips()
        {
            PowerButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.RunningToolTip;
            MinimizeButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.MinimizeToolTip;
            MenuButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.OptionsToolTip;
            CloseButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.CloseToolTip;
            OpacitySlider.ToolTip = KingsDamageMeter.Languages.Gui.Default.OpacityToolTip;
            CheckTopMost.ToolTip = KingsDamageMeter.Languages.Gui.Default.TopMostToolTip;
            ResizeThumb.ToolTip = KingsDamageMeter.Languages.Gui.Default.ResizeToolTip;
        }

        private void SetMainContextMenuHeaders()
        {
            MainContextMenuLocateLog.Header = KingsDamageMeter.Languages.Gui.Default.OptionsMenuLocateLog;
            MainContextMenuIgnoreList.Header = KingsDamageMeter.Languages.Gui.Default.OptionsMenuIgnoreList;
            MainContextMenuHelp.Header = KingsDamageMeter.Languages.Gui.Default.OptionsMenuHelp;
            MainContextMenuAbout.Header = KingsDamageMeter.Languages.Gui.Default.OptionsMenuAbout;
        }

        private void SaveSettings()
        {
            KingsDamageMeter.Properties.Settings.Default.WindowMainX = Left;
            KingsDamageMeter.Properties.Settings.Default.WindowMainY = Top;
            KingsDamageMeter.Properties.Settings.Default.WindowMainWidth = (int)Width;
            KingsDamageMeter.Properties.Settings.Default.WindowMainHeight = (int)Height;
            KingsDamageMeter.Properties.Settings.Default.WindowMainOpacity = Opacity;
            KingsDamageMeter.Properties.Settings.Default.WindowMainTopMost = Topmost;

            KingsDamageMeter.Properties.Settings.Default.HideOthers = PlayerViewer.HideAllOthers;
            KingsDamageMeter.Properties.Settings.Default.GroupOnly = PlayerViewer.GroupOnly;

            KingsDamageMeter.Properties.Settings.Default.Save();
        }

        #endregion

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

        #region Control Events

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

        private void ThumbResize_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double width = Width + e.HorizontalChange;
            double height = Height + e.VerticalChange;

            Width = (width < MinWidth) ? MinWidth : width;
            Height = (height < MinHeight) ? MinHeight : height;
        }

        #endregion

        #region MainContextMenu Events

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
                MessageBox.Show("Cannot find '" + path + "'", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                if (_LogParser.Running)
                {
                    _LogParser.Stop();
                }

                KingsDamageMeter.Properties.Settings.Default.AionLogPath = d.FileName;
                _LogParser.Start(KingsDamageMeter.Properties.Settings.Default.AionLogPath);
                MainContextMenuLocateLog.ToolTip = KingsDamageMeter.Properties.Settings.Default.AionLogPath;
            }
        }

        #endregion

        #region LogParser Events

        private void OnFileNotFound(object sender, EventArgs e)
        {
            MessageBox.Show(KingsDamageMeter.Languages.Gui.Default.OpenLogError, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void OnDamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            Dispatcher.Invoke(new DamageInflicted_Callback(DoDamageInflicted), sender, e);
        }

        private void DoDamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            if (!PlayerViewer.PlayerExists(e.Name))
            {
                PlayerViewer.AddPlayer(e.Name);
            }

            PlayerViewer.UpdatePlayerDamage(e.Name, e.Damage);
        }

        private void OnSkillDamageInflicted(object sender, PlayerSkillDamageEventArgs e)
        {
            Dispatcher.Invoke(new SkillDamageInflicted_Callback(DoSkillDamageInflicted), sender, e);
        }

        private void DoSkillDamageInflicted(object sender, PlayerSkillDamageEventArgs e)
        {
            if (!PlayerViewer.PlayerExists(e.Name))
            {
                PlayerViewer.AddPlayer(e.Name);
            }

            PlayerViewer.UpdatePlayerDamage(e.Name, e.Damage, e.Skill);
        }

        private void OnCriticalDamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            Dispatcher.Invoke(new CriticalInflicted_Callback(DoCriticalDamageInflicted), sender, e);
        }

        private void DoCriticalDamageInflicted(object sender, PlayerDamageEventArgs e)
        {
            if (!PlayerViewer.PlayerExists(e.Name))
            {
                PlayerViewer.AddPlayer(e.Name);
            }

            PlayerViewer.UpdatePlayerDamage(e.Name, e.Damage);
        }

        private void OnPlayerJoinedGroup(object sender, PlayerEventArgs e)
        {
            Dispatcher.Invoke(new PlayerJoinedGroup_Callback(DoPlayerJoinedGroup), sender, e);
        }

        private void DoPlayerJoinedGroup(object sender, PlayerEventArgs e)
        {
            PlayerViewer.AddGroupMember(e.Name);
        }

        private void OnPlayerLeftGroup(object sender, PlayerEventArgs e)
        {
            Dispatcher.Invoke(new PlayerJoinedGroup_Callback(DoPlayerLeftGroup), sender, e);
        }

        private void DoPlayerLeftGroup(object sender, PlayerEventArgs e)
        {
            PlayerViewer.RemoveGroupMember(e.Name);
        }

        private void OnExpGained(object sender, ExpEventArgs e)
        {
            Dispatcher.Invoke(new ExpGained_Callback(DoExpGained), sender, e);
        }

        private void DoExpGained(object sender, ExpEventArgs e)
        {
            PlayerViewer.UpdateExp(e.Exp);
        }

        private void OnKinahEarned(object sender, KinahEventArgs e)
        {
            Dispatcher.Invoke(new KinahEarned_Callback(DoKinahEarned), sender, e);
        }

        private void DoKinahEarned(object sender, KinahEventArgs e)
        {
            PlayerViewer.UpdateKinah(e.Kinah);
        }

        private void OnPlayerViewerIgnoreListChanged(object sender, EventArgs e)
        {
            KingsDamageMeter.Properties.Settings.Default.IgnoreList = PlayerViewer.IgnoreList;
        }

        private void OnParserStarted(object sender, EventArgs e)
        {
            TogglePowerButton();
        }

        private void OnParserStopped(object sender, EventArgs e)
        {
            TogglePowerButton();
        }

        #endregion

        private void PowerButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_LogParser.Running)
            {
                _LogParser.Stop();
            }

            else
            {
                _LogParser.Start(KingsDamageMeter.Properties.Settings.Default.AionLogPath);
            }
        }

        private void TogglePowerButton()
        {
            if (_LogParser.Running)
            {
                PowerButton.MouseDownImage = "pack://application:,,,/Themes/BlackPearl/Images/OnButtonPress.bmp";
                PowerButton.MouseOverImage = "pack://application:,,,/Themes/BlackPearl/Images/OnButton.bmp";
                PowerButton.MouseUpImage = "pack://application:,,,/Themes/BlackPearl/Images/OnButton.bmp";
                PowerButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.RunningToolTip;
            }

            else
            {
                PowerButton.MouseDownImage = "pack://application:,,,/Themes/BlackPearl/Images/OffButtonPress.bmp";
                PowerButton.MouseOverImage = "pack://application:,,,/Themes/BlackPearl/Images/OffButton.bmp";
                PowerButton.MouseUpImage = "pack://application:,,,/Themes/BlackPearl/Images/OffButton.bmp";
                PowerButton.ToolTip = KingsDamageMeter.Languages.Gui.Default.DisabledToolTip;
            }
        }
    }
}
