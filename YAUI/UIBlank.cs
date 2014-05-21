/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: a rect container for UIs, can handler event & trasform, but cannot be seen
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace ns_YAUI
{
    public class UIBlank : UIWidget
    {
        float _w = 20;
        float _h = 20;
        public override RectangleF drawRect
        {
            get
            {
                return new RectangleF(0, 0, _w, _h);
            }
        }

        //test children directly
        //public override bool testRectPick
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        public override string typeName
        {
            get { return "blank"; }
        }

        public override float width
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
                UIRoot.Instance.mLayoutUpdate = true;
            }
        }

        public override float height
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
                UIRoot.Instance.mLayoutUpdate = true;
            }
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIBlank();
            //int w, h;
            //bool br = true;

            //h = w = getAttr<float>(node, "length", schemes.width, out br);
            //if (!br)
            //{
            //    w = getAttr<float>(node, "width", schemes.width, out br);
            //    h = getAttr<float>(node, "height", schemes.height, out br);
            //}

            //ui.width = w;
            //ui.height = h;
            ui.fromXML(node);

            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
