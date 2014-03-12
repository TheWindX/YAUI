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

using System.Drawing.Drawing2D;
using System.Drawing;

using System.Xml;

using ns_utils;



namespace ns_YAUI
{
    public class UIWidget : Entity
    {
        #region Hierarchy
        void sortSibling()
        {
            if (mParesent != null)
            {
                mParesent.sortChildrent((a, b) =>
                {
                    var a1 = a as UIWidget;
                    var b1 = b as UIWidget;
                    if (a1.depth < b1.depth)
                    {
                        return -1;
                    }
                    else if (a1.depth == b1.depth)
                    {
                        return 0;
                    }
                    else
                        return 1;
                });

                for (int i = 0; i < (mParesent as UIWidget).mChildrent.Count(); ++i)
                {
                    var u = (mParesent as UIWidget).mChildrent[i] as UIWidget;
                    u.mDepth = i;
                    u.mGlobalDepth = (mParesent as UIWidget).mGlobalDepth + ((float)i) / 10;

                    for (int j = 0; j < u.mChildrent.Count(); ++j)
                    {
                        (u.mChildrent[j] as UIWidget).mGlobalDepth = u.mGlobalDepth + ((float)j) / 10;
                    }
                }
            }
        }

        public override Entity paresent
        {
            set
            {
                base.paresent = value;
                if (value != null)
                {
                    setDepthHead();
                }
            }
        }
        
        

        public IEnumerable<UIWidget> children()
        {
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                yield return mChildrent[i] as UIWidget;
            }
        }
        
        public UIWidget childOf(string name)
        {
            foreach (var elem in children() )
            {
                if (elem.name == name)
                {
                    return elem;
                }
                else if (elem.name == null || elem.name == "")
                {
                    var ui = elem.childOf(name);
                    if (ui != null)
                        return ui;
                }
            }
            return null;
        }

        public UIWidget childOfPath(string path)
        {
            string[] pathItems = path.Split('/');
            UIWidget p = this;
            foreach (var elem in pathItems)
            {
                p = p.childOf(elem);
                if (p == null) break;
            }
            return p;
        }

        public UIWidget topParesent
        {
            get
            {
                if (mParesent != null)
                {
                    return (mParesent as UIWidget).topParesent;
                }
                else
                {
                    return this;
                }
            }
        }

        float mGlobalDepth = 0;
        float mDepth = 0;
        public float depth
        {
            get { return mDepth; }
            set
            {
                mDepth = value;
                sortSibling();
            }
        }

        public void setDepthTail()
        {
            mDepth = mChildrent.Count + 1;
            sortSibling();
        }

        public void setDepthHead()
        {
            depth = -100;
            sortSibling();
        }

        public void setDepthDeeper()
        {
            mDepth += 1.5f;
            sortSibling();
        }

        public void setDepthShallower()
        {
            mDepth -= 1.5f;
            sortSibling();
        }

        ////在同一层次上的比较
        //static int levelCompare(UIWidget u1, UIWidget u2)
        //{
        //    UIWidget v1;
        //    UIWidget v2;

        //    var c = commonParesent(u1, u2, out v1, out v2);

        //    if (v1 == v2) return 0;
        //    else
        //    {
        //        if (v1.depth < v2.depth)
        //        {
        //            return 1;
        //        }
        //        else if (v1.depth == v2.depth)
        //        {
        //            return 0;
        //        }
        //        else
        //            return 1;
        //    }
        //}

        #endregion

        #region properties
        public int ID = -1;

        //properties start
        public virtual Rectangle drawRect
        {
            get { return new Rectangle(); }
        }

        protected virtual void setWidth(int width)
        {
        }

        protected virtual void setHeight(int height)
        {
        }

        public Rectangle layoutRect
        {
            get
            {
                var rc = drawRect;
                rc.Location = new Point(rc.Location.X - marginX, rc.Location.Y - marginY);
                rc.Size = new Size(rc.Size.Width + marginX*2, rc.Size.Height + marginY*2);
                return rc;
            }
        }

        public int borderX
        {
            get { return drawRect.Width; }
        }

        public int borderY
        {
            get { return drawRect.Height; }
        }

        public virtual Rectangle pickRect
        {
            get
            {
                return drawRect;
            }
        }
        public bool focus
        {
            set
            {
                if (value)
                {
                    topParesent.attrs["focus"] = this;
                }
                else
                {
                    topParesent.attrs["focus"] = null;
                }
            }
            get
            {
                object o = null;
                bool r = topParesent.attrs.TryGetValue("focus", out o);
                if (r)
                {
                    return object.ReferenceEquals(this, r);
                }
                return false;
            }
        }

