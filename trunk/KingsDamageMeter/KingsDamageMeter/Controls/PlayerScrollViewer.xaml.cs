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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A class that represents a player scroll viewer.
    /// </summary>
    public partial class PlayerScrollViewer : UserControl
    {
        private bool _HideOthers = false;

        private Dictionary<string, PlayerControl> _Players = new Dictionary<string, PlayerControl>();
        private StringCollection _IgnoreList = new StringCollection();
        private PlayerControl _SelectedPlayer;
        private PlayerControl _WorkingPlayer;

        public event EventHandler IgnoreListChanged;

        /// <summary>
        /// Gets or sets the player names that will be ignored.
        /// </summary>
        public StringCollection IgnoreList
        {
            get
            {
                return _IgnoreList;
            }

            set
            {
                _IgnoreList = value;

                if (IgnoreListChanged != null)
                {
                    IgnoreListChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// A class that represents a player scroll viewer.
        /// </summary>
        public PlayerScrollViewer()
        {
            InitializeComponent();
            SetMainContextMenuHeaders();
        }

        private void SetMainContextMenuHeaders()
        {
            MenuItemCopyYou.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuCopyYou;
            MenuItemCopyTop.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuCopyAll;
            MenuItemHideOthers.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuHideOthers;
            MenuItemRemove.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuRemove;
            MenuItemIgnore.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuIgnore;
            MenuItemSortByName.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuSortName;
            MenuItemSortByDamage.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuSortDamage;
            MenuItemClear.Header = KingsDamageMeter.Languages.En.Default.PlayerMenuClearAll;
        }

        /// <summary>
        /// Add a player to the scroll viewer.
        /// </summary>
        /// <param name="name">The name of the player</param>
        public void AddPlayer(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                CreateIgnoreList();

                if (_IgnoreList.Contains(name) && name != "You")
                {
                    return;
                }

                PlayerControl p = new PlayerControl();
                p.PlayerName = name;
                p.Damage = 0;
                p.DamagePercent = 0;
                p.MouseEnter += OnPlayerControlMouseEnter;
                _Players.Add(name, p);

                if (name == "You")
                {
                    PlayerPanel.Children.Add(p);
                    return;
                }

                if (!_HideOthers)
                {
                    PlayerPanel.Children.Add(p);
                }
            }
        }

        /// <summary>
        /// Remove a player from the scroll viewer.
        /// </summary>
        /// <param name="name">The name of the player</param>
        public void RemovePlayer(string name)
        {
            if (PlayerExists(name))
            {
                if (PlayerPanel.Children.Contains(_Players[name]))
                {
                    PlayerPanel.Children.Remove(_Players[name]);
                }

                _Players.Remove(name);
                UpdatePercents();
            }
        }

        /// <summary>
        /// Update a player's damage.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="damage">The total damage the player has dealt</param>
        public void UpdatePlayerDamage(string name, int damage)
        {
            if (PlayerExists(name))
            {
                CreateIgnoreList();

                if (_IgnoreList.Contains(name))
                {
                    RemovePlayer(name);
                    return;
                }

                _Players[name].Damage += damage;
                UpdatePercents();
            }
        }

        /// <summary>
        /// Indicates whether the player already exists in the scroll viewer.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <returns>bool</returns>
        public bool PlayerExists(string name)
        {
            return _Players.ContainsKey(name);
        }

        /// <summary>
        /// Adds a player to the ignore list.
        /// </summary>
        /// <param name="name">The name of the player</param>
        public void IgnorePlayer(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                CreateIgnoreList();
                IgnoreList.Add(name);
                RemovePlayer(name);

                if (IgnoreListChanged != null)
                {
                    IgnoreListChanged(this, EventArgs.Empty);
                }
            }
        }

        private void UpdatePercents()
        {
            int total = 0;

            foreach (PlayerControl p in _Players.Values)
            {
                total += p.Damage;
            }

            foreach (PlayerControl p in _Players.Values)
            {
                double percent = (double)((double)(p.Damage - total) / total) + 1;
                p.DamagePercent = percent;
            }
        }

        private void CreateIgnoreList()
        {
            if (_IgnoreList == null)
            {
                _IgnoreList = new StringCollection();
            }
        }

        private void HideOthers()
        {
            foreach (PlayerControl p in _Players.Values)
            {
                if (PlayerPanel.Children.Contains(p))
                {
                    if (p.PlayerName != "You")
                    {
                        PlayerPanel.Children.Remove(p);
                    }
                }
            }
        }

        private void ShowOthers()
        {
            foreach (PlayerControl p in _Players.Values)
            {
                if (!PlayerPanel.Children.Contains(p))
                {
                    PlayerPanel.Children.Add(p);
                }
            }
        }

        private void ClearAll()
        {
            foreach (PlayerControl p in _Players.Values)
            {
                if (PlayerPanel.Children.Contains(p))
                {
                    PlayerPanel.Children.Remove(p);
                }
            }

            _Players.Clear();
        }

        private void SortByName()
        {
            PlayerPanel.Children.Clear();

            var players = (from player in _Players orderby player.Value.PlayerName ascending select player.Value);

            foreach (PlayerControl player in players)
            {
                PlayerPanel.Children.Add(player);
            }
        }

        private void SortByDamage()
        {
            PlayerPanel.Children.Clear();

            var players = (from player in _Players orderby player.Value.Damage descending select player.Value);

            foreach (PlayerControl player in players)
            {
                PlayerPanel.Children.Add(player);
            }
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void MenuItemHideOthers_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemHideOthers.IsChecked)
            {
                _HideOthers = true;
                HideOthers();
            }

            else
            {
                _HideOthers = false;
                ShowOthers();
            }
        }

        private void OnPlayerControlMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _SelectedPlayer = (PlayerControl)sender;
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_WorkingPlayer != null)
            {
                RemovePlayer(_WorkingPlayer.PlayerName);
            }
        }

        private void MenuItemIgnore_Click(object sender, RoutedEventArgs e)
        {
            if (_WorkingPlayer != null)
            {
                IgnorePlayer(_WorkingPlayer.PlayerName);
            }
        }

        private void MainContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            _WorkingPlayer = _SelectedPlayer;
        }

        private void MenuItemCopyTop_Click(object sender, RoutedEventArgs e)
        {
            string result = String.Empty;

            foreach (PlayerControl p in _Players.Values)
            {
                double percent = (double)p.DamagePercent * 100;
                result += p.ToString() + " ";
            }

            Clipboard.SetText(result);
        }

        private void MenuItemCopyYou_Click(object sender, RoutedEventArgs e)
        {
            if (_Players.ContainsKey("You"))
            {
                PlayerControl p = _Players["You"];
                Clipboard.SetText(p.ToString());
            }
        }

        private void MenuItemSortByName_Click(object sender, RoutedEventArgs e)
        {
            SortByName();
        }

        private void MenuItemSortByDamage_Click(object sender, RoutedEventArgs e)
        {
            SortByDamage();
        }
    }
}
