using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YART
{
    class testAll : Singleton<testAll>
    {
        public testAll()
        {
            UIPackageTemplate.genWidget();
        }
    }
}
