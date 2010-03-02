using System.Windows.Input;

namespace KingsDamageMeter.Controls
{
    public static class Commands
    {
        public static RoutedCommand ClearAllCommand { get; private set; }
        public static RoutedCommand ResetCountsCommand { get; private set; }
        public static RoutedCommand RemovePlayerCommand { get; private set; }
        public static RoutedCommand IgnorePlayerCommand { get; private set; }

        static Commands()
        {
            ClearAllCommand = new RoutedCommand();
            ResetCountsCommand = new RoutedCommand();
            RemovePlayerCommand = new RoutedCommand();
            IgnorePlayerCommand = new RoutedCommand();
        }
    }
}
