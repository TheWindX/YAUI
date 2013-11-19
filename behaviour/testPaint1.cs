/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

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
            UIRoot.Instance.root.appendFromXML(@"<rect dragAble=""true""></rect>");
            UIRoot.Instance.root.appendFromXML(@"<rect dragAble=""true""></rect>");
        }


    }
}
