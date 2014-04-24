using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testTransform : Singleton<testTransform>
    {
        public testTransform()
        {
            UI.Instance.setTitle(@"1. drag all these
2. left botton down and scrollwheel to scale
3. right botton down and scrollwheel to rotate
");

            UI.Instance.fromXML(@"
<map location='256' dragAble='*true' rotateAble='*true' scaleAble='*true'>
    <rect color='red'></rect>
    <round color='green'></round>
    <round_rect color='blue'></round_rect>
    <triangle color='maroon'></triangle>
    <lable text='drag me!'></lable>
</map>
            ");
        }
    }
}
