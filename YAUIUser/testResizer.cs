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
            UIRoot.Instance.root.appendFromXML(@"
<rect shrink='true' clip='*true' padding='5' fillColor='0xff3e4649' layout='vertical'>
    <lable align='leftTop'></lable>
    <lable align='rightTop' text='x'></lable>
    <blank size='30'></blank>
    <resizer size='256'> </resizer>
</rect>
");
        }
    }
}
