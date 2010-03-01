using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Converters
{
    public class IsLanguageSelectedConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is CultureInfo && parameter is CultureInfo)
            {
                return value.Equals(parameter);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if((bool)value)
                {
                    return parameter;
                }
            }
            return Settings.Default.SelectedLanguage;
        }

        #endregion
    }
}