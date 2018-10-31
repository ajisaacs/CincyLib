using System;
using System.Xml.Linq;

namespace CincyLib.PressBrake
{
    static class Extensions
    {
        public static bool ToBool(this XAttribute a, bool defaultValue = false)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            int intValue;

            if (!int.TryParse(a.Value, out intValue))
                return defaultValue;

            return Convert.ToBoolean(intValue);
        }

        public static int ToInt(this XAttribute a, int defaultValue = 0)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            int intValue;

            if (!int.TryParse(a.Value, out intValue))
                return defaultValue;

            return intValue;
        }

        public static double ToDouble(this XAttribute a, double defaultValue = 0)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            double d;

            if (!double.TryParse(a.Value, out d))
                return defaultValue;

            return d;
        }
    }
}
