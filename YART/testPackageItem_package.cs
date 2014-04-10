using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class testPackageItem_package : Singleton<testPackageItem_package>
    {
        public testPackageItem_package()
        {
            var ui = new UIPackageItem_package(null);
            ui.name = "testPackage";
            ui.paresent = UIRoot.Instance.root;
        }
    }
}
