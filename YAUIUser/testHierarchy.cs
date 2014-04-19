using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;
    class testHierarchy : Singleton<testHierarchy>
    {
        public testHierarchy()
        {
            UI.Instance.fromXML(@"
<rect length='512' dragAble='*true' clip='*true' color='0xff87ceeb'>
    <round_rect length='256' color='0xffafeeee'>
        <round length='128' color='0xff6b8e23'>
        </round>
    </round_rect>
</rect>
");
        }
    }
}
