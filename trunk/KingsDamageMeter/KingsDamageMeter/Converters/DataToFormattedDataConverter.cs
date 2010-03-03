using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Controls;

namespace KingsDamageMeter.Converters
{
    public class DataToFormattedDataConverter : IValueConverter
    {
        public static DataToFormattedDataConverter Instance { get; private set; }
        static DataToFormattedDataConverter()
        {
            Instance = new DataToFormattedDataConverter();
        }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is DisplayType)
            {
                switch ((DisplayType)parameter)
                {
                    case DisplayType.Damage:
                    case DisplayType.DamagePerSecond:
                    case DisplayType.Experience:
                    case DisplayType.Kinah:
                    case DisplayType.AbyssPoints:
                        return System.Convert.ToDouble(value).ToString("#,#");
                    case DisplayType.Percent:
                        return System.Convert.ToDouble(value).ToString("0%");
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}