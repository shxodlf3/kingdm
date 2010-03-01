using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Controls;

namespace KingsDamageMeter.Converters
{
    public class PlayerSortTypeToBoolConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is PlayerSortType && parameter is PlayerSortType)
            {
                return (PlayerSortType) value == (PlayerSortType) parameter;

            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool && parameter is PlayerSortType)
            {
                if((bool)value)
                {
                    return (PlayerSortType) parameter;
                }
            }
            return PlayerSortType.None;
        }

        #endregion
    }
}
