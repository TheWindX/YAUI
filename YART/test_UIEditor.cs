using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class test_UIEditor : Singleton<test_UIEditor>
    {
        public test_UIEditor()
        {
            init();
        }
        
        public void init()
        {
            var editor = new UIEditor();
            editor.pushPage("main");
            editor.pushPage("sub");
            editor.addMenu("test1");
            editor.addMenuItem("test1", "cmd11", () => Console.WriteLine("cmd11 is picked"));
            editor.addMenu("test2");
            editor.addMenuItem("test2", "cmd21", () => Console.WriteLine("cmd21 is picked"));
            editor.paresent = UIRoot.Instance.root;
        }
    }
}
