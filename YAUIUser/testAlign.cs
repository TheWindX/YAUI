using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testAlign : Singleton<testAlign>
    {
        public testAlign()
        {
            UI.Instance.fromXML(@"
    <resizer location='128' size='384' padding='8' editMode='dragAble'>
        <lable text='leftTop' align='leftTop'></lable>
        <lable text='top' align='top'></lable>
        <lable text='rightTop' align='rightTop'></lable>
        <lable text='left' align='left'></lable>
        <lable text='center' align='center'></lable>
        <lable text='right' align='right'></lable>
        <lable text='leftBottom' align='leftBottom'></lable>
        <lable text='bottom' align='bottom'></lable>
        <lable text='rightBottom' align='rightBottom'></lable>
    </resizer>

");
        }
    }
}
