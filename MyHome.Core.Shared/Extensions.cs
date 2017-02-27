using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{
    public static class Extensions
    {
        public static long AsLong(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

            var s = "";
            foreach(var c in value)
            {
                if ("0123456789".IndexOf(c) != -1)
                {
                    s += c;
                }
            }

            long l;
            if (long.TryParse(s, out l))
            {
                return l;
            }

            return 0;
        }

        public static int AsInt(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

            var s = "";
            foreach (var c in value)
            {
                if ("0123456789".IndexOf(c) != -1)
                {
                    s += c;
                }
            }

            int l;
            if (int.TryParse(s, out l))
            {
                return l;
            }

            return 0;
        }

        public static decimal AsDecimal(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

            var s = "";
            foreach (var c in value)
            {
                if ("0123456789.,".IndexOf(c) != -1)
                {
                    s += c;
                }
            }
            if (value[0] == '-')
            {
                s = "-" + s;
            }

            s = s.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            while (s.IndexOf("..") != -1)
                s = s.Replace("..", ".");
            while (s.IndexOf(",,") != -1)
                s = s.Replace(",,", ",");

            decimal d;
            if (decimal.TryParse(s, out d))
            {
                return d;
            }

            return 0;
        }
    }
}
