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

using KingsDamageMeter.Forms;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A class that represents a player scroll viewer.
    /// </summary>
    public partial class PlayerScrollViewer : UserControl
    {
        private bool _HideOthers = false;
        private bool _GroupOnly = true;

        private Dictionary<string, PlayerControl> _Players = new Dictionary<string, PlayerControl>();
        private StringCollection _IgnoreList = new StringCollection();
        private PlayerControl _SelectedPlayer;
        private PlayerControl _WorkingPlayer;

        private string _YouAlias = KingsDamageMeter.Languages.Regex.Default.YouAlias;

        private PlayerSortType _LastSortType = PlayerSortType.None;

        public event EventHandler IgnoreListChanged;
        public event EventHandler HideOthersChanged;
        public event EventHandler GroupOnlyChanged;

        /// <summary>
        /// 
        /// </summary>
        public bool HideAllOthers
        {
            get
            {
                return _HideOthers;
            }

            set
            {
                _HideOthers = value;

                if (HideOthersChanged != null)
                {
                    HideOthersChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GroupOnly
        {
            get
            {
                return _GroupOnly;
            }

            set
            {
                _GroupOnly = value;

                if (GroupOnlyChanged != null)
                {
                    GroupOnlyChanged(this, EventArgs.Empty);
                }
            }
        }

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

            HideOthersChanged += OnHideOthersChanged;
            GroupOnlyChanged += OnGroupOnlyChanged;
        }

        private void SetMainContextMenuHeaders()
        {
            MenuItemCopyYou.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuCopyYou;
            MenuItemCopyTop.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuCopyAll;
            MenuItemGroupOnly.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuGroupOnly;
            MenuItemHideOthers.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuHideOthers;
            MenuItemAddGroupMemberByName.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuAddMemberByName;
            MenuItemRemove.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuRemove;
            MenuItemIgnore.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuIgnore;
            MenuItemSortByName.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuSortName;
            MenuItemSortByDamage.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuSortDamage;
            MenuItemResetDamage.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuResetDamage;
            MenuItemClear.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuClearAll;
            MenuItemGroupMember.Header = KingsDamageMeter.Languages.Gui.Default.PlayerMenuGroupMember;
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

                if (_IgnoreList.Contains(name) && name != _YouAlias)
                {
                    return;
                }

                PlayerControl p = new PlayerControl();
                p.PlayerName = name;
                p.Damage = 0;
                p.DamagePercent = 0;
                p.MouseEnter += OnPlayerControlMouseEnter;

                _Players.Add(name, p);

                if (name == _YouAlias)
                {
                    _Players[name].GroupMember = true;
                    PlayerPanel.Children.Add(p);
                    return;
                }

                if (!_HideOthers && !_GroupOnly)
                {
                    PlayerPanel.Children.Add(p);
                }
            }
        }

        /// <summary>
        /// Add or update an existing player as a group member.
        /// </summary>
        /// <param name="name">The name of the player</param>
        public void AddGroupMember(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                CreateIgnoreList();

                if (_IgnoreList.Contains(name) && name != _YouAlias)
                {
                    return;
                }

                if (_Players.ContainsKey(name))
                {
                    _Players[name].GroupMember = true;
                }

                else
                {
                    PlayerControl p = new PlayerControl();
                    p.PlayerName = name;
                    p.Damage = 0;
                    p.DamagePercent = 0;
                    p.GroupMember = true;
                    p.MouseEnter += OnPlayerControlMouseEnter;

                    _Players.Add(name, p);

                    if (!_HideOthers)
                    {
                        PlayerPanel.Children.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void RemoveGroupMember(string name)
        {
            if (_Players.ContainsKey(name))
            {
                _Players[name].GroupMember = false;
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

        public void ResetDamage()
        {
            if (_Players == null)
            {
                return;
            }

            foreach (PlayerControl p in _Players.Values)
            {
                p.Reset();
            }
        }

        private void UpdatePercents()
        {
            int total = 0;

            foreach (PlayerControl p in _Players.Values)
            {
                if (_GroupOnly)
                {
                    if (p.GroupMember)
                    {
                        total += p.Damage;
                    }
                }

                else
                {
                    total += p.Damage;
                }
            }

            foreach (PlayerControl p in _Players.Values)
            {
                double percent;

                try
                {
                    percent = (double)((double)(p.Damage - total) / total) + 1;
                }

                catch
                {
                    percent = 0;
                }

                if (_GroupOnly)
                {
                    if (p.GroupMember)
                    {
                        p.DamagePercent = percent;
                    }

                    else
                    {
                        p.DamagePercent = 0;
                    }
                }

                else
                {
                    p.DamagePercent = percent;
                }
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
                    if (p.PlayerName != _YouAlias)
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
                if (!PlayerPanel.Children.Contains(p) && !_HideOthers)
                {
                    if (_GroupOnly && !p.GroupMember)
                    {
                        continue;
                    }

                    PlayerPanel.Children.Add(p);
                }
            }
        }

        private void HideAllButGroup()
        {
            foreach (PlayerControl p in _Players.Values)
            {
                if (PlayerPanel.Children.Contains(p))
                {
                    if (p.PlayerName != _YouAlias && !p.GroupMember)
                    {
                        PlayerPanel.Children.Remove(p);
                    }
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
            if (_HideOthers)
            {
                return;
            }

            PlayerPanel.Children.Clear();

            var players = (from player in _Players orderby player.Value.PlayerName ascending select player.Value);

            foreach (PlayerControl player in players)
            {
                if (_GroupOnly)
                {
                    if (player.GroupMember)
                    {
                        PlayerPanel.Children.Add(player);
                    }
                }

                else
                {
                    PlayerPanel.Children.Add(player);
                }
            }

            _LastSortType = PlayerSortType.Name;
        }

        private void SortByDamage()
        {
            if (_HideOthers)
            {
                return;
            }

            PlayerPanel.Children.Clear();

            var players = (from player in _Players orderby player.Value.Damage descending select player.Value);

            foreach (PlayerControl player in players)
            {
                if (_GroupOnly)
                {
                    if (player.GroupMember)
                    {
                        PlayerPanel.Children.Add(player);
                    }
                }

                else
                {
                    PlayerPanel.Children.Add(player);
                }
            }

            _LastSortType = PlayerSortType.Damage;
        }

        private void SortByLast()
        {
            switch (_LastSortType)
            {
                case PlayerSortType.Damage:
                    SortByDamage();
                    break;

                case PlayerSortType.Name:
                    SortByName();
                    break;

                default:
                    break;
            }
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void MenuItemHideOthers_Click(object sender, RoutedEventArgs e)
        {
            DoHideOthers();
        }

        private void MenuItemGroupOnly_Click(object sender, RoutedEventArgs e)
        {
            DoGroupOnly();
            UpdatePercents();
        }

        private void DoHideOthers()
        {
            _HideOthers = MenuItemHideOthers.IsChecked;

            if (_HideOthers)
            {
                HideOthers();
            }

            else
            {
                ShowOthers();
            }
        }

        private void DoGroupOnly()
        {
            _GroupOnly = MenuItemGroupOnly.IsChecked;

            if (_GroupOnly)
            {
                HideAllButGroup();
            }

            else
            {
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

            if (_WorkingPlayer != null)
            {
                MenuItemGroupMember.IsChecked = _WorkingPlayer.GroupMember;
            }
        }

        private void MenuItemCopyTop_Click(object sender, RoutedEventArgs e)
        {
            string result = String.Empty;

            foreach (object o in PlayerPanel.Children)
            {
                if (o is PlayerControl)
                {
                    PlayerControl p = (PlayerControl)o;
                    double percent = (double)p.DamagePercent * 100;
                    result += p.ToString() + " ";
                }
            }

            Clipboard.SetText(result);
        }

        private void MenuItemCopyYou_Click(object sender, RoutedEventArgs e)
        {
            if (_Players.ContainsKey(_YouAlias))
            {
                PlayerControl p = _Players[_YouAlias];
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

        protected void OnHideOthersChanged(object sender, EventArgs e)
        {
            DoHideOthers();
        }

        protected void OnGroupOnlyChanged(object sender, EventArgs e)
        {
            DoGroupOnly();
            UpdatePercents();
        }

        private void MenuItemGroupMember_Click(object sender, RoutedEventArgs e)
        {
            if (_WorkingPlayer != null)
            {
                if (_WorkingPlayer.PlayerName != _YouAlias)
                {
                    _WorkingPlayer.GroupMember = MenuItemGroupMember.IsChecked;
                }

                else
                {
                    _WorkingPlayer.GroupMember = true;
                }
            }

            DoGroupOnly();
            UpdatePercents();
        }

        private void MenuItemAddGroupMemberByName_Click(object sender, RoutedEventArgs e)
        {
            AddToGroupDialog d = new AddToGroupDialog();

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddGroupMember(d.PlayerName);
            }
        }

        private void MenuItemResetDamage_Click(object sender, RoutedEventArgs e)
        {
            ResetDamage();
        }
    }
}
