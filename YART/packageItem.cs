using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    public class any : InheriteBase
    {
        public override Type[] inheritFrom()
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

        public string name
        {
            get;
            set;
        }
        List<Pacakge> mParesents = new List<Pacakge>();
        public List<Pacakge> locations()
        {
            return mParesents;
        }

        public override Type[] inheritFrom()
        {
            return new Type[]{ typeof(any) };
        }
    }

    public class Pacakge : InheriteBase
    {
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

        public Pacakge addPackage(string pname)
        {
            var p = new Pacakge();
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

        public override Type[] inheritFrom()
        {
            return new Type[] { typeof(PackageItem) };
        }
    }
}
