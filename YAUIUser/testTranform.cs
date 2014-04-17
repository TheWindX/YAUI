using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace YAUIUser
{
    class testTransform : Singleton<testTransform>
    {
        public testTransform()
        {
            UI.Instance.fromXML(@"
<map dragAble='*true' rotateAble='*true' scaleAble='*true'>
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
