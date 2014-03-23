using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YART
{
    class UIPackageTemplate
    {
        static string XML = @"
<stub layout=""vertical"" dragAble=""true"">
    <round_rect width=""512"" height=""48"" radius=""5"" leftBottomCorner=""false"" rightBottomCorner=""false"">
    </round_rect>
    <rect width=""512"" height=""512"" >
    </rect>
</stub>
";
        public static UIWidget genWidget()
        {
            var ret = UIRoot.Instance.loadFromXML(XML);
            ret.paresent = UIRoot.Instance.root;
            return ret;
        }
    }
}
