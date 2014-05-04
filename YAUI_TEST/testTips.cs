using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    class testTips : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "tips";
        }

        public string desc()
        {
            return
@"
    left click to toggle tips
";
        }


        bool moveHandler(UIWidget ui, int x, int y)
            {
                UI.Instance.setTips("position at:" + UI.Instance.getCursorPosition());
                return false;
            }

        bool toggleState = false;
        bool toggle(UIWidget ui, int x, int y)
       
            {
                toggleState = !toggleState;
                if (toggleState)
                {
                    moveHandler(null, 0, 0);
                    UI.Instance.root.evtOnMMove += moveHandler;
                }
                else
                {
                    UI.Instance.setTips(null);
                    UI.Instance.flush();
                    UI.Instance.root.evtOnMMove -= moveHandler;
                }
                return false;
            }
        public UIWidget getAttach()
        {
            toggle(null, 0, 0);
            UI.Instance.root.evtOnLMUp += toggle;
            return UI.Instance.fromXML(@"
<stub dragAble='*true'>
</stub>", false);
            //UI.Instance.getTip().foreground = (uint)EColorUtil.red;
            //UI.Instance.getTip().background = (uint)EColorUtil.black;
            //UI.Instance.getTip().size = 20;
        }

        public void lostAttach()
        {
            UI.Instance.root.evtOnLMUp -= toggle;
            UI.Instance.root.evtOnMMove -= moveHandler;
            UI.Instance.setTips(null);
        }
    }

}

    