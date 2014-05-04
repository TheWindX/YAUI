
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    class testMenu : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "menu";
        }

        public string desc()
        {
            return
@"
right click to rise a context menu
";
        }


        bool leftHandler(UIWidget ui, int x, int y)
        {
            mMenu.visible = false;
            mMenu.setDirty(true);
            return false;
        }

        bool rightHandler(UIWidget ui, int x, int y)
        {
            mMenu.visible = true;
            mMenu.setDepthHead();
            
            var pos = UI.Instance.getCursorPosition();
            mMenu.px = pos.X;
            mMenu.py = pos.Y;
            mMenu.selectName("item2");
            mMenu.setDirty(true);
            return false;
        }

        UIMenu mMenu = null;
        public UIWidget getAttach()
        {
            mMenu = new UIMenu();
            //m.itemWidth = 64;
            //m.itemHeight = 20;
            mMenu.addItem("item1", () => { Console.WriteLine("click item1"); mMenu.visible = false; mMenu.setDirty(true); });
            mMenu.addItem("item2", () => { Console.WriteLine("click item2"); mMenu.visible = false; mMenu.setDirty(true); });
            mMenu.paresent = UI.Instance.root;
            UI.Instance.root.evtOnRMUp += rightHandler;
            UI.Instance.root.evtOnLMDown += leftHandler;

            rightHandler(null, 0, 0);
            return UI.Instance.fromXML(@"
<stub dragAble='*true'>
</stub>", false);
            //UI.Instance.getTip().foreground = (uint)EColorUtil.red;
            //UI.Instance.getTip().background = (uint)EColorUtil.black;
            //UI.Instance.getTip().size = 20;
        }

        public void lostAttach()
        {
            mMenu = null;
            UI.Instance.root.evtOnRMUp -= rightHandler;
            UI.Instance.root.evtOnLMDown -= leftHandler;
        }
    }

}

