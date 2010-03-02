using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Converters
{
    public class DetectYouConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if(((string)value) == Settings.Default.YouAlias)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}