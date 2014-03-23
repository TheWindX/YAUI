using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YART
{
    class runtime : Singleton<runtime>
    {
        public CPacakge mRoot;
        public runtime()
        {
            mRoot = new CPacakge();
            (mRoot as iPackageItem).name = "root";

            mRoot.addItem(CInt_t.instance);
        }
    }
}
