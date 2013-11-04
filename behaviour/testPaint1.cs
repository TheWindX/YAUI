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
            GlobalInit.Instance.evtOnInit += main;
        }

        public void main()
        {
            GlobalInit.Instance.mPainter.setSize(1024, 1024);
            GlobalInit.Instance.mPainter.evtLeftDown += (x, y) =>
            {
                GlobalInit.Instance.mRepl.print("position:" + x + ", " + y);
            };
        }


    }
}
