using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml;

namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class testScrolledMap : Singleton<testScrolledMap>
    {
        public testScrolledMap()
        {
            UI.Instance.fromXML(@"
<rect color='black' dragAble='true' shrink='true' padding='64' layout='vertical'>
    <resizer size='384'>
        <scrolledMap scaleAble='true' expand='true'>
            <rect dragAble='*true' scaleAble='true' color='red' location='128, 128'></rect>
            <rect color='blue' location='384, 384'></rect>
        </scrolledMap>
    </resizer>
</rect>");
        }
    }
}
