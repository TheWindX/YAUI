using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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


        void moveHandler(int x, int y)
            {
                UI.Instance.setTips("position at:" + UI.Instance.getCursorPosition());
                //return false;
            }

        bool toggleState = false;
        void toggle(int x, int y)
       
            {
                toggleState = !toggleState;
                if (toggleState)
                {
                    moveHandler(0, 0);
                    UIRoot.Instance.evtMove += moveHandler;
                }
                else
                {
                    UI.Instance.setTips(null);
                    UI.Instance.flush();
                    UIRoot.Instance.evtMove -= moveHandler;
                }
                //return false;
            }
        public UIWidget getAttach()
        {
            toggle(0, 0);
            UIRoot.Instance.evtLeftDown += toggle;
            return UI.Instance.fromXML(@"
<stub dragAble='*true'>
</stub>", false);
        }

        public void lostAttach()
        {
            UIRoot.Instance.evtLeftDown -= toggle;
            UIRoot.Instance.evtMove -= moveHandler;
            UI.Instance.setTips(null);
            UIRoot.Instance.evtMove -= moveHandler;
            UIRoot.Instance.evtLeftDown -= toggle;
        }
    }

}

    