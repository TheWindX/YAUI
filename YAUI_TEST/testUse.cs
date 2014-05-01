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
                var ui = UI.Instance.fromXML(@"
<scrolledMap>
    <div layout='horizon'>
        <edit width='128' rows='3'></edit>
        <!--label name='lb' link='true' text='下asdfasdfasdf划线，链asdfasdf接asdfasdfasdf文字控件，serasdfasdf' style='bold' color='red' maxLineWidth='256' size='12'></label-->
    </div>
</scrolledMap>
");   
                var lb = ui.findByPath("lb") as UILabel;
                //lb.link = true;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine(e.LineNumber);
                //Console.WriteLine(e.LinePosition);
            }
        }
    }
}
