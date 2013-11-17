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
using System.Xml;

namespace ns_behaviour
{
    class UIStub : UIWidget
    {
        Rectangle mRect = new Rectangle(-1000000, -1000000, 1000000 * 2, 1000000 * 2);
        public UIStub()
        {
 
        }

        public override Rectangle rect
        {
            get
            {
                Rectangle ret = new Rectangle();
                bool init = false;
                foreach (var elem in children())
                {
                    if (!init)
                    {
                        init = true;
                        ret = elem.rect.transform(elem.transformMatrix);
                    }
                    else
                    {
                        var elemRc = elem.rect.transform(elem.transformMatrix);
                        ret = expandRect(ret, elemRc);
                    }
                }
                return ret;
            }
        }

        public override string typeName
        {
            get { return "stub"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g) 
        {
        }


        public static XmlNodeList fromXML(XmlNode nd, out UIWidget ui, UIWidget p)
        {
            ui = new UIStub();
            ui.paresent = p;
            ui.fromXML(nd);
            return nd.ChildNodes;
        }
    }
}
