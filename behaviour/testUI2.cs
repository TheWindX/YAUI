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
            UIRoot.Instance.mRoot.mScalex = 2;
            UIRoot.Instance.mRoot.mScaley = 2;
            UIRoot.Instance.mRoot.px = 20;
            UIRoot.Instance.mRoot.py = 20;
            UIRoot.Instance.mRoot.dragAble = true;
            rc = new UIRect(128, 64, 0x88333333, 0x33333333);
            rc.setParesent(UIRoot.Instance.mRoot);
            rc.bClip = true;
            rc.px = 64;
            rc.py = 64;
            rc.bEnable = true;

            rc1 = new UIRect(128, 64);
            rc1.mPos.X = -20;
            rc1.mPos.Y = -20;
            rc1.setParesent(rc);
            rc1.dragAble = true;
            rc1.bClip = true;

            var rc2 = new UIRect(64, 128);
            rc2.setParesent(rc1);
            rc2.dragAble = true;

            var t1 = new UIString("lable test");
            t1.dragAble = true;
            t1.setParesent(rc1);
            t1.depth = -1;


            var tri = new UITriangle(32, 32);
            tri.setParesent(rc);
            tri.dragAble = true;

            Point orgPt = new Point();
            PaintDriver.EvtOnWheel wheel = (delta) =>
            {
                float sc = 1;
                if (delta > 0) sc = 1.1f;
                else sc = 0.9f;
                UIRoot.Instance.mRoot.scalePoint(orgPt, sc);
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


            UIRoot.Instance.mRoot.evtOnLMDown += leftDown;
            UIRoot.Instance.mRoot.evtOnLMUp += leftUp;
        }
    }
}
