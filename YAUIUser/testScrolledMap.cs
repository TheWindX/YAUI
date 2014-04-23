using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testScrolledMap : Singleton<testScrolledMap>
    {
        public testScrolledMap()
        {
            UI.Instance.fromXML(@"
<rect location='128, 128' color='black' dragAble='true' shrink='true' padding='5' layout='vertical'>
    <blank size='32'></blank>
    <resizer size='384'>
        <scrolledMap expand='true'>
            <rect dragAble='*true' color='red' location='128, 128'></rect>
            <rect color='blue' location='384, 384'></rect>
        </scrolledMap>
    </resizer>
</rect>");
        }
    }
}
