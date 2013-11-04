using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class testUIRoot1 : Singleton<testUIRoot1>
    {

        public testUIRoot1()
        {
            GlobalInit.Instance.evtOnInit += main;
        }

        UIRect rc;

        Point moveStart;
        Point rcStart;
        
        bool onDragStart(int x, int y)
        {
            Console.WriteLine("onDragStart:" + x + "," + y);
        
            var pt = new Point(x, y);
            rc.mParesent.Abs2Local(ref pt);

            moveStart = pt;

            rcStart = rc.mPos;

            GlobalInit.Instance.mPainter.evtMove += onMove;
            return false;
        }

        bool onDragEnd(int x, int y)
        {
            Console.WriteLine("onDragEnd:" + x + "," + y);
            
            GlobalInit.Instance.mPainter.evtMove -= onMove;
            return false;
        }

        void onMove(int x, int y)
        {
            Console.WriteLine("onMove:" + x + "," + rc.py);
            Point pt = new Point(x, y);
            rc.mParesent.Abs2Local(ref pt);

            rc.px = rcStart.X + pt.X - moveStart.X;
            rc.py = rcStart.Y + pt.Y - moveStart.Y;

            Console.WriteLine("onMove:" + rc.px + "," + rc.py);
        }

        public void main()
        {
            GlobalInit.Instance.mPainter.evtPaint += (g) =>
            {
                UIRoot.Instance.draw(g);
            };
            GlobalInit.Instance.mPainter.evtLeftDown += (x, y) =>
            {
                UIRoot.Instance.testLMD(x, y);
            };
            GlobalInit.Instance.mPainter.evtLeftUp += (x, y) =>
            {
                UIRoot.Instance.testLMU(x, y);
            };
            GlobalInit.Instance.mPainter.evtRightDown += (x, y) =>
            {
                UIRoot.Instance.testRMD(x, y);
            };
            GlobalInit.Instance.mPainter.evtRightUp += (x, y) =>
            {
                UIRoot.Instance.testRMU(x, y);
            };
            GlobalInit.Instance.mPainter.evtMidDown += (x, y) =>
            {
                UIRoot.Instance.testMMD(x, y);
            };
            GlobalInit.Instance.mPainter.evtMidUp += (x, y) =>
            {
                UIRoot.Instance.testMMU(x, y);
            };

            UIRoot.Instance.init();

            UIRoot.Instance.mRoot.mScalex = 4;
            UIRoot.Instance.mRoot.mScaley = 4;
            UIRoot.Instance.mRoot.px = 20;
            UIRoot.Instance.mRoot.py = 20;
            rc = new UIRect(128, 64);
            rc.setParesent(UIRoot.Instance.mRoot);


            rc.evtOnLMDown += onDragStart;
            rc.evtOnLMUp += onDragEnd;


            Point orgPt = new Point();
            PaintDriver.EvtOnWheel wheel = (delta) =>
                {
                    float sc = 1;
                    if(delta>0) sc = 1.1f;
                    else sc = 0.9f;
                    UIRoot.Instance.mRoot.scalePoint(orgPt, sc);
                };

            UIWidget.EvtOnLMDown leftDown = (x, y) =>
                {
                    orgPt = new Point(x, y);
                    GlobalInit.Instance.mPainter.evtOnWheel += wheel;
                    return true;
                };

            UIWidget.EvtOnLMUp leftUp = (x, y) =>
            {
                GlobalInit.Instance.mPainter.evtOnWheel -= wheel;
                return true;
            };


            UIRoot.Instance.mRoot.evtOnLMDown += leftDown;
            UIRoot.Instance.mRoot.evtOnLMUp += leftUp;
        }
    }
}
