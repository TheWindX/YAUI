using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace ns_behaviour
{
    class Entity
    {
        protected Entity mParesent = null;
        protected List<Entity> mChildrent = new List<Entity>();//TODO, optims, first child is better

        public Entity paresent
        {
            get
            {
                return mParesent;
            }
        }

        public void sortChildrent( Comparison<Entity> pred )
        {
            mChildrent.Sort(pred);
        }

        void deattach(Entity c)
        {
            if(c == null) return;
            if(mChildrent.Contains(c) )
            {
                mChildrent.Remove(c);
                c.mParesent = null;
            }
        }

        void attach(Entity c)
        {
            if(c == null) return;
            if(mChildrent.Contains(c) ) return;
            if(c.mParesent != null)
                c.mParesent.deattach(c);
            
            mChildrent.Add(c);
            c.mParesent = this;
        }

        public virtual void setParesent(Entity c)
        {
            if(c == null)
            {
               mParesent.deattach(this);
            }
            else
                c.attach(this);
        }

        public Point mPos = new Point(0, 0);
        public float mDir = 0;//0~360
        public float mScalex = 1;
        public float mScaley = 1;

        public void scalePoint(Point center, float scaleR)
        {
            var pt = invertTransform(center);
            mScalex += scaleR - 1;
            mScaley += scaleR - 1;

            var pos = transform(pt);
            var offset = new Point(pos.X - center.X,  pos.Y - center.Y);
            mPos = new Point(mPos.X-offset.X, mPos.Y-offset.Y);
        }

        public Point transform(Point pt)
        {
            var pts = new Point[] { pt };
            getLocalMatrix().TransformPoints(pts);
            return pts[0];
        }

        public Point invertTransform(Point pt)
        {
            var t = getLocalMatrix();
            t.Invert();
            var pts = new Point[] { pt };
            t.TransformPoints(pts);
            return pts[0];
        }

        public Matrix getAbsMatrix()
        {
            Matrix m = new Matrix();
            if(mParesent != null)
                m = mParesent.getAbsMatrix().Clone();
            m.Translate(mPos.X, mPos.Y);
            m.Rotate(mDir);
            m.Scale(mScalex, mScaley);
            return m;
        }

        public void Local2Abs(ref Point pt)
        {
            var t = getAbsMatrix();
            var pts = new Point[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
        }

        public void Abs2Local(ref Point pt)
        {
            var t = getAbsMatrix();
            t.Invert();
            var pts = new Point[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
        }

        public void ParesentLocal2Abs(ref Point pt)
        {
            Matrix t;
            if (mParesent != null)
                t = mParesent.getAbsMatrix();
            else t = new Matrix();//ID
            var pts = new Point[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
        }

        public void ParesentAbs2Local(ref Point pt)
        {
            Matrix t;
            if (mParesent != null)
                t = mParesent.getAbsMatrix();
            else t = new Matrix();//ID
            t.Invert();
            var pts = new Point[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
        }

        public Matrix getLocalMatrix()
        {
            Matrix m = new Matrix();
            m.Translate(mPos.X, mPos.Y);
            m.Rotate(mDir);
            m.Scale(mScalex, mScaley);
            return m;
        }

        public Point getAbsPos()
        {
            Matrix m = getAbsMatrix();
            Point p = new Point(0, 0);
            Point[] myArray = {p};
            m.TransformPoints(myArray);
            return p;
        }

        public float getAbsDir()
        {
            if(mParesent == null) return mDir;
            else
            {
                var dir = mParesent.getAbsDir()+mDir;
                if(dir >360) return dir-360;
                return dir;
            }
        }
    }


    abstract class Shape : Entity
    {
        public bool mVisiable = true;
        public int ID = 0;
        Matrix _mtxSave = null;

        internal Shape()
        {   
        }
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
                (e as Shape).doDraw(g);
            }
            //for text drawing
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb( (int)mStrokeColor ) );
            int sz = size();


            popMatrix(g);
        }

        internal abstract void onDraw(Graphics g);

        public virtual int size()
        {
            return 0;
        }

        public UInt32 fillColor
        {
            get { return mFillColor; }
            set
            {
                mFillColor = value;
                mBrush = new SolidBrush(Color.FromArgb((Int32)mFillColor));
            }
        }

        public UInt32 strokeColor
        {
            get { return mStrokeColor; }
            set
            {
                mStrokeColor = value;
                mPen = new Pen(Color.FromArgb((Int32)mStrokeColor));
            }
        }
         
        UInt32 mFillColor = 0xff000000;
        UInt32 mStrokeColor = 0xff000000;
        static SolidBrush mStaticBrush = new SolidBrush(Color.WhiteSmoke);
        static Pen mStaticPen = new Pen(Color.AliceBlue);
        internal SolidBrush mBrush = mStaticBrush;
        internal Pen mPen = mStaticPen;
    }

    class ShapeStub : Shape
    {
        internal override void onDraw(Graphics g)
        {
        }

        public override int size()
        {
            return 0;
        }
    }



    class Circle : Shape
    {
        int mSize = 0;
        // Create solid brush.
        public Circle(int size)
        {
            mSize = size;
        }

        internal override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();
            p.AddArc(-mSize, -mSize, mSize*2, mSize*2, 0, 360);
            g.FillPath(mBrush, p);
            //g.DrawPath(mPen, p);
        }

        public override int size()
        {
            return mSize;
        }
    }


    
}
