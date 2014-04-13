using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testUse : Singleton<testUse>
    {
        const string XMLPAGE = @"

    <rect length='512' layout='vertical' layoutFilled='true'>
        <blank debugName='tab' length='32' layout='horizon' layoutFilled='true' layoutInverse='true' expandX='true'>
        </blank>
        <rect debugName='cmd' name='commandCtn' expandX='true' height='64'></rect>
        <blank debugName='client' name='clients' layout='horizon' layoutFilled='true'></blank>
    </rect>

";
        public testUse()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);
        }
    }
}
