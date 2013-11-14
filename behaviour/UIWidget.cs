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
using ns_utils;

namespace ns_behaviour
{
    class UIWidget : Entity
    {
        protected bool mVisiable = true;
        public int ID = 0;

        static int idcount = 0;
        public UIWidget()
        {
            ID = idcount++;
        }


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
                        return 1;
                    }
                    else if (a1.depth == b1.depth)
                    {
                        return 0;
                    }
                    else
                        return -1;
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

        public override void setParesent(Entity c)
        {
            base.setParesent(c);
            sortSibling();
        }

        void setDepthTail()
        {
            var p = mParesent;
            this.setParesent(null);
            this.setParesent(p);
        }

        void setDepthHead()
        {
            mDepth = -1;
            sortSibling();
        }

        void setDepthDeeper()
        {
            mDepth += 1.5f;
            sortSibling();
        }

        void setDepthShallower()
        {
            mDepth -= 1.5f;
            sortSibling();
        }


        Matrix _mtxSave = null;
        void pushMatrix(Graphics g)
        {
            _mtxSave = g.Transform.Clone();
            Matrix _mtx = g.Transform;
            _mtx.Multiply(this.getLocalMatrix());
            g.Transform = _mtx;
        }

        void popMatrix(Graphics g)
        {
            g.Transform = _mtxSave;
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

        //properties start
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
                return mPos.X;
            }
            set
            {
                mPos.X = value;
            }
        }

        public int py
        {
            get
            {
                return mPos.Y;
            }
            set
            {
                mPos.Y = value;
            }
        }

        float mGlobalDepth = 0;
        float mDepth = 0;
        public float depth
        {
            get {return mDepth;}
            set { 
                mDepth = value;
                sortSibling();
            }
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
            int i1 = u1Plist.Count-1;
            int i2 = u2Plist.Count-1;
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

        public Matrix transformMatrix
        {
            get
            {
                Matrix m = new Matrix();
                m.Translate(mPos.X, mPos.Y);
                m.Rotate(mDir);
                m.Scale(mScalex, mScaley);
                return m;
            }
        }

        public virtual Rectangle rect
        {
            get;
            set;
        }

        public Point center
        {
            get
            {
                return transform(rect.center());
            }

            set
            {
                var pt = center;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point leftTop
        {
            get
            {
                return transform(rect.leftTop());
            }
            set
            {
                var pt = leftTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point leftMiddle
        {
            get
            {
                return transform(rect.leftMiddle());
            }
            set
            {
                var pt = leftMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point leftButtom
        {
            get
            {
                return transform(rect.leftButtom());
            }
            set
            {
                var pt = leftButtom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point rightTop
        {
            get
            {
                return transform(rect.rightTop());
            }
            set
            {
                var pt = rightTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point rightMiddle
        {
            get
            {
                return transform(rect.rightMiddle());
            }
            set
            {
                var pt = rightMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point rightButtom
        {
            get
            {
                return transform(rect.rightButtom());
            }
            set
            {
                var pt = rightButtom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point middleTop
        {
            get
            {
                return transform(rect.middleTop());
            }
            set
            {
                var pt = middleTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public Point middleButtom
        {
            get
            {
                return transform(rect.middleButtom());
            }
            set
            {
                var pt = middleButtom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                mPos.X += dx;
                mPos.Y += dy;
            }
        }

        public void offset(int dx, int dy)
        {
            mPos.X += dx;
            mPos.Y += dy;
        }

        public Dictionary<string, object> attrs = new Dictionary<string,object>();

        bool mClip = false;
        public bool clip { get{return mClip;} set{mClip = value;} }

        bool mEnable = true;
        public bool enable { get{return mEnable;} set{mEnable = value;} }

        public string name
        {
            get;
            set;
        }

        public virtual string typeName
        {
            get{return "nulltype";}
        }

        public bool visiable
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
        //properties end


        public virtual bool testPick(Point pos)
        {
            return false;
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

            var r = rect;
            if (!r.Contains(newpos))
            {
                ui = null;
                return false;
            }

            List<UIWidget> uilist = new List<UIWidget>();
            foreach (UIWidget elem in children() )
            {
                UIWidget picked = null;
                if (elem.doTestPick(newpos, out picked, testEnable))
                {
                    uilist.Add(picked);
                }
            }
            uilist.Sort((a, b) =>
                {
                    if (a.mGlobalDepth < b.mGlobalDepth)
                    {
                        return 1;
                    }
                    else if (a.mGlobalDepth < b.mGlobalDepth)
                    {
                        return 0;
                    }
                    else
                        return -1;
                });

            if (uilist.Count != 0)
            {
                ui = uilist[0];
                return true;
            }
            else
            {
                bool ret = testPick(newpos);
                if (ret)
                {
                    ui = this;
                    return true;
                }
                ui = null;
                return false;
            }
        }

        IEnumerable<UIWidget> children()
        {
            for (int i = mChildrent.Count-1; i >= 0; --i)
            {
                yield return mChildrent[i] as UIWidget;
            }
        }

        public static Font mDrawFont = new Font("Arial", 12, FontStyle.Bold);
        internal void doDraw(Graphics g)
        {
            if (!mVisiable)
                return;
            var gs = g.Save();
            pushMatrix(g);
            if (clip)
            {
                var r = rect;
                GraphicsPath p = new GraphicsPath();
                p.AddRectangle(r);
                g.SetClip(p, CombineMode.Intersect);
            }

            //catch it?
            onDraw(g);

            foreach (Entity e in mChildrent)
            {
                (e as UIWidget).doDraw(g);
            }
            //for text drawing            

            g.Restore(gs);
            //popMatrix(g);
        }

        internal virtual void onDraw(Graphics g){}


        public delegate bool EvtMouse(UIWidget _this, int x, int y);

        public delegate bool EvtKeyboard(UIWidget _this, int kc);

        public EvtMouse evtOnLMDown;
        public EvtMouse evtOnLMUp;
        public EvtMouse evtOnRMDown;
        public EvtMouse evtOnRMUp;
        public EvtMouse evtOnMMDown;
        public EvtMouse evtOnMMUp;
        public EvtMouse evtOnEnter;
        public EvtMouse evtOnExit;
        public EvtKeyboard evtOnChar;

        //for test 

        bool mDragAble = false;
        public bool dragAble
        {
            set
            {
                mDragAble = value;
                if (mDragAble)
                {
                    evtOnLMDown += onDragStart;
                    //evtOnLMUp += onDragEnd;
                }
                else
                {
                    evtOnLMDown -= onDragStart;
                    onDragEnd(0, 0);
                }
                
            }

            get
            {
                return mDragAble;
            }
        }



        Point moveStart;
        Point rcStart;
        bool mBDragDown = false;
        bool onDragStart(UIWidget _this, int x, int y)
        {
            if (mBDragDown) return true;
            mBDragDown = true;

            _this.setDepthHead();
            var pt = new Point(x, y);
            
            pt = invertTransformParesentAbs(pt);

            moveStart = pt;

            rcStart = mPos;

            Globals.Instance.mPainter.evtMove += this.onMove;
            Globals.Instance.mPainter.evtLeftUp += this.onDragEnd;
            return false;
        }

        void onDragEnd(int x, int y)
        {
            if (!mBDragDown) return;
            mBDragDown = false;
            Globals.Instance.mPainter.evtMove -= this.onMove;
        }

        void onMove(int x, int y)
        {
            Point pt = new Point(x, y);
            pt = invertTransformParesentAbs(pt);

            px = rcStart.X + pt.X - moveStart.X;
            py = rcStart.Y + pt.Y - moveStart.Y;
        }

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
    }
}
