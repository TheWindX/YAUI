using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testRoundRect : Singleton<testRoundRect>
    {
        public testRoundRect()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble='true' scaleAble='true' width='256' height='256' layout='horizon' paddingX='4' paddingY='4'>
        <innerTemplate name='t1'>
            <round dragAble='true' width='64' height='64' leftTopCorner='false' fillColor='0xffff0000' marginX='2' marginY='2' layout='horizon'>            </round>
        </innerTemplate>
        <apply innerTemplate='t1' height='128' r2.height='55'>
               <rect name='r2' width='30' height='20' marginX='2' marginY='2'>
               </rect>
        </apply>
        <apply innerTemplate='t1' height='64'></apply>
        <apply innerTemplate='t1' height='32'></apply>
    </rect>
");
            

        }
    }
}
