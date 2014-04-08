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
<stub layout='vertical' dragAble='true'>
    <rect width='768' height='40' radius='12' leftBottomCorner='false' rightBottomCorner='false'
     fillColor='ff4b86d4'>
    </rect>
    <rect width='768' height='1024' fillColor='0xffdee8f2'>
        <rect width='768' height='80' name=''></rect>
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
