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
