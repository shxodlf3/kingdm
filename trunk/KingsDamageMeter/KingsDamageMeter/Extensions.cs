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
