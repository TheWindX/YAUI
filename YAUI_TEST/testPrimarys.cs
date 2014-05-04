using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    class testPrimarys : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "primary";
        }

        public string desc()
        {
            return
@"
    基本控件
";
        }

        public UIWidget getAttach()
        {
            return UI.Instance.fromXML(@"
<stub dragAble='*true'>
    <rect location='100' color='red'></rect>
    <round location='200, 100' color='green'></round>
    <round_rect location='100, 200' color='blue'></round_rect>
    <triangle location='200' color='maroon'></triangle>
</stub>", false);
        }

        public void lostAttach()
        {
            //throw new NotImplementedException();
        }
    }
   
}
