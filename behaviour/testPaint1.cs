using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_behaviour
{
    class testPaint1
    {
        public static testPaint1 ins = new testPaint1();
        testPaint1()
        {
            Globals.Instance.evtOnInit += main;
        }

        public void main()
        {
            Globals.Instance.mPainter.setSize(1024, 1024);
            Globals.Instance.mPainter.evtLeftDown += (x, y) =>
            {
                Globals.Instance.mRepl.print("position:" + x + ", " + y);
            };
        }


    }
}
