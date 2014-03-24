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

namespace ns_YAUI
{
    class UIBlank : UIWidget
    {
        int _w = 20;
        int _h = 20;
        public override Rectangle drawRect
        {
            get
            {
                return new Rectangle(0, 0, _w, _h);
            }
        }

        public override string typeName
        {
            get { return "blank"; }
        }

        public override int width
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
                setDirty();
            }
        }

        public override int height
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
                setDirty();
            }
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIBlank();
            ui.fromXML(node);

            var ret = node.Attributes.GetNamedItem("height");
            if (ret != null)
            {
                ui.height = ret.Value.castInt();
            }
            ret = node.Attributes.GetNamedItem("width");
            if (ret != null)
            {
                ui.width = ret.Value.castInt();
            }

            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
