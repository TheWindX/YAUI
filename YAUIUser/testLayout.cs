using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testLayout : Singleton<testLayout>
    {
        public testLayout()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect name='r1' length='512' layout='vertical' padding='12' clip='true'>
        <rect height='128' layout='horizon' padding='6' margin='3'>
            <rect length='64' margin='2'></rect>
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
