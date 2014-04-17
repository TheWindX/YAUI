using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace YAUIUser
{
    using ns_YAUI;
    class Program
    {
        static void Main(string[] args)
        {
            UI.Instance.init();
            var ui = UI.Instance.fromXML(@"
<rect></rect>
");
            var tall = testAll.Instance;
            UI.Instance.run();
        }
    }
}
