using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ns_behaviour
{
    class Program
    {
        public static int Main()
        {
            InitAll.main();
            testAll.main();
            GlobalInit.Instance.init();
            return 0;
        }
    }
}
