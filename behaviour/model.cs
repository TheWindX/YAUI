using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    interface iNameItem
    {
        string name { get; set; }
        MNamespace namespaceParesent {get; set;}

        UIWidget viewSelf
        {
            get;
        }

        UIWidget viewInstance
        {
            get;
        }
    }

    class MNamespace : iNameItem
    {
        public MNamespace(string name_)
        {
            name = name_;
        }

        string mName;
        public string name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        protected MNamespace mParesent = null;
        protected List<MNamespace> mChildrent = new List<MNamespace>();//TODO, optims, 

        public void sortChildrent(Comparison<MNamespace> pred)
        {
            mChildrent.Sort(pred);
        }

        void deattach(MNamespace c)
        {
            if (c == null) return;
            if (mChildrent.Contains(c))
            {
                mChildrent.Remove(c);
                c.mParesent = null;
            }
        }

        void attach(MNamespace c)
        {
            if (c == null) return;
            if (mChildrent.Contains(c)) return;
            if (c.mParesent != null)
                c.mParesent.deattach(c);

            mChildrent.Add(c);
            c.mParesent = this;
        }

        public virtual void setParesent(MNamespace c)
        {
            if (c == null)
            {
                mParesent.deattach(this);
            }
            else
                c.attach(this);
        }

        class ViewerInstanceMNamespace : UIRect
        {
            public UILable mLable;
            public MNamespace mModel;

            public ViewerInstanceMNamespace(MNamespace self)
                : base(96, 64)
            {
                mModel = self;
                mLable = new UILable(mModel.name, 12, 0xffaaaaaa);
                mLable.enable = false;
                mLable.setParesent(this);
                adjust();

                evtOnMMUp += onMUp;
            }

            void adjust()
            {
                mLable.center = this.center;
            }

            public bool onMUp(UIWidget _this, int x, int y)
            {
                var edit = Globals.Instance.mPainter.textEdit;
                edit.show(true, x, y);
                edit.evtInputExit += (text) =>
                    {
                        mModel.name = text;
                        mLable.text = text;
                        adjust();
                    };
                return false;
            }
        }


        class ViewerSelfMNamespace : UIRect
        {
            public UILable mLable;
            public MNamespace mModel;

            public List<UIWidget> mItems = new List<UIWidget>();

            public ViewerSelfMNamespace(MNamespace self)
                : base(500, 500)
            {
                mModel = self;
                mLable = new UILable(mModel.name, 12, 0xffaaaaaa);
                mLable.enable = false;
                mLable.setParesent(this);
                mLable.mScalex = 4;
                mLable.mScaley = 4;
                adjust();

                evtOnMMUp += onMUp;
                evtOnRMUp += onRUp;
            }

            void adjust()
            {
                mLable.leftTop = this.leftTop;
                mLable.mPos.X += 2;
                mLable.mPos.Y += 2;

                for (int i = 0; i < mItems.Count; ++i)
                {
                    mItems[i].setParesent(null);
                }
                mItems.Clear();

                int dx = 4;
                int dy = 4;
                int x = dx;
                int y = dy;
                int yNext = y;

                for (int i = 0; i < mModel.mChildrent.Count; ++i)
                {
                    var viewItem = mModel.mChildrent[i].viewInstance;
                    viewItem.setParesent(this);
                    viewItem.leftTop = new Point(x, y);
                    var pt = viewItem.rightButtom;
                    if (pt.X > rect.Width)
                    {
                        x = dx;
                        y = yNext;
                        viewItem.leftTop = new Point(width, height);    
                    }
                    else
                    {
                        var ptrb = viewItem.rightButtom;
                        x = ptrb.X + dx;
                        yNext = ptrb.Y + dy;
                    }
                }
            }

            Action<string> mMUpAction = null;
            public bool onMUp(UIWidget _this, int x, int y)
            {
                var edit = Globals.Instance.mPainter.textEdit;
                edit.show(true, x, y);
                edit.evtInputExit = (text) =>
                {
                    mModel.name = text;
                    mLable.text = text;
                    adjust();
                };
                return false;
            }


            public bool onRUp(UIWidget _this, int x, int y)
            {
                var edit = Globals.Instance.mPainter.textEdit;
                edit.show(true, x, y);
                edit.evtInputExit = (text) =>
                {
                    mModel.addNameSpace(text);
                    mModel.reflushViewSelf();
                };

                return false;
            }
        }

        ViewerSelfMNamespace mViewSelf; 
        public UIWidget viewSelf
        {
            get
            {
                mViewSelf = new ViewerSelfMNamespace(this);
                return mViewSelf;
            }
        }

        public void reflushViewSelf()
        {
            if (mViewSelf != null)
            {
                mViewSelf.setParesent(null);
            }
            var flushed = viewSelf;
            flushed.setParesent(UIRoot.Instance.mRoot);
        }

        public UIWidget viewInstance
        {
            get
            {
                return new ViewerInstanceMNamespace(this);
            }
        }

        public MNamespace namespaceParesent 
        { 
            get
            {
                return mParesent;
            }
            set
            {
                setParesent(value);
            }
        }

        public void addNameSpace(string name_)
        {
            var ns = new MNamespace(name_);
            ns.namespaceParesent = this;
        }
    }

    class model : Singleton<model>
    {
        public MNamespace mRootNs = new MNamespace("root");
    }
}
