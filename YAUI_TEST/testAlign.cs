using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testAlign : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "align";
        }

        public string desc()
        {
            return
@"
1. 9个锚点;
2. offset， margin, padding 作用;
";
        }

        public UIWidget getAttach()
        {
            return UI.Instance.fromXML(@"
    <resizer size='384' padding='8' editMode='dragAble'>
        <label text='leftTop' align='leftTop'></label>
        <label text='top' align='top'></label>
        <label text='rightTop' align='rightTop'></label>
        <label text='left' align='left'></label>
        <label text='center' align='center'></label>
        <label text='right' align='right'></label>
        <label text='leftBottom' align='leftBottom'></label>
        <label text='bottom' align='bottom'></label>
        <label text='rightBottom' align='rightBottom'></label>
    </resizer>
", false);
        }

        public void lostAttach()
        {
            //throw new NotImplementedException();
        }
    }
}
