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
                <blank name='tabs' height='48' layout='horizon'>
                    <rect width='*128' marginX='*1' expandY='*true'>
                    </rect>
                    <rect>
                    </rect>
                </blank>
";
        public testUse()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);
        }
    }
}
