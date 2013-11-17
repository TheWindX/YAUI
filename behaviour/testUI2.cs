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
    class testUI2 : Singleton<testUI2>
    {

        public testUI2()
        {
            Globals.Instance.evtOnInit += main;
        }

        UIRect rc;
        UIRect rc1;


        public void main()
        {
            UIRoot.Instance.root.mScalex = 2;
            UIRoot.Instance.root.mScaley = 2;
            UIRoot.Instance.root.px = 20;
            UIRoot.Instance.root.py = 20;
            UIRoot.Instance.root.dragAble = true;
            rc = new UIRect(128, 64, 0x88333333, 0x33333333);
            rc.paresent = UIRoot.Instance.root;
            rc.clip = true;
            rc.px = 64;
            rc.py = 64;
            rc.enable = true;

            rc1 = new UIRect(128, 64);
            rc1.position.X = -20;
            rc1.position.Y = -20;
            rc1.paresent = rc;
            rc1.dragAble = true;
            rc1.clip = true;

            var rc2 = new UIRect(64, 128);
            rc2.paresent = rc1;
            rc2.dragAble = true;

            var t1 = new UILable("lable test");
            t1.dragAble = true;
            t1.paresent = rc1;
            t1.depth = -1;


            var tri = new UIArrow(32, 32);
            tri.paresent = rc;
            tri.dragAble = true;

            Point orgPt = new Point();
            PaintDriver.EvtOnWheel wheel = (delta) =>
            {
                float sc = 1;
                if (delta > 0) sc = 1.1f;
                else sc = 0.9f;
                UIRoot.Instance.root.scalePoint(orgPt, sc);
            };

            UIWidget.EvtMouse leftDown = (ui, x, y) =>
            {
                orgPt = new Point(x, y);
                Globals.Instance.mPainter.evtOnWheel += wheel;
                return true;
            };

            UIWidget.EvtMouse leftUp = (ui, x, y) =>
            {
                Globals.Instance.mPainter.evtOnWheel -= wheel;
                return true;
            };


            UIRoot.Instance.root.evtOnLMDown += leftDown;
            UIRoot.Instance.root.evtOnLMUp += leftUp;
        }
    }
}
