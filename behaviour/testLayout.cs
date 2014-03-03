/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class testLayout : Singleton<testLayout>
    {
        public testLayout()
        {
            Globals.Instance.evtOnInit += main;
        }

        public void main()
        {
            var rc = new UIRect(512, 128);
            rc.paddingX = 0;
            rc.paddingY = 100;
            rc.position = new Point(100, 100);
            rc.dragAble = true;
            //rc.mDir = 45;
            rc.paresent = UIRoot.Instance.root;
            rc.mLayout = UIWidget.ELayout.vertical;
            //rc.resizeAble = true;
            rc.wrap = true;

            var rc1 = new UIRect(128, 128);
            rc1.marginX = 50;
            rc1.marginY = 10;
            rc1.paresent = rc;

            var rc2 = new UIRect(128, 128);
            rc2.marginX = 50;
            rc2.marginY = 10;
            rc2.paresent = rc;

            var rc3 = new UIRect(128, 128);
            rc3.marginX = 50;
            rc3.marginY = 10;
            rc3.paresent = rc;

            var rc4 = new UIRect(128, 128);
            rc4.marginX = 50;
            rc4.marginY = 10;
            rc4.paresent = rc;
        }
    }
}
