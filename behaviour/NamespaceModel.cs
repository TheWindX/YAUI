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
        string name { get;  }
        string type { get;  }
        
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


        protected MNamespace mParesent = null;
        protected List<MNamespace> mChildrent = new List<MNamespace>();//TODO, optims, 

        public IEnumerable<MNamespace> enumChildrent()
        {
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                yield return mChildrent[i];
            }
        }


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


        NamespaceViewSelf mViewSelf; 
        public UIWidget viewSelf
        {
            get
            {
                mViewSelf = new NamespaceViewSelf(this);
                return mViewSelf;
            }
        }

        public void reflushViewSelf()
        {
            if (mViewSelf != null)
            {
                mViewSelf.paresent = null;
            }
            var flushed = viewSelf;
            flushed.paresent = UIRoot.Instance.root;
        }

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
    }

    class NamespaceModel : Singleton<NamespaceModel>
    {
        //此处为全局root
        public MNamespace mRootNs = new MNamespace("root");
    }
}
