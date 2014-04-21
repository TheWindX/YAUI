using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testUse : Singleton<testUse>
    {
        public testUse()
        {
            try
            {
                UI.Instance.fromXML(@"
<round_rect shrink='true' paddingX='5' paddingY='2' dragAble='true'>
    <lable text='asdf' style='normal'></lable>
</round_rect>
");
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.LineNumber);
                Console.WriteLine(e.LinePosition);
            }
        }
    }
}
