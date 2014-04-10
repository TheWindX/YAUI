/* author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    //any type
    public class nullType : InheriteBase
    {
        public override Type[] deriveFrom()
        {
            return new Type[]{};
        }
    }

    public class PackageItem : InheriteBase
    {
        public override string stringForm(int space)
        {
            return base.stringForm(space) + name;
        }

        public override Type[] deriveFrom()
        {
            return new Type[]{ typeof(nullType) };
        }

        /// methods
        public string name
        {
            get;
            set;
        }
        List<Packge> mParesents = new List<Packge>();
        public List<Packge> locations()
        {
            return mParesents;
        }

        //to be overrided
        //ui in package
        public Func<UIWidget> funcGetUiWidget = () => null;
    }

    public class Packge : InheriteBase
    {
        public override Type[] deriveFrom()
        {
            return new Type[] { typeof(PackageItem) };
        }

        public override string stringForm(int space)
        {
            string p = cast<PackageItem>().stringForm(space);
            return mItems.Aggregate(p, 
                (acc, item)=>{
                    string ret = acc + "\n";
                    ret += item.self.stringForm(space + 1);
                    return ret;
                }
            );
            
        }

        List<PackageItem> mItems = new List<PackageItem>();


        public Packge()
        {
            var pkg = cast<PackageItem>();
            pkg.funcGetUiWidget = () =>
            {
                return new UIPackageItem_package(this);
            };
        }

        public Packge(string name):this()
        {
            var selfItem = cast<PackageItem>();
            selfItem.name = name;
        }

        /// methods
        public bool addItem(PackageItem item)//if true, it is in package
        {
            var ls = item.locations();
            if (!ls.Contains(this))
            {
                mItems.Add(item);
                ls.Add(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool removeItem(PackageItem item)
        {
             var ls = item.locations();
             if (!ls.Contains(this))
             {
                 return false;
             }
            mItems.Remove(item);
            ls.Remove(this);
            return true;
        }

        public Packge addPackage(string pname)
        {
            var p = new Packge();
            var it = p.cast<PackageItem>();
            it.name = pname;
            if (addItem(it))
            {
                return p;
            }
            else
                return null;
        }

        public bool removePacage(string pname)
        {
            var item = this.mItems.First(it => it.name == pname);
            if (item == null) return false;
            return removeItem(item);
        }

        public List<PackageItem> getItems()
        {
            return mItems;
        }

        UIWidget getUI()
        {
            return new UIPackage(this);
        }
    }
}
