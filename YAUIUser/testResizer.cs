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
            UIRoot.Instance.root.rotateAble = true;
            UIRoot.Instance.root.dragAble = true;
            UIRoot.Instance.root.scaleAble = true;
            UIResizer r = UIRoot.Instance.root.appendFromXML(@"
    <resizer length='512' color='' clip='true'>
    </resizer>
") as UIResizer;
            r.append(UIRoot.Instance.loadFromXML(@"
<rect inherent='false' dragAble='true' length='128'></rect>
"));
        }
    }
}
