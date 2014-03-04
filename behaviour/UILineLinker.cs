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

        public EForward beginForward
        {
            get
            {
                if (mDir == EDIR.e_hor)
                {
                    if (endPos.X > beginPos.X)
                    {
                        return EForward.e_left;
                    }
                    else
                    {
                        return EForward.e_right;
                    }
                }
                else //if (mDir == EDIR.e_ver)
                {
                    if (endPos.Y > beginPos.Y)
                    {
                        return EForward.e_up;
                    }
                    else
                    {
                        return EForward.e_down;
                    }
                }
            }
        }

        public EForward endForward
        {
            get
            {
                if (mDir == EDIR.e_hor)
                {
                    if (endPos.X > beginPos.X)
                    {
                        return EForward.e_right;
                    }
                    else
                    {
                        return EForward.e_left;
                    }
                }
                else //if (mDir == EDIR.e_ver)
                {
                    if (endPos.Y > beginPos.Y)
                    {
                        return EForward.e_down;
                    }
                    else
                    {
                        return EForward.e_up;
                    }
                }
            }
        }

        public override Rectangle pickRect
        {
            get
            {
                int pickLineWidth = lineWidth * 5;
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

        public override bool postTestPick(Point pos)
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
            mPosBack = position;
        }

        internal bool restorePos()
        {
            return true;
            if (mPosBack != mInvalidPt)
            {
                position = mPosBack;
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


        public Point beginPos
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Point endPos
        {
            get
            {
                if (mDir == EDIR.e_hor)
                {
                    return new Point(position.X + mLength, position.Y);
                }
                else //if(mDir == EDIR.e_ver)
                {
                    return new Point(position.X, position.Y + mLength);
                }
            }
            set
            {
                var tp = endPos;
                int dx = value.X - tp.X;
                int dy = value.Y - tp.Y;
                position = new Point(position.X + dx, position.Y + dy);
            }
        }


        bool adjustFromFrontStable(Point val)
        {
            var tpos = endPos;
            beginPos = val;
            var tPos1 = endPos;

            if (mDir == EDIR.e_hor)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dx;
                if (dy == 0) return true;
                //else if (mNext == null)
                //{
                //    position.Y += dy;
                //    if(mPre != null)
                //        mPre.adjustFromEndStable(beginPos);
                //}
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = tpos.X - tPos1.X;
                int dy = tpos.Y - tPos1.Y;

                mLength += dy;
                if (dx == 0) return true;
                //else if (mNext == null)
                //{
                //    position.X += dx;
                //    if (mPre != null)
                //        mPre.adjustFromEndStable(beginPos);
                //}
            }

            return false;
        }


        bool adjustFromEndStable(Point val)
        {
            var hpos = beginPos;
            endPos = val;
            var hPos1 = beginPos;

            if (mDir == EDIR.e_hor)
            {
                int dx = hpos.X - hPos1.X;
                int dy = hpos.Y - hPos1.Y;

                position.X += dx;
                mLength -= dx;
                if (dy == 0) return true;
                //else if (mPre == null)
                //{
                //    position.Y += dy;
                //    if (mNext != null)
                //        mNext.adjustFromFrontStable(endPos);
                //}
            }
            else if (mDir == EDIR.e_ver)
            {
                int dx = hpos.X - hPos1.X;
                int dy = hpos.Y - hPos1.Y;

                position.Y += dy;
                mLength -= dy;
                if (dx == 0)
                {
                    return true;
                }
                //else if (mPre == null)
                //{
                //    position.X += dx;
                //    if (mNext != null)
                //        mNext.adjustFromFrontStable(endPos);
                //}
            }

            
            return false;
        }


        //返回是否原点维持
        internal bool adjustFromEndSeq(Point pt)
        {
            bool ret = adjustFromEndStable(pt);
            if (ret)
            {
                return true;
            }
            else
            {
                if (mPre != null)
                {
                    mPre.adjustFromEndSeq(beginPos);
                }
            }

            return false;
        }

        //返回是否原点维持
        internal bool adjustFromFrontSeq(Point pt)
        {
            bool ret = adjustFromFrontStable(pt);
            if (ret)
            {
                return true;
            }
            else
            {
                if (mNext != null)
                {
                    mNext.adjustFromFrontSeq(endPos);
                }
            }

            return false;
        }

        internal bool adjustFromFrontToBothSide(Point pt)
        {
            //if ( (mPre != null && mPre.mPre != null)
            //     || (mNext != null && mNext.mNext != null))
            //{
            //    if (mDir == EDIR.e_hor)
            //    {
            //        pt.X = position.X;
            //    }
            //    else if (mDir == EDIR.e_ver)
            //    {
            //        pt.Y = position.Y;
            //    }   
            //}

            adjustFromFrontToTail(pt);
            adjustFromFrontToHead(pt);
            return true;
        }

        internal bool adjustFromFrontToTail(Point pt)
        {   
            if (mNext != null)
            {
                beginPos = pt;
                mNext.adjustFromFrontSeq(endPos);
            }
            return true;
        }

        internal bool adjustFromFrontToHead(Point pt)
        {
            if (mPre != null)
            {  
                beginPos = pt;
                mPre.adjustFromEndSeq(pt);
            }
            return true;
        }
    }

    class UILineLinker : UIWidget
    {
        UILineNode mFirst;
        UILineNode mLast;

        UIRect mStartRect;
        UIArrow mBeginArrow;
        UIArrow mEndArrow;
        public UILineNode first
        {
            get
            {
                return mFirst;
            }
        }

        public UILineNode last
        {
            get
            {
                //UILineNode iter = first;
                //UILineNode lastNode = iter;
                //while (iter != null)
                //{
                //    lastNode = iter;
                //    iter = iter.mNext;
                //}
                //return lastNode;
                return mLast;
            }
        }

        uint mFillColor;
        internal Brush mBrush;
        Pen mPen;
        public UILineLinker(uint fill = 0xff888888)
        {
            //mFirst = new UILineNode(UILineNode.EDIR.e_hor, 0);
            setFillColor(fill);
        }

        public void setFillColor(uint fillColor = 0xff888888)
        {
            mFillColor = fillColor;
            mBrush = new SolidBrush(Color.FromArgb((Int32)mFillColor));
        }

        Point mFrom;

        //线的终点
        public Point endPos
        {
            get
            {
                if (last == null)
                    return mFrom;
                else
                {
                    return last.endPos;
                }
            }
        }

        //线的终点
        public Point beginPos
        {
            get
            {
                if (first == null)
                    return mFrom;
                else
                {
                    return first.beginPos;
                }
            }
        }

        public void startFrom(Point pt)
        {
            mFrom = pt;
        }

        public void appendNode(UILineNode.EDIR dir, int length)
        {
            var node = new UILineNode(dir, length);
            node.paresent = this;
            if (first == null)
            {
                node.beginPos = mFrom;
                mFirst = node;
                mLast = node;
            }
            else
            {
                if(last.mDir == dir)
                {
                    last.mLength += length;
                }
                else
                {
                    node.beginPos = last.endPos;
                    last.mNext = node;
                    node.mPre = last;
                    mLast = node;
                }
            }
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

        public override Rectangle drawRect
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
                        ret = elem.drawRect;
                        ret.Offset(elem.position);
                    }
                    else
                    {
                        var elemRc = elem.drawRect;
                        elemRc.Offset(elem.position);
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

        public override bool postTestPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            //set up sign end
            if (mFirst == null)
            {
                if (mStartRect == null)
                {
                    mStartRect = new UIRect(4, 4, 0xffff0000);
                    mStartRect.paresent = this;
                    mStartRect.position = beginPos;
                    mStartRect.position.X -= 2;
                    mStartRect.position.Y -= 2;
                }
            }
            else
            {
                if (mStartRect != null)
                {
                    mStartRect.paresent = null;
                    mStartRect = null;
                }
                if (mBeginArrow == null)
                {
                    mBeginArrow = new UIArrow(8, 8);
                    mEndArrow = new UIArrow(8, 8);
                    mBeginArrow.paresent = this;
                    mEndArrow.paresent = this;
                }
                mBeginArrow.center = beginPos;
                mBeginArrow.forward = mFirst.beginForward;
                mEndArrow.center = endPos;
                mEndArrow.forward = mLast.endForward;

            }
        }

        public class cons
        {
            UIWidget mParesent;
            public UILineLinker mLinker;
            public cons(UIWidget p)
            {
                mParesent = p;
            }

            public void tryLineTo(Point pt)
            {
                pt = mParesent.invertTransformAbs(pt);
                int dx = pt.X - mLinker.last.position.X;
                int dy = pt.Y - mLinker.last.position.Y;

                if (Math.Abs(dy) > Math.Abs(dx))
                {
                    mLinker.last.mDir = UILineNode.EDIR.e_ver;
                    mLinker.last.mLength = dy;
                }
                else
                {
                    mLinker.last.mDir = UILineNode.EDIR.e_hor;
                    mLinker.last.mLength = dx;
                }

            }

            public void lineTo(Point pt)
            {
                pt = mParesent.invertTransformAbs(pt);
                if (mLinker == null) 
                {
                    mLinker = new UILineLinker();
                    mLinker.paresent = mParesent;
                    mLinker.startFrom(pt);
                }
                else
                {
                    int dx;
                    int dy;

                    dx = pt.X - mLinker.endPos.X;
                    dy = pt.Y - mLinker.endPos.Y;

                    if (Math.Abs(dy) > Math.Abs(dx))
                    {
                        var dir = UILineNode.EDIR.e_ver;
                        var length = dy;
                        mLinker.appendNode(dir, length);
                    }
                    else
                    {
                        var dir = UILineNode.EDIR.e_hor;
                        var length = dx;
                        mLinker.appendNode(dir, length);
                    }
                }
            }
        }

    }
}
