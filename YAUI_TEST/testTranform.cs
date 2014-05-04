
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testTransform : iTestInstance
    {   
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "transform";
        }

        public string desc()
        {
            return
@"
    1. 左键拖拽图形;
    2. 按住左键，滚轮缩放;
    3. 按住右键，滚轮旋转;
    4. 空白处也可transform(map space);
";
        }

        int timeID = -1;
        public UIWidget getAttach()
        {
            return UI.Instance.fromXML(@"
<stub>
    <rect location='100' color='red' editMode='*dragAble, rotateAble, scaleAble'></rect>
    <round location='200, 100' color='green'></round>
    <round_rect location='100, 200' color='blue'></round_rect>
    <triangle location='200' color='maroon'></triangle>
    <label location='128' color='gold' text='drag me!' size='30'></label>
</stub>
            ", false);
        }

        public void lostAttach()
        {
            
        }
    }
}
