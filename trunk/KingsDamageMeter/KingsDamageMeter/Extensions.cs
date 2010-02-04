﻿/**************************************************************************\
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

namespace KingsDamageMeter
{
    public static class Extensions
    {
        public static int GetNumber(this string expression)
        {
            if (String.IsNullOrEmpty(expression))
            {
                return 0;
            }

            char c;
            string result = String.Empty;

            for (int i = 0; i < expression.Length; i++)
            {
                c = Convert.ToChar(expression.Substring(i, 1));

                if (Char.IsNumber(c))
                {
                    result += c;
                }
            }

            if (result.Length > 0)
            {
                return Convert.ToInt32(result);
            }

            else
            {
                return 0;
            }
        }
    }
}
