using System;

namespace KingsDamageMeter.Controls
{
    public static class Format
    {
        private static string _ShortFormat = "%n %d";
        private static string _LongFormat = "%n %d (%s dps)(%p)";

        public static string Short
        {
            get
            {
                return _ShortFormat;
            }

            set
            {
                _ShortFormat = value;
            }
        }

        public static string Long
        {
            get
            {
                return _LongFormat;
            }

            set
            {
                _LongFormat = value;
            }
        }

        public static string Player(string name, string damage, string dps, string percent, PlayerFormatOptions options)
        {
            string format;

            switch (options)
            {
                case PlayerFormatOptions.Short:
                    format = _ShortFormat;
                    break;

                case PlayerFormatOptions.Long:
                    format = _LongFormat;
                    break;

                default:
                    format = _ShortFormat;
                    break;
            }

            format = format.Replace("%n", name);
            format = format.Replace("%d", damage);
            format = format.Replace("%s", dps);
            format = format.Replace("%p", percent);

            return format;
        }
    }
}
