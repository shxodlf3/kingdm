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

using System;
using System.Globalization;
using System.Windows.Data;
using KingsDamageMeter.Enums;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Converters
{
    public class DisplayTypeToBoolConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayType && parameter is DisplayType)
            {
                return ((DisplayType)value & (DisplayType)parameter) == (DisplayType)parameter;

            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && parameter is DisplayType)
            {
                if((bool)value)
                {
                    DisplayType result = Settings.Default.DisplayType;
                    result |= (DisplayType)parameter;
                    return result;
                }
                else
                {
                    DisplayType result = Settings.Default.DisplayType;
                    result &= ~(DisplayType)parameter;
                    return result;
                }
            }
            return DisplayType.Damage;
        }

        #endregion
    }
}