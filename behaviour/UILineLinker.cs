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

    class UILineNode : UIWidget
    {
        const int lineWidth = 1;

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
                int pickLineWidth = lineWidth * 3;
                if (mDir == EDIR.e_ver)
                {
                    int left = -pickLineWidth;
                    int width = pickLineWidth * 2;
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
                    int top = -pickLineWidth;
                    int height = pickLineWidth * 2;
                    if (mLength < 0)
                    {
                        left = mLength;
                        width = -mLength;
                    }
                    return new Rectangle(left, top, width, height);
                }
            }
        }

        public Rectangle drawRect
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
                    int height = lineWidth * 2;
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
                p.AddRectangle(drawRect);
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
            //restorePos();
            //if (mNext != null)
            //    mNext.restorePost();
        }


        public Point headPos
        {
            get
            {
                return mPos;
            }
            set
            {
                mPos = value;
            }
        }

        public Point tailPos
        {
            get
            {
                if (mDir == EDIR.e_hor)
                {
                    return new Point(mPos.X + mLength, mPos.Y);
                }
                //else if(mDir == EDIR.e_ver)
                {
                    return new Point(mPos.X, mPos.Y + mLength);
                }
            }
            set
            {
                var tp = tailPos;
                int dx = value.X - tp.X;
                int dy = value.Y - tp.Y;
                mPos = new Point(mPos.X + dx, mPos.Y + dy);
            }
        }


        bool setHeadPosStable(Point val)
        {
            var tpos = tailPos;
            headPos = val;
            var tPos1 = tailPos;

            if (mDir == EDIR.e_hor)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dx;
                if (dy == 0) return true;
                else if (mNext == null)
                {
                    mPos.Y += dy;
                    if (mPre != null)
                    {
                        //mPre.adjustPosPre(new Point(mPre.mPos.X, mPre.mPos.Y+dy));
                        adjustPos(mPos);
                    }
                }
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dy;
                if (dx == 0) return true;
                else if (mNext == null)
                {
                    mPos.X += dx;
                    if (mPre != null)
                    {
                        //mPre.adjustPosPre(new Point(mPre.mPos.X + dx, mPre.mPos.Y));
                        adjustPos(mPos);
                    }
                }
            }

            return false;
        }


        bool setTailPosStable(Point val)
        {
            var hpos = headPos;
            tailPos = val;
            var hPos1 = headPos;

            if (mDir == EDIR.e_hor)
            {
                int dx = hpos.X - hPos1.X;
                int dy = hpos.Y - hPos1.Y;

                mPos.X += dx;
                mLength -= dx;
                if (dy == 0) return true;
                else if (mPre == null)
                {
                    mPos.Y += dy;
                    if(mNext != null)
                    {
                        mNext.adjustPosPost(tailPos);
                    }
                }
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = hpos.X - hPos1.X;
                int dy = hpos.Y - hPos1.Y;

                mPos.Y += dy;
                mLength -= dy;
                if (dx == 0) return true;
                else if(mPre == null)
                {
                    mPos.X += dx;
                    if (mNext != null)
                    {
                        mNext.adjustPosPost(tailPos);
                        //mNext.tryAdjustByHead(tailPos);
                    }
                }
            }

            
            return false;
        }


        //返回是否原点维持
        internal bool tryAdjustByTail(Point pt)
        {
            bool ret = setTailPosStable(pt);
            if (ret)
            {
                return true;
            }
            else
            {
                if (mPre != null)
                {
                    mPre.tryAdjustByTail(headPos);
                }
            }

            return false;
        }

        //返回是否原点维持
        internal bool tryAdjustByHead(Point pt)
        {
            bool ret = setHeadPosStable(pt);
            if (ret)
            {
                return true;
            }
            else
            {
                if (mNext != null)
                {
                    mNext.tryAdjustByHead(tailPos);
                }
            }

            return false;
        }

        internal bool adjustPos(Point pt)
        {
            headPos = pt;
            adjustPosPost(pt);
            adjustPosPre(pt);
            return true;
        }

        internal bool adjustPosPost(Point pt)
        {
            headPos = pt;
            if (mNext != null)
                mNext.tryAdjustByHead(tailPos);
            return true;
        }

        internal bool adjustPosPre(Point pt)
        {
            headPos = pt;
            if (mPre != null)
                mPre.tryAdjustByTail(pt);
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
            Point mStartPos;
            public cons(UIWidget p)
            {
                mParesent = p;
            }

            public void moveTo(Point pt)
            {
                mLinker = new UILineLinker();
                mLinker.setParesent(mParesent);
                mStartPos = pt;                
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
                    
                    if (mBaseNode == null)
                    {
                        mBaseNode = new UILineNode(UILineNode.EDIR.e_hor, 0);
                        mBaseNode.setParesent(mLinker);
                        mLinker.mFirst = mBaseNode;
                        mBaseNode.mPos = mStartPos;
                        tryLineTo(pt);
                    }
                    else
                    {
                        var oldNode = mBaseNode;
                        var newNode = new UILineNode(UILineNode.EDIR.e_hor, 0);
                        newNode.setParesent(mLinker);
                        newNode.mPre = mBaseNode;
                        mBaseNode.mNext = newNode;
                        newNode.mPos = mBaseNode.tailPos;
                        mBaseNode = newNode;
                        tryLineTo(pt);

                        if (oldNode.mDir == newNode.mDir)
                        {
                            oldNode.mNext = null;
                            newNode.setParesent(null);
                            mBaseNode = oldNode;
                            tryLineTo(pt);
                        }
                        
                    }
                }
            }
        }

    }
}
