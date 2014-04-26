using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testTips : Singleton<testTips>
    {
        public testTips()
        {
            //UI.Instance.getTip().foreground = (uint)EColorUtil.red;
            //UI.Instance.getTip().background = (uint)EColorUtil.black;
            //UI.Instance.getTip().size = 20;
            UI.Instance.setTitle("Left click to toggle tips");

            Func<UIWidget, int, int, bool> mh = (ui, x, y)=>
                {
                    UI.Instance.setTips("position at:"+UI.Instance.getCursorPosition() );
                    return false;
                };
            bool toggleState = true;
            mh(null, 0, 0);
            UI.Instance.root.evtOnLMUp += (ui, x, y) =>
                {
                    toggleState = !toggleState;
                    if(toggleState)
                    {
                        mh(null, 0, 0);
                        UI.Instance.root.evtOnMMove += mh;
                    }
                    else
                    {
                        UI.Instance.setTips(null);
                        UI.Instance.flush();
                        UI.Instance.root.evtOnMMove -= mh;
                    }
                    return false;
                };

        }
    }
}
