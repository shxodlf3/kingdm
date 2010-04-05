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

using KingsDamageMeter.Enums;

namespace KingsDamageMeter.Controls
{
    public static class Commands
    {
        public static RelayCommand<EncounterBase> RemoveEncounterCommand { get; set; }
        public static ObjectRelayCommand RemoveAllEncountersCommand { get; set; }
        public static RelayCommand<Player> RemovePlayerCommand { get; set; }
        public static RelayCommand<Player> IgnorePlayerCommand { get; set; }
        public static RelayCommand<Player> CopySelectedToClipboardCommand { get; set; }
        public static RelayCommand<ClipboardCopyType> CopyToClipboardCommand { get; set; }
        public static RelayCommand<Player> IsGroupMemberChangedCommand { get; set; }
        public static RelayCommand<Player> IsFriendChangedCommand { get; set; }
    }
}
