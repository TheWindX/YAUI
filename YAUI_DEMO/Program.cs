using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ns_YAUI;
using ns_YAUtils;

namespace ns_YAUIUser
{

    class Program
    {
        static void Main(string[] args)
        {
            UI.Instance.init().setAntiAliasing(true);
            var tall = testAll.Instance;
            UI.Instance.run();
        }
    }
}
