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
        public bool mVisiable = true;
        public int ID = 0;
        

        static int idcount = 0;
        public UIWidget()
        {
            ID = idcount++;
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

        float mDepth = 0;
        public float depth
        {
            get {return mDepth;}
            set { mDepth = value; }
        }



        public Matrix transformMatrix
        {
            get
            {
                Matrix m = new Matrix();
                m.Translate(mPos.X, mPos.Y);
                m.Rotate(mdir);
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

        public bool bclip = false;

        public int id = -1;

        public string name
        {
            get;
            set;
        }

        public virtual string typeName
        {
            get{return "nulltype";}
        }

        public bool bshow
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

        public virtual bool testPick(Point pos)
        {
            return false;
        }

        public bool doTestPick(Point pos, out UIWidget ui)
        {
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
                if (a.depth < b.depth)
                {
                    return 1;
                }
                else if (a.depth == b.depth)
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
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                yield return mChildrent[i] as UIWidget;
            }
        }

        static Font mDrawFont = new Font("Arial", 12, FontStyle.Bold);
        internal void doDraw(Graphics g)
        {
            if (!mVisiable)
                return;
            pushMatrix(g);
            //catch it?
            onDraw(g);
            foreach (Entity e in mChildrent)
            {
                (e as UIWidget).doDraw(g);
            }
            //for text drawing            
            
            popMatrix(g);
        }

        internal virtual void onDraw(Graphics g){}




        public delegate bool EvtOnLMDown(int x, int y);
        public delegate bool EvtOnLMUp(int x, int y);
        public delegate bool EvtOnRMDown(int x, int y);
        public delegate bool EvtOnRMUp(int x, int y);
        public delegate bool EvtOnMMDown(int x, int y);
        public delegate bool EvtOnMMUp(int x, int y);
        public delegate bool EvtOnEnter(int x, int y);
        public delegate bool EvtOnExit(int x, int y);
        public delegate bool EvtOnChar(int kc);

        public EvtOnLMDown evtOnLMDown;
        public EvtOnLMUp evtOnLMUp;
        public EvtOnRMDown evtOnRMDown;
        public EvtOnRMUp evtOnRMUp;
        public EvtOnMMDown evtOnMMDown;
        public EvtOnMMUp evtOnMMUp;
        public EvtOnEnter evtOnEnter;
        public EvtOnExit evtOnExit;
        public EvtOnChar evtOnChar;
    }
}
