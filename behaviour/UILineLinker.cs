using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace ns_behaviour
{

    class UILineNode : UIWidget
    {
        const int lineWidth = 3;
        
        public enum EDIR
        {
            e_ver,
            e_hor,
        }

        public new EDIR mDir = EDIR.e_ver;

        public int mLength;

        public UILineNode(EDIR dir, int length)
        {
            mDir = dir;
            mLength = length;
        }

        public override Rectangle rect
        {
            get
            {
                if (mDir == EDIR.e_ver)
                {
                    int left = -lineWidth;
                    int width = lineWidth * 2;
                    int top = 0;
                    int height = mLength;
                    if (mLength < 0)
                    {
                        top = mLength;
                        height = -mLength;
                    }
                    return new Rectangle(left, top, width, height);
                }
                else
                {
                    int left = 0;
                    int width = mLength;
                    int top = -lineWidth;
                    int height = lineWidth*2;
                    if (mLength < 0)
                    {
                        left = mLength;
                        width = -mLength;
                    }
                    return new Rectangle(left, top, width, height);
                }
            }
        }

        public override string typeName
        {
            get { return "UILineNode"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            if (mParesent != null)
            {
                var linker = mParesent as UILineLinker;
                GraphicsPath p = new GraphicsPath();
                p.AddRectangle(rect);
                g.FillPath(linker.mBrush, p);
            }
        }

        ////
        internal UILineNode mPre;
        internal UILineNode mNext;

        static Point mInvalidPt = new Point(int.MaxValue, int.MaxValue);
        Point mPosBack = mInvalidPt;
        internal void savePos()
        {
            mPosBack = mPos;
        }

        internal bool restorePos()
        {
            return true;
            if (mPosBack != mInvalidPt)
            {
                mPos = mPosBack;
                mPosBack = mInvalidPt;
                return true;
            }
            return false;
        }

        internal void restorePre()
        {
            restorePos();
            if (mPre != null)
                mPre.restorePre();
        }

        internal void restorePost()
        {
            restorePos();
            if (mNext != null)
                mNext.restorePost();
        }

        internal Point tailPos()//由mPos and length 得到的 tail pos
        {
            if (mDir == EDIR.e_hor)
            {
                return new Point(mPos.X + mLength, mPos.Y);
            }
            else
            {
                return new Point(mPos.X, mPos.Y + mLength);
            }
        }

        //返回是否原点有调整
        internal bool tryAdjustByTail(Point pt)
        {
            savePos();
            Point tpos = tailPos();
            if (pt == tpos) return true;
            if (mDir == EDIR.e_hor)
            {
                int dx = pt.X - tpos.X;
                mLength += dx;

                int dy = pt.Y - tpos.Y;
                if (dy == 0) return true;
                else mPos.Y += dy;
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = pt.X - tpos.X;
                int dy = pt.Y - tpos.Y;

                mLength += dy;

                if (dx == 0) return true;
                else mPos.X += dx;
            }
            if (mPre == null)//cannot ajust
            {
                restorePos();
                return false;
            }
            else
            {
                bool t = mPre.tryAdjustByTail(mPos);
                if (!t)
                {
                    restorePos();
                    return false;
                }
                return true;
            }
            return true;
        }

        //返回是否原点有调整
        internal bool tryAdjustByHead(Point pt)
        {
            savePos();
            Point tpos = tailPos();
            mPos = pt;
            Point tPos1 = tailPos();

            if (tpos == tPos1) return true;
            if (mDir == EDIR.e_hor)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dx;

                if (dy == 0) return true;
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dy;

                if (dx == 0) return true;
            }
            if (mNext == null)//cannot ajust
            {
                restorePos();
                return false;
            }
            else
            {
                bool t = mNext.tryAdjustByHead(tPos1);
                if (!t)
                {
                    restorePos();
                    return false;
                }
                return true;
            }
            return true;
        }

        internal bool adjustPos(Point pt)
        {
            if(mPre == null || mNext == null) return false;
            savePos();
            if (!tryAdjustByHead(pt))
            {
                return false;
            }
            else 
                if (mPre != null && !mPre.tryAdjustByTail(pt))
            {
                restorePost();
                return false;
            }
            return true;
        }
    }

    class UILineLinker : UIWidget
    {
        UILineNode mFirst;

        uint mFillColor;
        internal Brush mBrush;
        Pen mPen;
        public UILineLinker(uint fill = 0xff888888)
        {
            //mFirst = new UILineNode(UILineNode.EDIR.e_hor, 0);
            setFillColor(fill);
        }

        void setFillColor(uint fillColor = 0xff888888)
        {
            mFillColor = fillColor;
            mBrush = new SolidBrush(Color.FromArgb((Int32)mFillColor));
        }

        static int min(int a, int b) { if (a < b)return a; else return b; }
        static int max(int a, int b) { if (a > b)return a; else return b; }
        static Rectangle expandRect(Rectangle r1, Rectangle r2)
        {
            int left = min(r1.Left, r2.Left);
            int top = min(r1.Top, r2.Top);
            int right = max(r1.Right, r2.Right);
            int buttom = max(r1.Bottom, r2.Bottom);

            return new Rectangle(left, top, right - left, buttom - top);
        }

        public IEnumerable<UILineNode> nodeIter()
        {
            UILineNode iter = mFirst;
            while (iter != null)
            {
                yield return iter;
                iter = iter.mNext;
            }
        }

        public override Rectangle rect
        {
            get
            {
                Rectangle ret = new Rectangle();
                bool init = false;
                foreach (var elem in nodeIter())
                {
                    if (!init)
                    {
                        init = true;
                        ret = elem.rect;
                        ret.Offset(elem.mPos);
                    }
                    else
                    {
                        var elemRc = elem.rect;
                        elemRc.Offset(elem.mPos);
                        ret = expandRect(ret, elemRc);
                    }
                }
                return ret;
            }
        }

        public override string typeName
        {
            get { return "lineList"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            //no use
        }

        public class cons
        {
            UIWidget mParesent;
            public UILineLinker mLinker;
            UILineNode mBaseNode;

            public cons(UIWidget p)
            {
                mParesent = p;
            }

            public void moveTo(Point pt)
            {
                mLinker = new UILineLinker();
                mLinker.setParesent(mParesent);

                mBaseNode = new UILineNode(UILineNode.EDIR.e_hor, 0);
                mBaseNode.setParesent(mLinker);
                mLinker.mFirst = mBaseNode;
                mBaseNode.mPos = pt;
            }

            public void tryLineTo(Point pt)
            {
                int dx = pt.X - mBaseNode.mPos.X;
                int dy = pt.Y - mBaseNode.mPos.Y;

                if (Math.Abs(dy) > Math.Abs(dx))
                {
                    mBaseNode.mDir = UILineNode.EDIR.e_ver;
                    mBaseNode.mLength = dy;
                }
                else
                {
                    mBaseNode.mDir = UILineNode.EDIR.e_hor;
                    mBaseNode.mLength = dx;
                }
                
            }

            public void lineTo(Point pt)
            {
                if (mLinker == null) { moveTo(pt); return; }
                else
                {
                    tryLineTo(pt);
                    var newNode = new UILineNode(UILineNode.EDIR.e_hor, 0);
                    newNode.setParesent(mLinker);
                    newNode.mPre = mBaseNode;
                    mBaseNode.mNext = newNode;
                    mBaseNode = newNode;
                    mBaseNode.mPos = mBaseNode.mPre.tailPos();
                }
            }
        }

    }
}
