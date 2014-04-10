using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class testPackage : Singleton<testPackage>
    {
        public testPackage()
        {
            Packge proot = new Packge("root");
            proot.addPackage("testA");
            proot.addPackage("testB");

            var ui = new UIPackage(proot);
            ui.paresent = UIRoot.Instance.root;
        }
    }
}
