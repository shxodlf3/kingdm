﻿/**************************************************************************\
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
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using KingsDamageMeter.Controls;
using KingsDamageMeter.Converters;
using KingsDamageMeter.Forms;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter
{
    public partial class WindowMain
    {
    	private bool isInitializing;
        public WindowMain()
        {
            DataContext = new WindowMainData();

            try
            {
                isInitializing = true;
                InitializeComponent();
            }
            finally
            {
                isInitializing = false;
            }

            if (Settings.Default.IsEncountersExpanded)
            {
                EncountersExpander_Expanded(EncountersExpander, new RoutedEventArgs());
            }

            NotifyIcon.Clicked += OnNotifyIconClicked;
            NotifyIcon.ShowHideClicked += OnNotifyShowHideClicked;
            NotifyIcon.CloseClicked += OnNotifyCloseClicked;
        }

        private bool _Dragging;
        private Point _MousePoint;

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
            if (_Dragging)
            {
                Point p = e.GetPosition(this);
                Left += p.X - _MousePoint.X;
                Top += p.Y - _MousePoint.Y;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowMainData data = (WindowMainData)DataContext;

            if (data.Regions.Where(o=>!(o is AllEncounters)).Count() > 0)
            {
                string message = Localization.WindowMainRes.ConfirmCloseMessage;
                string caption = Localization.WindowMainRes.ConfirmCloseCaption;
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            data.OnClose();
        }

        #endregion

        #region Control Events

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
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
            string path = Settings.Default.HelpFilePath;

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
            var d = new System.Windows.Forms.OpenFileDialog();
            //d.InitialDirectory = "C:\\";
            d.Filter = "Chat.log (Chat.log)|Chat.log|All files (*.*)|*.*";

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((WindowMainData) DataContext).ChangeLogFile(d.FileName);
            }
        }
        
        private void MenuItemAddGroupMemberByName_Click(object sender, RoutedEventArgs e)
        {
            SetNameDialog d = new SetNameDialog();
            d.Text = WindowMainRes.AddByNameMenuHeader;

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((WindowMainData)DataContext).AddGroupMemberPlayer(d.PlayerName);
            }
        }

        private void MainContextMenuSetYouAlias_Click(object sender, RoutedEventArgs e)
        {
            SetNameDialog d = new SetNameDialog();
            d.Text = WindowMainRes.SetYouAliasMenuHeader;
            d.PlayerName = Settings.Default.YouAlias;

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                if (((WindowMainData)DataContext).PlayerExists(d.PlayerName))
                {
                    MessageBox.Show(WindowMainRes.NameTakenMessage, "Oops", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                else
                {
                    ((WindowMainData)DataContext).Rename(d.PlayerName, Settings.Default.YouAlias);
                    Settings.Default.YouAlias = d.PlayerName;
                }
            }
        }

        #endregion

        private bool isLoaded;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!isLoaded)
            {
                var menuStyle = (Style) Resources["NotUncheckedMenuItemStyle"];
                var languageSelectedConverter = new IsLanguageSelectedConverter();
                foreach (var language in ((WindowMainData)DataContext).AvailableLanguages)
                {
                    var menuItem = new MenuItem
                                       {
                                           Header = language.DisplayName,
                                           IsCheckable = true,
                                           Style = menuStyle
                                       };
                    menuItem.SetBinding(MenuItem.IsCheckedProperty,
                                        new Binding("SelectedLanguage") { Mode = BindingMode.TwoWay, Source = Settings.Default, Converter = languageSelectedConverter, ConverterParameter = language});
                    MenuItemLanguage.Items.Add(menuItem);
                }
                foreach (var language in ((WindowMainData)DataContext).AvailableLogLanguages)
                {
                    var menuItem = new MenuItem
                    {
                        Header = language.DisplayName,
                        IsCheckable = true,
                        Style = menuStyle
                    };
                    menuItem.SetBinding(MenuItem.IsCheckedProperty,
                                        new Binding("SelectedLogLanguage") { Mode = BindingMode.TwoWay, Source = Settings.Default, Converter = languageSelectedConverter, ConverterParameter = language });
                    MenuItemLogLanguage.Items.Add(menuItem);
                }

                isLoaded = true;
            }
        }

        private void MagicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary skinDict =
                    Application.LoadComponent(new Uri("/KingsDamageMeter;component/Themes/BlueSky.xaml", UriKind.Relative)) as ResourceDictionary;

            Collection<ResourceDictionary> mergedDicts =
                Application.Current.Resources.MergedDictionaries;

            // Remove the existing skin dictionary, if one exists.

            // NOTE: In a real application, this logic might need

            // to be more complex, because there might be dictionaries

            // which should not be removed.

            if (mergedDicts.Count > 0)
                mergedDicts.Clear();

            // Apply the selected skin so that all elements in the

            // application will honor the new look and feel.

            mergedDicts.Add(skinDict);
        }

		private void EncountersExpander_Expanded(object sender, RoutedEventArgs e)
		{
			if (isInitializing)
			{
				return;
			}

			if (IsLoaded)
			{
                var size = Settings.Default.EncountersWidth - EncountersColumn.MinWidth - EncountersSplitter.Width;
				Left -= size;
				Width += size;
			}
            EncountersColumn.Width = new GridLength(Settings.Default.EncountersWidth - EncountersSplitter.Width);

			var dpd = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
			if (dpd != null)
			{
				dpd.AddValueChanged(EncountersColumn, EncountersColumnWidthChanged);
			}
		}

		private void EncountersExpander_Collapsed(object sender, RoutedEventArgs e)
		{
            var size = Settings.Default.EncountersWidth - EncountersColumn.MinWidth - EncountersSplitter.Width;
			Width -= size;
			Left += size;

			EncountersColumn.Width = new GridLength(EncountersColumn.MinWidth);
			
			var dpd = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
			if (dpd != null)
			{
				dpd.RemoveValueChanged(EncountersColumn, EncountersColumnWidthChanged);
			}
		}
	
		private void EncountersColumnWidthChanged(object sender, EventArgs e)
		{
			if (EncountersExpander.IsExpanded)
			{
				Settings.Default.EncountersWidth = EncountersColumn.Width.Value;
			}
		}

        private void EncountersTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((WindowMainData) DataContext).SelectedEncounter = (EncounterBase) e.NewValue;
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TreeViewItem) sender).IsSelected = true;
            e.Handled = true;
        }

        private void OnNotifyIconClicked(object sender, EventArgs e)
        {
            ToggleVisibility();
        }

        private void ToggleVisibility()
        {
            if (Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Visible;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(delegate()
                    {
                        WindowState = WindowState.Normal;
                    })
                );
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Visibility = Visibility.Hidden;
            }
        }

        private void OnNotifyShowHideClicked(object sender, EventArgs e)
        {
            ToggleVisibility();
        }

        private void OnNotifyCloseClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
