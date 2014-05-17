using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
using ns_YAUtils;
namespace ns_YAUIUser
{
    class testLine1 : Singleton<testLine1>
    {
        const string XMLLayout = @"
<line begin='30, 30' end = '130, 130' color='red'></line>
";
        public testLine1()
        {
            UI.Instance.fromXML(XMLLayout);
        }
    }
}
