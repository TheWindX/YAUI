using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayoutShrinkExpand : Singleton<testLayoutShrinkExpand>
    {
        public testLayoutShrinkExpand()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect name='r1' shrink='true' size='512' padding='3' clip='true' layout='vertical' layoutInverse='*true'>
        <rect shrink='false' height='128' expandX='true' layout='horizon' padding='6' margin='3' layoutFilled='true'>
            <rect expandX='NA' expandY='true' size='64' margin='2'></rect>
            <rect text='5678'></rect>
        </rect>
        <rect height='60' layout='horizon'></rect>
        <rect></rect>
        <rect></rect>
    </rect>
");

        }
    }
}
