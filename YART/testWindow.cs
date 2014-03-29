using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    class testWindow : ns_YAUtils.Singleton<testWindow>
    {
        public testWindow()
        {
            var ins = windows.Instance;
        }
    }
}
