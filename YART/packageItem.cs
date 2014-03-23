using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    public interface iPackageItem
    {
        string name { get; set; }
        List<CPacakge> locations();
    }

    public class implPackageItem
    {
        public string name
        {
            get;
            set;
        }
        List<CPacakge> mParesents = new List<CPacakge>();
        public List<CPacakge> locations()
        {
            return mParesents;
        }
    }

    public class CPacakge : iPackageItem
    {
        #region iPackageItem impl
        implPackageItem mImpl = new implPackageItem();
        string iPackageItem.name
        {
            get { return mImpl.name; }
            set { mImpl.name = value; }
        }

        List<CPacakge> iPackageItem.locations()
        {
            return mImpl.locations();
        }
        #endregion

        List<iPackageItem> mItems = new List<iPackageItem>();
        public void addItem(iPackageItem item)
        {
            mItems.Add(item);
        }

        public void removeItem(iPackageItem item)
        {
            mItems.Remove(item);
        }

        public void addPackage(string packageName)
        {
            CPacakge pkg = new CPacakge();
            (pkg as iPackageItem).name = packageName;
            addItem(pkg);
        }
    }
}
