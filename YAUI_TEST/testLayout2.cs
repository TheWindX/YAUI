using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayoutInverse : Singleton<testLayoutInverse>
    {
        public testLayoutInverse()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble='true' scaleAble='true' size='512' shrink='true' layout='horizon' layoutInverse='true' margin='*6'>
        <rect margin='3' width='*64' height='*64' padding='3' expandY='true'></rect>
        <rect ></rect>
        <rect ></rect>
    </rect>
");
        }
    }
}
