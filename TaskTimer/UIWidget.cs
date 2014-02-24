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



namespace ns_behaviour
{
    class UIWidget : Entity
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

        #endregion

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

        public Point leftButtom
        {
            get
            {
                return transform(drawRect.leftButtom());
            }
            set
            {
                var pt = leftButtom;
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

        public Point rightButtom
        {
            get
            {
                return transform(drawRect.rightButtom());
            }
            set
            {
                var pt = rightButtom;
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

        public Point middleButtom
        {
            get
            {
                return transform(drawRect.middleButtom());
            }
            set
            {
                var pt = middleButtom;
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
            leftButtom,
            rightTop,
            rightMiddle,
            rightButtom,
            middleTop,
            middleButtom,
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

        public virtual void adjust()
        {
            if(paresent == null)
                return;

            if (mAlign == EAlign.noAlign || mAlignParesent == EAlign.noAlign)
            {
                return;
            }

            var pui = (paresent as UIWidget);

            if (mAlign == EAlign.leftTop && mAlignParesent == EAlign.leftTop)
            {
                position.X = offsetx;
                position.Y = offsety;
                return;
            }
            Point alignPos = new Point();
            switch (mAlign)
            {
                case EAlign.center:
                    alignPos = paresent.invertTransform((paresent as UIWidget).center);
                    break;
                case EAlign.leftTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftTop);
                    break;
                case EAlign.leftMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftMiddle);
                    break;
                case EAlign.leftButtom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftButtom);
                    break;
                case EAlign.rightTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightTop);
                    break;
                case EAlign.rightMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightMiddle);
                    break;
                case EAlign.rightButtom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightButtom);
                    break;
                case EAlign.middleTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleTop);
                    break;
                case EAlign.middleButtom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleButtom);
                    break;
                default:
                    break
                    ;
            }

            switch (mAlignParesent)
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
                case EAlign.leftButtom:
                    this.leftButtom = alignPos;
                    break;
                case EAlign.rightTop:
                    this.rightTop = alignPos;
                    break;
                case EAlign.rightMiddle:
                    this.rightMiddle = alignPos;
                    break;
                case EAlign.rightButtom:
                    this.rightButtom = alignPos;
                    break;
                case EAlign.middleTop:
                    this.middleTop = alignPos;
                    break;
                case EAlign.middleButtom:
                    this.middleButtom = alignPos;
                    break;
                default:
                    break
                    ;
            }

            position.X += offsetx;
            position.Y += offsety;
        }

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
        public UIWidget()
        {
        }
        static UIWidget commonParesent(UIWidget u1, UIWidget u2, out UIWidget sub1, out UIWidget sub2)
        {
            List<UIWidget> u1Plist = new List<UIWidget>();
            UIWidget t1 = u1;
            while (t1 != null)
            {
                u1Plist.Add(t1);
                t1 = t1.mParesent as UIWidget;
            }

            List<UIWidget> u2Plist = new List<UIWidget>();
            UIWidget t2 = u2;
            while (t2 != null)
            {
                u2Plist.Add(t2);
                t2 = t2.mParesent as UIWidget;
            }

            UIWidget c = null;
            int i1 = u1Plist.Count - 1;
            int i2 = u2Plist.Count - 1;
            sub1 = null;
            sub2 = null;
            for (; i1 >= 0 && i2 >= 0; )
            {
                var c1 = u1Plist[i1];
                var c2 = u2Plist[i2];
                sub1 = c1;
                sub2 = c2;

                if (c1 == c2)
                {
                    c = c1;
                }
                i1--;
                i2--;
            }
            return c;
        }
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


        public bool doEvtOnLMDown(int x, int y)
        {
            if (evtOnLMDown == null)
            {
                return true;
            }
            return evtOnLMDown(this, x, y);
        }

        public bool doEvtOnMMove(int x, int y)
        {
            if (evtOnMMove == null)
            {
                return true;
            }
            return evtOnMMove(this, x, y);
        }

        public bool doEvtOnLMUp(int x, int y)
        {
            if (evtOnLMUp == null)
            {
                return true;
            }
            return evtOnLMUp(this, x, y);
        }

        public bool doEvtOnRMDown(int x, int y)
        {
            if (evtOnRMDown == null)
            {
                return true;
            }
            return evtOnRMDown(this, x, y);
        }

        public bool doEvtOnRMUp(int x, int y)
        {
            if (evtOnRMUp == null)
            {
                return true;
            }
            return evtOnRMUp(this, x, y);
        }

        public bool doEvtOnMMDown(int x, int y)
        {
            if (evtOnMMDown == null)
            {
                return true;
            }
            return evtOnMMDown(this, x, y);
        }

        public bool doEvtOnMMUp(int x, int y)
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

        public bool doEvtOnDClick(int x, int y)
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

            Globals.Instance.mPainter.evtMove += onDragMove;
            Globals.Instance.mPainter.evtLeftUp += onDragEnd;
            return false;
        }

        void onDragEnd(int x, int y)
        {
            Globals.Instance.mPainter.evtMove -= onDragMove;
            Globals.Instance.mPainter.evtLeftUp -= onDragEnd;
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

            Globals.Instance.mPainter.evtMove += onRotateMove;
            Globals.Instance.mPainter.evtRightUp += onRotateEnd;
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
            Globals.Instance.mPainter.evtMove -= onRotateMove;
            Globals.Instance.mPainter.evtRightUp -= onRotateEnd;
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
                    Globals.Instance.mPainter.evtLeftUp += onScaleEnd;
                    //evtOnLMUp += onDragEnd;
                }
                else
                {
                    evtOnLMDown -= onScaleBegin;
                    Globals.Instance.mPainter.evtLeftUp -= onScaleEnd;
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
            Globals.Instance.mPainter.evtOnWheel += onScaleWheel;
            return false;
        }

        void onScaleEnd(int x, int y)
        {
            Globals.Instance.mPainter.evtOnWheel -= onScaleWheel;
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

            adjust();
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
