using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace ns_GameViewer
{
    class Entity
    {
        internal Entity mParesent = null;
        internal List<Entity> mChildrent = new List<Entity>();//TODO, optims, first child is better
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

        public void setParesent(Entity c)
        {
            if(c == null)
            {
               mParesent.deattach(this);
            }
            else
                c.attach(this);
        }

        public Point mPos = new Point(0, 0);
        public float mdir = 0;//0~360
        public float mScalex = 1;
        public float mScaley = 1;

        public Matrix getAbsMatrix()
        {
            Matrix m = new Matrix();
            if(mParesent != null)
                m = mParesent.getAbsMatrix().Clone();
            m.Translate(mPos.X, mPos.Y);
            m.Rotate(mdir);
            m.Scale(mScalex, mScaley);
            return m;
        }

        public Matrix getLocalMatrix()
        {
            Matrix m = new Matrix();
            m.Translate(mPos.X, mPos.Y);
            m.Rotate(mdir);
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
            if(mParesent == null) return mdir;
            else
            {
                var dir = mParesent.getAbsDir()+mdir;
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

        static Font mDrawFont = new Font("Arial", ViewMap.fontsize, FontStyle.Bold);
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
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb( (int)mStrokeColor ) );
            int sz = size();
            g.DrawString(mAniState.Substring(0, 1), mDrawFont, drawBrush, new Point(-ViewMap.fontsize / 2, -ViewMap.fontsize/2));
            popMatrix(g);
        }

        internal abstract void onDraw(Graphics g);

        string mAniState = "idle";
        int mCurAniStateTimerID = -1;
        public virtual void play(string actName, int aniDur, bool bLoop = false)
        {
            if (mMoveTimerID != -1) ViewWorld.getTimer().clearTimer(mMoveTimerID);//stop run
            if (mCurAniStateTimerID != -1) ViewWorld.getTimer().clearTimer(mCurAniStateTimerID);//stop animation
            mAniState = actName;
            if (!bLoop)
            {
                mCurAniStateTimerID = ViewWorld.getTimer().setTimeout((t) => { mCurAniStateTimerID = -1; mAniState = "idle"; }, aniDur);
            }
        }

        int mMoveTimerID = -1;
        public virtual void runTo(int dx, int dy, int speed)
        {
            if (mMoveTimerID != -1) ViewWorld.getTimer().clearTimer(mMoveTimerID);//stop run
            if (mCurAniStateTimerID != -1) ViewWorld.getTimer().clearTimer(mCurAniStateTimerID);//stop animation
            int x = mPos.X;
            int y = mPos.Y;
            var dist = Math.Sqrt((dx - x) * (dx - x) + (dy - y) * (dy - y));
            float t = ((float)dist / speed)*1000;
            mAniState = "run";
            mMoveTimerID = ViewWorld.getTimer().setInterval((n) => { 
                mPos.X = (int)(x + (dx - x) * ((float)n / t) );
                mPos.Y = (int)(y + (dy - y) * ((float)n / t) ); 
            },
                ViewWorld.mFrameInterval, (n) => { mMoveTimerID = -1; mAniState = "idle"; }, (int)t);
        }
        
        public virtual int size()
        {
            return 0;
        }

        public void ObjectEvent(Logic.MapObject obj, string evt, int param)
        {
            Logic.Character c = obj as Logic.Character;
            if (mMoveTimerID != -1) ViewWorld.getTimer().clearTimer(mMoveTimerID);//stop run
            mAniState = evt;    // Ó¦¸Ãplay
            switch (evt)
            {
                case "run":
                    mPos.X = obj.Position.x;
                    mPos.Y = obj.Position.y;
                    mdir = ViewMap.dir2angle((obj.Target.x - mPos.X), (obj.Target.y - mPos.Y));
                    if (c != null)
                        runTo(obj.Target.x, obj.Target.y, c.Attr("MoveSpeed") * 1000 / Logic.GameMap.FrameInterval);
                    break;
                case "stop":
                    mPos.X = obj.Position.x;
                    mPos.Y = obj.Position.y;
                    break;
                case "removed":
                    ViewMap.removeChar(ID);
                    if (c is Logic.Player)
                        ViewWorld.exit();
                    break;
                case "Start":
                    mVisiable = true;
                    break;
                default:
                    break;
            }
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

    class Rect : Shape
    {
        int mSize = 0;
        
        // Create solid brush.
        public Rect(int size)
        {
            mSize = size;
        }

        internal override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();
            p.AddRectangle(new Rectangle((int)-mSize, (int)-mSize, (int)mSize*2, (int)mSize*2) );
            g.FillPath(mBrush, p);
            //g.DrawPath(mPen, p);
        }

        public override int size()
        {
            return mSize;
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
