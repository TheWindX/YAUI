using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace ns_YAUI
{

    public class Entity
    {
        protected Entity mParesent = null;
        protected List<Entity> mChildrent = new List<Entity>();//TODO, optims, 

        public virtual Entity paresent
        {
            get
            {
                return mParesent;
            }
            set
            {
                if (mParesent != null)
                    mParesent.deattach(this);
                if (value != null)
                {   
                    value.attach(this);
                }
            }
        }

        public virtual void clear()
        {
            List<Entity> cCopy = new List<Entity>();
            foreach (var ent in mChildrent)
            {
                cCopy.Add(ent);
            }

            foreach (var ent in cCopy)
            {
                ent.paresent = null;
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

        PointF mPosition = new PointF();
        //public PointF position
        //{
        //    get
        //    {
        //        return mPosition;
        //    }
        //    set
        //    {
        //        mPosition = value;
        //    }
        //}

        public float px
        {
            get
            {
                //if (this.GetType() == typeof(UIMap)) { int i = 0; }
                return mPosition.X;
            }
            set
            {
                //if(this.GetType() == typeof(UIMap) ) {int i = 0;}
                mPosition.X = value;
            }
        }

        public float py
        {
            get
            {
                return mPosition.Y;
            }
            set
            {
                mPosition.Y = value;
            }
        }

        public float direction = 0;//0~360
        public void rotateSelf(float dir)
        {
            direction += dir;
            direction = direction % 360;
        }

        public void rotate(float dir)
        {
            var m = new Matrix();
            m.RotateAt(dir, new PointF(-px, -py));

            direction = direction + dir;
            direction = direction % 360;

            px = px + m.OffsetX;
            py = py + m.OffsetY;
        }

        public void rotatePointF(PointF center/* abs position */, float dir)
        {
            var fp = invertTransformAbs(center);
            direction += dir;
            direction = direction % 360;

            var ppt = invertTransformParesentAbs(center);

            Matrix m = new Matrix();
            m.Rotate(direction);
            m.Scale(mScalex, mScaley);

            var ppt1 = fp.transform(m);

            px = ppt.X - ppt1.X;
            py = ppt.Y - ppt1.Y;
        }


        protected  float mScalex = 1;
        protected float mScaley = 1;

        public void setScale(float sx, float sy)
        {
            mScalex = sx;
            mScaley = sy;
        }

        public void scalePointF(PointF center, float scaleR)
        {
            var fp = invertTransformAbs(center);
            mScalex += scaleR - 1;
            mScaley += scaleR - 1;

            var ppt = invertTransformParesentAbs(center);

            Matrix m = new Matrix();
            m.Rotate(direction);
            m.Scale(mScalex, mScaley);

            var ppt1 = fp.transform(m);

            px = ppt.X - ppt1.X;
            py = ppt.Y - ppt1.Y;
        }

        public PointF transform(PointF pt)
        {
            var pts = new PointF[] { pt };
            getLocalMatrix().TransformPoints(pts);
            return pts[0];
        }

        public PointF invertTransform(PointF pt)
        {
            var t = getLocalMatrix();
            t.Invert();
            var pts = new PointF[] { pt };
            t.TransformPoints(pts);
            return pts[0];
        }

        public Matrix getAbsMatrix()
        {
            Matrix m = new Matrix();
            if(mParesent != null)
                m = mParesent.getAbsMatrix().Clone();
            m.Translate(px, py);
            m.Rotate(direction);
            m.Scale(mScalex, mScaley);
            return m;
        }

        public PointF transformAbs(PointF pt)
        {
            var t = getAbsMatrix();
            var pts = new PointF[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
            return pt;
        }

        public PointF invertTransformAbs(PointF pt)
        {
            var t = getAbsMatrix();
            t.Invert();
            var pts = new PointF[] { pt };
            var m = new Matrix();
            t.TransformPoints(pts);
            pt = pts[0];
            return pt;
        }

        public PointF transformParesentAbs(PointF pt)
        {
            Matrix t;
            if (mParesent != null)
                t = mParesent.getAbsMatrix();
            else t = new Matrix();//ID
            var pts = new PointF[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
            return pt;
        }

        public PointF invertTransformParesentAbs(PointF pt)
        {
            Matrix t;
            if (mParesent != null)
                t = mParesent.getAbsMatrix();
            else t = new Matrix();//ID
            t.Invert();
            var pts = new PointF[] { pt };
            t.TransformPoints(pts);
            pt = pts[0];
            return pt;
        }

        public Matrix getLocalMatrix()
        {
            Matrix m = new Matrix();
            m.Translate(px, py);
            m.Rotate(direction);
            m.Scale(mScalex, mScaley);
            
            return m;
        }

        PointF fp;
        public void beginFixPoint(float x, float y)
        {
            fp = invertTransformAbs(new PointF(x, y));
        }

        public void updateFixPoint(float x, float y)
        {
            var delta = updateFixPointDelta(x, y);
            px = px + delta.X;
            py = py + delta.Y;
        }

        public PointF updateFixPointDelta(float x, float y)
        {
            var ppt = invertTransformParesentAbs(new PointF(x, y));
            Matrix m = new Matrix();
            m.Rotate(direction);
            m.Scale(mScalex, mScaley);

            var pts = new PointF[] { fp };
            m.TransformPoints(pts);
            var ppt1 = pts[0];
            return new PointF(ppt.X - ppt1.X-px, ppt.Y - ppt1.Y-py);
        }
    }
}
