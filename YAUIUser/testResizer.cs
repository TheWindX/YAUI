using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
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
<rect shrink='true' clip='*true' padding='5' fillColor='red' layout='vertical'>
    <lable align='leftTop'></lable>
    <lable align='rightTop' text='x'></lable>
    <blank length='30'></blank>
    <resizer length='256' clip='true'></resizer>
</rect>
") as UIResizer;
        }
    }
}