        public int px
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public int py
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }


        bool mClip = false;
        public bool clip { get { return mClip; } set { mClip = value; } }

        bool mEnable = true;
        public bool enable { get { return mEnable; } set { mEnable = value; } }

        string mName;
        public string name
        {
            get { return mName; }
            set {mName = value;}
        }

        protected bool mVisiable = true;
        public bool visible
        {
            get
            {
                return mVisiable;
            }
            set
            {
                mVisiable = value;
            }
        }

        bool mPropagate = true;
        public bool propagate
        {
            get
            {
                return mPropagate;
            }
            set
            {
                mPropagate = value;
            }
        }

        public virtual string typeName
        {
            get { return "nulltype"; }
        }
        public Dictionary<string, object> attrs = new Dictionary<string, object>();

        //box model
        public int marginX = 0;
        public int marginY = 0;
        public int paddingX = 0;
        public int paddingY = 0;
        #endregion

        #region layout
        #region align
        public Point center
        {
            get
            {
                return transform(drawRect.center());
            }

            set
            {
                var pt = center;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftTop
        {
            get
            {
                return transform(drawRect.leftTop());
            }
            set
            {
                var pt = leftTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftMiddle
        {
            get
            {
                return transform(drawRect.leftMiddle());
            }
            set
            {
                var pt = leftMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftBottom
        {
            get
            {
                return transform(drawRect.leftBottom());
            }
            set
            {
                var pt = leftBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightTop
        {
            get
            {
                return transform(drawRect.rightTop());
            }
            set
            {
                var pt = rightTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightMiddle
        {
            get
            {
                return transform(drawRect.rightMiddle());
            }
            set
            {
                var pt = rightMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightBottom
        {
            get
            {
                return transform(drawRect.rightBottom());
            }
            set
            {
                var pt = rightBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point middleTop
        {
            get
            {
                return transform(drawRect.middleTop());
            }
            set
            {
                var pt = middleTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point middleBottom
        {
            get
            {
                return transform(drawRect.middleBottom());
            }
            set
            {
                var pt = middleBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public enum EAlign
        {
            noAlign,
            center,
            leftTop,
            leftMiddle,
            leftBottom,
            rightTop,
            rightMiddle,
            rightBottom,
            middleTop,
            middleBottom,
        }

        EAlign mAlign = EAlign.noAlign;
        public EAlign align
        {
            get
            {
                return mAlign;
            }
            set
            {
                mAlign = value;
            }
        }

        EAlign mAlignParesent = EAlign.noAlign;
        public EAlign alignParesent
        {
            get
            {
                return mAlignParesent;
            }
            set
            {
                mAlignParesent = value;
            }

        }

        int mOffsetx = 0;
        public int offsetx
        {
            get
            {
                return mOffsetx;
            }
            set
            {
                mOffsetx = value;
            }
        }

        int mOffsety = 0;
        public int offsety
        {
            get
            {
                return mOffsety;
            }
            set
            {
                mOffsety = value;
            }
        }

        /// <returns> need ajust </returns>
        public virtual bool adjustAlign()
        {
            if(paresent == null)
                return false;

            if (mAlign == EAlign.noAlign || mAlignParesent == EAlign.noAlign)
            {
                return false;
            }

            var pui = (paresent as UIWidget);

            Point alignPos = new Point();
            switch (mAlignParesent)
            {
                case EAlign.center:
                    alignPos = paresent.invertTransform((paresent as UIWidget).center);
                    break;
                case EAlign.leftTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftTop);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    alignPos.Y += offsety + pui.paddingY + marginY;
                    break;
                case EAlign.leftMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftMiddle);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    break;
                case EAlign.leftBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftBottom);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.rightTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightTop);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety + pui.paddingY + marginY;
                    break;
                case EAlign.rightMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightMiddle);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    break;
                case EAlign.rightBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightBottom);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.middleTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleTop);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.middleBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleBottom);
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                default:
                    break
                    ;
            }

            switch (mAlign)
            {
                case EAlign.center:
                    this.center = alignPos;
                    break;
                case EAlign.leftTop:
                    this.leftTop = alignPos;
                    break;
                case EAlign.leftMiddle:
                    this.leftMiddle = alignPos;
                    break;
                case EAlign.leftBottom:
                    this.leftBottom = alignPos;
                    break;
                case EAlign.rightTop:
                    this.rightTop = alignPos;
                    break;
                case EAlign.rightMiddle:
                    this.rightMiddle = alignPos;
                    break;
                case EAlign.rightBottom:
                    this.rightBottom = alignPos;
                    break;
                case EAlign.middleTop:
                    this.middleTop = alignPos;
                    break;
                case EAlign.middleBottom:
                    this.middleBottom = alignPos;
                    break;
                default:
                    break;
            }

            return true;
        }
        #endregion

        #region layout implement
        public enum ELayout
        {
            none,
            vertical,
            horizen,
        }


        public ELayout mLayout = ELayout.none;
        public bool wrap = false;
        public bool resizeAble = false;//if resizeAble, wrap is invalid

        //这个因与渲染的遍历次序不同,因此不能让到draw里
        public virtual void adjustLayout()
        {
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                var c = mChildrent[i] as UIWidget;
                c.adjustLayout();
            }

            if (mLayout == ELayout.none)
            {
                //无layout就使用align
                bool bAjust = adjustAlign();
                return;
            }

            #region layout calc
            Point rb = new Point();
            Rectangle rc = new Rectangle(new Point(paddingX, paddingY), new Size(0, 0));
            rb = rc.rightBottom();
            int idxCount = 0;
            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                if (mLayout == ELayout.vertical)
                {
                    var c = mChildrent[i] as UIWidget;
                    var pt = rc.leftBottom();
                    pt.X += c.marginX;
                    pt.Y += c.marginY;
                    c.leftTop = pt;

                    var lr = c.layoutRect;
                    var rcOld = rc;
                    rc.Width = max(lr.Width, rc.Width);
                    rc.Height = rc.Height + lr.Height;
                    rb.X = max(rc.Right, rb.X);
                    rb.Y = max(rc.Bottom, rb.Y);

                    idxCount++;
                    if (!this.resizeAble && this.wrap && rc.Bottom > this.drawRect.Height - paddingY && idxCount > 1)//只能容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new Point(rc.Location.X + rc.Width, paddingY);
                        rc.Size = new Size();
                    }
                }
                else
                {
                    var c = mChildrent[i] as UIWidget;
                    var pt = rc.rightTop();
                    pt.X += c.marginX;
                    pt.Y += c.marginY;
                    c.leftTop = pt;

                    var lr = c.layoutRect;
                    var rcOld = rc;
                    rc.Height = max(lr.Height, rc.Height);
                    rc.Width = rc.Width + lr.Width;
                    rb.X = max(rc.Right, rb.X);
                    rb.Y = max(rc.Bottom, rb.Y);

                    idxCount++;
                    if (!this.resizeAble && this.wrap && rc.Right > this.drawRect.Width - paddingX && idxCount > 1)//至少容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new Point(paddingX, rc.Location.Y + rc.Height);
                        rc.Size = new Size();
                    }
                }
            }
            if (this.resizeAble)
            {
                setWidth(rb.X + paddingX);//right padding
                setHeight(rb.Y + paddingY);//button padding
            }
            #endregion
        }
        #endregion

        #endregion

        #region transform
        public Matrix transformMatrix
        {
            get
            {
                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.Rotate(mDir);
                m.Scale(mScalex, mScaley);
                return m;
            }
        }
        
        public void translate(int dx, int dy)
        {
            position.X += dx;
            position.Y += dy;
        }
        #endregion

        #region methods
        public UIWidget(){}


        #endregion

        #region events
        public delegate bool EvtMouse(UIWidget _this, int x, int y);
        public delegate bool EvtKeyboard(UIWidget _this, int kc, bool isControl, bool isShift);
        public delegate void EvtSizeChanged(UIWidget _this, int w, int h);


        public event EvtMouse evtOnMMove;
        public void evtOnMMoveClear()
        {
            evtOnMMove = null;
        }

        public event EvtMouse evtOnLMDown;
        public void evtOnLMDownClear()
        {
            evtOnLMDown = null;
        }
        public event EvtMouse evtOnLMUp;
        public void evtOnLMUpClear()
        {
            evtOnLMUp = null;
        }
        public event EvtMouse evtOnRMDown;
        public void evtOnRMDownClear()
        {
            evtOnRMDown = null;
        }
        public event EvtMouse evtOnRMUp;
        public void evtOnRMUpClear()
        {
            evtOnRMUp = null;
        }
        public event EvtMouse evtOnMMDown;
        public void evtOnMMDownClear()
        {
            evtOnMMDown = null;
        }
        public event EvtMouse evtOnMMUp;
        public void evtOnMMUpClear()
        {
            evtOnMMUp = null;
        }
        public event EvtMouse evtOnEnter;
        public void evtOnEnterClear()
        {
            evtOnEnter = null;
        }
        public event EvtMouse evtOnExit;
        public void evtOnExitClear()
        {
            evtOnExit = null;
        }
        public event EvtMouse evtOnDClick;
        public void evtOnDClickClear()
        {
            evtOnDClick = null;
        }
        public event Action evtPreDraw;
        public void evtPreDrawClear()
        {
            evtPreDraw = null;
        }

        public event EvtKeyboard evtOnChar;
        public void evtOnCharClear()
        {
            evtOnChar = null;
        }


        public bool doEvtLeftDown(int x, int y)
        {
            if (evtOnLMDown == null)
            {
                return true;
            }
            return evtOnLMDown(this, x, y);
        }

        public bool evtMove(int x, int y)
        {
            if (evtOnMMove == null)
            {
                return true;
            }
            return evtOnMMove(this, x, y);
        }

        public bool doEvtLeftUp(int x, int y)
        {
            if (evtOnLMUp == null)
            {
                return true;
            }
            return evtOnLMUp(this, x, y);
        }

        public bool doEvtRightDown(int x, int y)
        {
            if (evtOnRMDown == null)
            {
                return true;
            }
            return evtOnRMDown(this, x, y);
        }

        public bool doEvtRightUp(int x, int y)
        {
            if (evtOnRMUp == null)
            {
                return true;
            }
            return evtOnRMUp(this, x, y);
        }

        public bool doEvtMiddleDown(int x, int y)
        {
            if (evtOnMMDown == null)
            {
                return true;
            }
            return evtOnMMDown(this, x, y);
        }

        public bool doEvtMiddleUp(int x, int y)
        {
            if (evtOnMMUp == null)
            {
                return true;
            }
            return evtOnMMUp(this, x, y);
        }

        public bool doEvtOnEnter(int x, int y)
        {
            if (evtOnEnter == null)
            {
                return true;
            }
            return evtOnEnter(this, x, y);
        }

        public bool doEvtOnExit(int x, int y)
        {
            if (evtOnExit == null)
            {
                return true;
            }
            return evtOnExit(this, x, y);
        }

        public bool doEvtOnChar(int kc, bool isControl, bool isShift)
        {
            if (evtOnChar == null)
            {
                return true;
            }
            return evtOnChar(this, kc, isControl, isShift);
        }

        public bool doEvtDoubleClick(int x, int y)
        {
            if (evtOnDClick == null)
            {
                return true;
            }
            return evtOnDClick(this, x, y);
        }
        #endregion

        #region utils
        static int idcount = 1000000;
        public static int newID
        {
            get
            {
                return idcount++;
            }
        }
        //for test 
        /// <summary>
        /// dragable
        /// </summary>
        bool mDragAble = false;
        public bool dragAble
        {
            set
            {
                mDragAble = value;
                if (mDragAble)
                {
                    evtOnLMDown += onDragBegin;
                    //evtOnLMUp += onDragEnd;
                }
                else
                {
                    evtOnLMDown -= onDragBegin;
                    onDragEnd(0, 0);
                }

            }

            get
            {
                return mDragAble;
            }
        }

        Point mPtDragBegin;
        Point mPosBegin;
        void onDragMove(int x, int y)
        {
            var pt = invertTransformParesentAbs(new Point(x, y));
            int dx = pt.X - mPtDragBegin.X;
            int dy = pt.Y - mPtDragBegin.Y;
            position.X = mPosBegin.X + dx;
            position.Y = mPosBegin.Y + dy;
        }

        bool onDragBegin(UIWidget _this, int x, int y)
        {
            mPosBegin = position;
            mPtDragBegin = invertTransformParesentAbs(new Point(x, y));

            this.setDepthHead();

            UIRoot.Instance.mEvtMove += onDragMove;
            UIRoot.Instance.mEvtLeftUp += onDragEnd;
            return false;
        }

        void onDragEnd(int x, int y)
        {
            UIRoot.Instance.mEvtMove -= onDragMove;
            UIRoot.Instance.mEvtLeftUp -= onDragEnd;
        }


        bool mRotateAble = false;
        public bool rotateAble
        {
            set
            {
                mRotateAble = value;
                if (mRotateAble)
                {
                    evtOnRMDown += onRotateBegin;
                    //evtOnRMUp += onRotateEnd;
                }
                else
                {
                    evtOnRMDown -= onRotateBegin;
                    onRotateEnd(0, 0);
                }
            }

            get
            {
                return mRotateAble;
            }
        }

        Point ptRotateOrg;
        float dirRotateOrg;
        Point ptLocalRotateOrg;
        bool onRotateBegin(UIWidget _this, int x, int y)
        {
            ptRotateOrg = position;
            dirRotateOrg = mDir;
            ptLocalRotateOrg = invertTransformAbs(new Point(x, y));

            UIRoot.Instance.mEvtMove += onRotateMove;
            UIRoot.Instance.mEvtRightUp += onRotateEnd;
            return false;
        }

        void onRotateMove(int x, int y)
        {
            int dx = x - ptRotateOrg.X;
            int dy = y - ptRotateOrg.Y;

            var dist = dx;

            mDir = dirRotateOrg + (float)(dist * 0.2f);
            var pt = invertTransformAbs(new Point(x, y));

            int posDx = pt.X - ptLocalRotateOrg.X;
            int posDy = pt.Y - ptLocalRotateOrg.Y;
            //position.X += posDx;
            //position.Y += posDy;
        }

        void onRotateEnd(int x, int y)
        {
            UIRoot.Instance.mEvtMove -= onRotateMove;
            UIRoot.Instance.mEvtRightUp -= onRotateEnd;
        }

        /// <summary>
        /// dragable end
        /// </summary>


        /// <summary>
        /// scaleAble begin
        /// </summary>
        bool mScaleAble = false;
        public bool scaleAble
        {
            set
            {
                mScaleAble = value;
                if (mScaleAble)
                {
                    evtOnLMDown += onScaleBegin;
                    UIRoot.Instance.mEvtLeftUp += onScaleEnd;
                }
                else
                {
                    evtOnLMDown -= onScaleBegin;
                    UIRoot.Instance.mEvtLeftUp -= onScaleEnd;
                    onScaleEnd(0, 0);
                }

            }

            get
            {
                return mDragAble;
            }
        }
        Point mWheelScaleBegin;
        void onScaleWheel(int delta)
        {
            float sc = 1;
            if (delta > 0) sc = 1.1f;
            else sc = 0.9f;
            this.scalePoint(mWheelScaleBegin, sc);
        }

        bool onScaleBegin(UIWidget ui, int x, int y)
        {
            mWheelScaleBegin = invertTransformParesentAbs(new Point(x, y));
            UIRoot.Instance.mEvtWheel += onScaleWheel;
            return false;
        }

        void onScaleEnd(int x, int y)
        {
            UIRoot.Instance.mEvtWheel -= onScaleWheel;
        }
        /// <summary>
        /// scaleAble end
        /// </summary>

        //utils
        protected static int min(int a, int b) { if (a < b)return a; else return b; }
        protected static int max(int a, int b) { if (a > b)return a; else return b; }
        protected static Rectangle expandRect(Rectangle r1, Rectangle r2)
        {
            int left = min(r1.Left, r2.Left);
            int top = min(r1.Top, r2.Top);
            int right = max(r1.Right, r2.Right);
            int buttom = max(r1.Bottom, r2.Bottom);

            return new Rectangle(left, top, right - left, buttom - top);
        }

        public XmlNodeList fromXML(XmlNode node)
        {
            var ret = node.Attributes.GetNamedItem("name");
            if (ret != null) name = ret.Value;

            ret = node.Attributes.GetNamedItem("px");
            if (ret != null) px = ret.Value.castInt();

            ret = node.Attributes.GetNamedItem("py");
            if (ret != null) py = ret.Value.castInt();

            ret = node.Attributes.GetNamedItem("rotate");
            if (ret != null) mDir = ret.Value.castFloat();

            ret = node.Attributes.GetNamedItem("scalex");
            if (ret != null) mScalex = ret.Value.castFloat(1);

            ret = node.Attributes.GetNamedItem("scaley");
            if (ret != null) mScaley = ret.Value.castFloat(1);

            ret = node.Attributes.GetNamedItem("align");
            if (ret != null) 
            {
                mAlign = (EAlign)Enum.Parse(typeof(EAlign), node.Attributes["align"].Value);
            }
            ret = node.Attributes.GetNamedItem("alignParesent");
            if (ret != null)
            {
                mAlignParesent = (EAlign)Enum.Parse(typeof(EAlign), node.Attributes["alignParesent"].Value);
            }

            //note //offset is after align
            ret = node.Attributes.GetNamedItem("offsetx");
            if (ret != null) { var ox = ret.Value.castInt(); mOffsetx = ox; }

            ret = node.Attributes.GetNamedItem("offsety");
            if (ret != null) { var oy = ret.Value.castInt(); mOffsety = oy; }

            ret = node.Attributes.GetNamedItem("clip");
            if (ret != null)
            {
                clip = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("enable");
            if (ret != null)
            {
                enable = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("visible");
            if (ret != null)
            {
                visible = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("dragAble");
            if (ret != null)
            {
                dragAble = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("dragAble");
            if (ret != null)
            {
                scaleAble = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("layout");
            if (ret != null)
            {
                mLayout = (ELayout)Enum.Parse(typeof(ELayout), ret.Value);
            }

            ret = node.Attributes.GetNamedItem("wrap");
            if (ret != null)
            {
                wrap = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("resizeAble");
            if (ret != null)
            {
                resizeAble = ret.Value.castBool();
            }

            ret = node.Attributes.GetNamedItem("marginX");
            if (ret != null)
            {
                marginX = ret.Value.castInt();
            }

            ret = node.Attributes.GetNamedItem("marginY");
            if (ret != null)
            {
                marginY = ret.Value.castInt();
            }

            ret = node.Attributes.GetNamedItem("paddingX");
            if (ret != null)
            {
                paddingX = ret.Value.castInt();
            }

            ret = node.Attributes.GetNamedItem("paddingY");
            if (ret != null)
            {
                paddingY = ret.Value.castInt();
            }
            return node.ChildNodes;
        }

        public bool appendFromXML(string xml)
        {
            var ui = UIRoot.Instance.loadFromXML(xml);
            if (ui == null) return false;
            else ui.paresent = this;
            return true;
        }

        #endregion

        #region others

        public virtual bool postTestPick(Point pos)
        {
            return false;
        }


        public virtual bool preTestPick(Point pos)
        {
            return true;
        }

        public bool doTestPick(Point pos, out UIWidget ui, bool testEnable = true)
        {
            if (testEnable && !enable)
            {
                ui = null;
                return false;
            }

            var t = transformMatrix.Clone();
            t.Invert();
            var ps = new Point[] { pos };
            t.TransformPoints(ps);
            var newpos = ps[0];

            var r = pickRect;
            if (!r.Contains(newpos))
            {
                ui = null;
                return false;
            }

            bool ret = preTestPick(newpos);
            if (!ret)
            {
                ui = null;
                return false;
            }

            UIWidget picked = null;
            foreach (UIWidget elem in children())
            {
                if (elem.doTestPick(newpos, out picked, testEnable))
                {
                    //uilist.Add(picked);
                    break;
                }
            }

            if (picked != null)
            {
                ui = picked;
                return true;
            }

            ret = postTestPick(newpos);
            if (!ret)
            {
                ui = null;
                return false;
            }
            ui = this;
            return true;
        }

        //public static Font mDrawFont = new Font("Arial", 12, FontStyle.Bold);
        internal void doDraw(Graphics g)
        {
            if (!mVisiable)
                return;
            if (evtPreDraw != null)
                    evtPreDraw();

            var gs = g.Save();

            Matrix _mtx = g.Transform;
            _mtx.Multiply(this.getLocalMatrix());
            g.Transform = _mtx;

            //catch it?
            onDraw(g);

            if (clip)
            {
                var r = drawRect;
                //r.Offset(-1, -1);
                r.Width += 1;
                r.Height += 1;
                GraphicsPath p = new GraphicsPath();
                p.AddRectangle(r);
                g.SetClip(p, CombineMode.Intersect);
            }

            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                (mChildrent[i] as UIWidget).doDraw(g);
            }

            //for text drawing            

            g.Restore(gs);
            //popMatrix(g);
        }

        internal virtual void onDraw(Graphics g) { }
        #endregion
    }
}
