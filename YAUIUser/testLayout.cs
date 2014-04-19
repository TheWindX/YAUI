using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testLayout : Singleton<testLayout>
    {
        const string XMLPAGE = @"
<resizer name='root' length='256' layout='horizon' layoutInverse='true' layoutFilled='true' expandY='true'>
    <rect name='split' width='8' expandY='true' color='red'></rect>
    <rect layout='vertical' layoutFilled='true' color='yellow'>
        <rect name='tab' color='orange' width='128' height='24'>
            <lable name='tabName' text='tab' align='center'></lable>
        </rect>
        <rect name='client' color='blue'></rect>
    </rect>
</resizer>
";
        public testLayout()
        {
            UIRoot.Instance.root.appendFromXML(XMLPAGE);

        }
    }
}
