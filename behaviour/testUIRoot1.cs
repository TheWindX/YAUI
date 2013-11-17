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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class testUIRoot1 : Singleton<testUIRoot1>
    {
        public testUIRoot1()
        {
            Globals.Instance.evtOnInit += main;
        }


        public void main()
        {
            UIRoot.Instance.lockable = false;
            UIRoot.Instance.root.mScalex = 4;
            UIRoot.Instance.root.mScaley = 4;
            UIRoot.Instance.root.px = 20;
            UIRoot.Instance.root.py = 20;

            var rc = new UIRect(96, 64);
            rc.paresent = UIRoot.Instance.root;

            rc.dragAble = true;
            UIRoot.Instance.root.scaleAble = true;
        }
    }
}
