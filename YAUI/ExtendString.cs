using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUI
{
    public static class ExtendString
    {
        public static int castInt(this string _this, int defaultVal = 0)
        {
            Int32.TryParse(_this, out defaultVal);
            return defaultVal;
        }

        public static uint castHex(this string _this, uint defaultVal = 0)
        {
            try
            {
                string str = _this;
                if (str.StartsWith("0x") || str.StartsWith("0X")) str = str.Substring(2);
                defaultVal = uint.Parse(str, NumberStyles.AllowHexSpecifier);
            }
            catch { }
            return defaultVal;
        }


        public static float castFloat(this string _this, float defaultVal = 0.0f)
        {
            float.TryParse(_this, out defaultVal);
            return defaultVal;
        }

        public static bool castBool(this string _this, bool defaultVal = false)
        {
            bool.TryParse(_this, out defaultVal);
            return defaultVal;
        }
    }

}
