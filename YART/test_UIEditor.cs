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
            editor.pushPage("asdf");
            editor.pushPage("asdf1");
            editor.pushPage("asdf2");
            editor.pushPage("asdf3");
            editor.paresent = UIRoot.Instance.root;
        }
    }
}
