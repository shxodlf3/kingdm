using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Controls;

namespace KingsDamageMeter.Converters
{
    public class DisplayTypeToBoolConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayType && parameter is DisplayType)
            {
                return (DisplayType)value == (DisplayType)parameter;

            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && parameter is DisplayType)
            {
                if((bool)value)
                {
                    return (DisplayType)parameter;
                }
            }
            return DisplayType.Damage;
        }

        #endregion
    }
}