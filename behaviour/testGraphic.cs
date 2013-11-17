
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
    class testGraphic : Singleton<testGraphic>
    {

        public testGraphic()
        {
            Globals.Instance.evtOnInit += main;
        }


        public void main1()
        {
            UIRoot.Instance.root.position = new Point(0, 0);
            
            var edit = new UIEdit();
            edit.position = new Point(128, 128);
            var di1 = UIGSelfEnd.create(EForward.e_left);
            var di2 = UIGSelfEnd.create(EForward.e_right);
            var di3 = UIGSelfEnd.create(EForward.e_up);
            var di4 = UIGSelfEnd.create(EForward.e_down);
            edit.paresent = UIRoot.Instance.root;
            di1.paresent = UIRoot.Instance.root;
            di2.paresent = UIRoot.Instance.root;
            di3.paresent = UIRoot.Instance.root;
            di4.paresent = UIRoot.Instance.root;

            di1.dragAble = true;
            di2.dragAble = true;
            di3.dragAble = true;
            di4.dragAble = true;

            di1.basePos = new Point(512, 256);
            di2.basePos = new Point(0, 256);
            di3.basePos = new Point(256, 512);
            di4.basePos = new Point(256, 0);
            Globals.Instance.mRepl.print(di1.pivotPos.ToString() );
            Globals.Instance.mRepl.print(di1.basePos.ToString());
        }

        public void main()
        {
            UIRoot.Instance.root.position = new Point(0, 0);
            UIRoot.Instance.root.dragAble = true;
            UIRoot.Instance.root.scaleAble = true;

            var v = NamespaceModel.Instance.mRootNs.viewSelf;
            v.paresent = UIRoot.Instance.root;
            v.scaleAble = true;
        }
    }
}
