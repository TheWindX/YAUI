using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using ns_YAUtils;
namespace ns_YAUIUser
{
    using ns_YAUI;

    class testUse : Singleton<testUse>
    {
        public testUse()
        {
            try
            {
                const string XMLLayout = @"
<div location='30, 0' rectExclude='false' layout='horizon, shrink' dragAble='true'>
        <round_rect rectExclude='false' layout='shrink' padding='0'>
            <label text='template' name='label' margin='5'>
            </label>
            <round radius='8' align='left' alignParesent='right' name='subs' rectExclude='false'>
                <label text='+' align='center' offset='2'></label>
            </round>
        </round_rect>
</div>
";
        //UI.Instance.root.dragAble = false;
        var ui = UI.Instance.fromXML(XMLLayout);
        var subs = ui.findByPath("subs");
        
        var ui2 = UI.Instance.fromXML(XMLLayout, false);
        ui2.paresent = subs;
        
        UI.Instance.flush();
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
