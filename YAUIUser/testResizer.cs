using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;
    class testResizer : ns_YAUI.Singleton<testResizer>
    {
        public testResizer()
        {
            UIResizer r = UIRoot.Instance.root.appendFromXML(@"
    <resizer length='512' color='0xffffffff' clip='true'>
    </resizer>
") as UIResizer;
            r.append(UIRoot.Instance.loadFromXML(@"
<rect noInheret='true' dragAble='true' length='128'></rect>
"));
        }
    }
}
