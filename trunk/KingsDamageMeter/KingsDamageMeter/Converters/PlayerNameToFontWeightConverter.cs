using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KingsDamageMeter.Languages;

namespace KingsDamageMeter.Converters
{
    public class PlayerNameToFontWeightConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if(((string)value) == Regex.Default.YouAlias)
                {
                    return FontWeights.Bold;
                }
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}