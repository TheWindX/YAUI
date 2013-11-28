using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class MNamespace : iModelName
    {
        #region interface
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

        string mType = "NS";
        public string type
        {
            get
            {
                return mType;
            }
            set
            {
                mName = mType;
            }
        }

        NamespaceViewSelf mViewSelf;
        public iViewerSelf getViewerSelf()
        {
            mViewSelf = new NamespaceViewSelf(this);
            return mViewSelf;
        }

        public iViewerNameItem getViewerNameItem()
        {
            return new NamespaceViewInstance(this);
        }

        #endregion

        public MNamespace(string name_)
        {
            name = name_;
        }

        protected MNamespace mParesent = null;
        protected List<iModelName> mChildrent = new List<iModelName>();//TODO, optims, 

        public IEnumerable<iModelName> enumChildrent()
        {
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                yield return mChildrent[i];
            }
        }


        public void sortChildrent(Comparison<iNameItem> pred)
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

        public MNamespace paresent
        {
            set
            {
                if (value == null)
                {
                    mParesent.deattach(this);
                }
                else
                    value.attach(this);
            }
        }




        //public void reflushViewSelf()
        //{
        //    if (mViewSelf != null)
        //    {
        //        //mViewSelf.paresent = null;
        //        mViewSelf.reflush();
        //    }
        //    //var oldView = mViewSelf;


        //    //var flushed = viewSelf;
        //    //flushed.px = oldView.px;
        //    //flushed.py = oldView.py;
            

        //    flushed.paresent = UIRoot.Instance.root;
        //}

        public UIWidget viewInstance
        {
            get
            {
                return new NamespaceViewInstance(this);
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
                paresent = value;
            }
        }

        public void addNameSpace(string name_)
        {
            var ns = new MNamespace(name_);
            ns.namespaceParesent = this;
        }

        public void addItem(iModelName item)
        {
            mChildrent.Add(item);
        }
    }

    class NamespaceModel : Singleton<NamespaceModel>
    {
        //此处为全局root
        public MNamespace mRootNs = new MNamespace("root");
    }
}
