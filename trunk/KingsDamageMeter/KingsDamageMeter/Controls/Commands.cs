namespace KingsDamageMeter.Controls
{
    public static class Commands
    {
        public static ObjectRelayCommand ClearAllCommand { get; set; }
        public static ObjectRelayCommand ResetCountsCommand { get; set; }
        public static RelayCommand<Player> RemovePlayerCommand { get; set; }
        public static RelayCommand<Player> IgnorePlayerCommand { get; set; }
    }
}
