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

        //properties start
        public bool focus
        {
            get;
            set;
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

        public Dictionary<string, object> attrs = new Dictionary<string,object>();

        public bool bClip = false;

        public bool bEnable = true;

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

        public bool doTestPick(Point pos, out UIWidget ui)
        {
            if (!bEnable)
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
                if(elem.doTestPick(newpos, out picked) )
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
            if (bClip)
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

        bool onDragStart(UIWidget _this, int x, int y)
        {
            _this.setDepthHead();
            var pt = new Point(x, y);
            
            ParesentAbs2Local(ref pt);

            moveStart = pt;

            rcStart = mPos;

            Globals.Instance.mPainter.evtMove += this.onMove;
            Globals.Instance.mPainter.evtLeftUp += this.onDragEnd;
            return false;
        }

        void onDragEnd(int x, int y)
        {
            Globals.Instance.mPainter.evtMove -= this.onMove;
        }

        void onMove(int x, int y)
        {
            Point pt = new Point(x, y);
            ParesentAbs2Local(ref pt);

            px = rcStart.X + pt.X - moveStart.X;
            py = rcStart.Y + pt.Y - moveStart.Y;
        }


    }
}
