using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Controls;

namespace KingsDamageMeter.Converters
{
    public class DisplayTypeToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayType && parameter is DisplayType)
            {
                if (((DisplayType)value & (DisplayType)parameter) == (DisplayType)parameter)
                {
                    return Visibility.Visible;
                }

            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}