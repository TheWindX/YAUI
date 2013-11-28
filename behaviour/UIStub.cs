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
        public override Rectangle drawRect
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
                        ret = elem.drawRect.transform(elem.transformMatrix);
                    }
                    else
                    {
                        var elemRc = elem.drawRect.transform(elem.transformMatrix);
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

        public override bool postTestPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g) 
        {
        }


        public static XmlNodeList fromXML(XmlNode nd, out UIWidget ui, UIWidget p)
        {
            ui = new UIStub();
            ui.fromXML(nd);
            ui.paresent = p;
            return nd.ChildNodes;
        }
    }
}
