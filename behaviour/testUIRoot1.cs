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
    class testUIRoot1 : Singleton<testUIRoot1>
    {
        public testUIRoot1()
        {
            Globals.Instance.evtOnInit += main;
        }

        UIRect rc;

        Point moveStart;
        Point rcStart;
        
        bool onDragStart(UIWidget ui, int x, int y)
        {
            Console.WriteLine("onDragStart:" + x + "," + y);
        
            var pt = new Point(x, y);
            pt = rc.paresent.invertTransformAbs(pt);

            moveStart = pt;

            rcStart = rc.mPos;

            Globals.Instance.mPainter.evtMove += onMove;
            return false;
        }

        bool onDragEnd(UIWidget ui, int x, int y)
        {
            Console.WriteLine("onDragEnd:" + x + "," + y);
            
            Globals.Instance.mPainter.evtMove -= onMove;
            return false;
        }

        void onMove(int x, int y)
        {
            Console.WriteLine("onMove:" + x + "," + rc.py);
            Point pt = new Point(x, y);
            pt = rc.paresent.invertTransformAbs(pt);

            rc.px = rcStart.X + pt.X - moveStart.X;
            rc.py = rcStart.Y + pt.Y - moveStart.Y;

            Console.WriteLine("onMove:" + rc.px + "," + rc.py);
        }

        public void main()
        {
            UIRoot.Instance.lockable = true;
            UIRoot.Instance.root.mScalex = 4;
            UIRoot.Instance.root.mScaley = 4;
            UIRoot.Instance.root.px = 20;
            UIRoot.Instance.root.py = 20;
            rc = new UIRect(128, 64);
            rc.paresent = UIRoot.Instance.root;

            rc.evtOnLMDown += onDragStart;
            rc.evtOnLMUp += onDragEnd;


            Point orgPt = new Point();
            PaintDriver.EvtOnWheel wheel = (delta) =>
                {
                    float sc = 1;
                    if(delta>0) sc = 1.1f;
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
