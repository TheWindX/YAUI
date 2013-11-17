//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Drawing;
//using System.Drawing.Drawing2D;

//namespace ns_behaviour
//{
//    class UIReference : UIWidget
//    {
//        UIWidget mRef;
//        public UIReference(UIWidget reference = null)
//        {
//            mRef = reference;
//        }

//        public override Rectangle rect
//        {
//            get
//            {
//                if()
//                {
//                }
//            }
//        }

//        public override string typeName
//        {
//            get { return "rect"; }
//        }

//        public override bool testPick(Point pos)
//        {
//            return true;
//        }

//        internal override void onDraw(Graphics g) 
//        {
//            GraphicsPath p = new GraphicsPath();
//            p.AddRectangle(new Rectangle(0, 0, _w, _h));
//            g.FillPath(mBrush, p);
//            g.DrawPath(mPen, p);
//        }
//    }
//}
