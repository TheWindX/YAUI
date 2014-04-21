using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    class testMenu : Singleton<testMenu>
    {
        public testMenu()
        {
            UIMenu m = new UIMenu();
            //m.itemWidth = 64;
            //m.itemHeight = 20;
            m.addItem("item1", () => { Console.WriteLine("item1"); m.visible = false; m.setDirty(true); });
            m.addItem("item2", () => { Console.WriteLine("item2"); m.visible = false; m.setDirty(true); });
            m.paresent = UI.Instance.root;
            UI.Instance.root.evtOnRMUp += (ui, x, y) =>
                {
                    m.visible = true;
                    m.setDirty(true);
                    m.position = UI.Instance.getCursorPosition();
                    return false;
                };
            UI.Instance.root.evtOnLMDown += (ui, x, y) =>
            {
                m.visible = false;
                m.setDirty(true);
                return false;
            };
        }
    }
}
