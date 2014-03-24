using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace YAUIUser
{
    class testAll : Singleton<testAll>
    {
        public testAll()
        {
            //var _1 = testRoundRect.Instance;
            //var _2 = testLayout.Instance;
            var _3 = testWindow.Instance;
        }
    }
}
