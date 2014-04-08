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
    public class UIBlank : UIWidget
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
            }
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIBlank();
            ui.fromXML(node);

            var ret = node.Attributes.GetNamedItem("length");
            string strRet = (ret == null) ? UIRoot.Instance.getProperty("length") : ((ret.Value == "NA") ? null : ret.Value);
            UIRoot.Instance.setProperty("length", ref strRet);
            if (strRet != null)
            {
                ui.width = strRet.castInt();
                ui.height = ui.width;
            }

            ret = node.Attributes.GetNamedItem("width");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("width") : ((ret.Value == "NA") ? null : ret.Value);
            UIRoot.Instance.setProperty("width", ref strRet);
            if (strRet != null)
            {
                ui.width = strRet.castInt();
            }

            ret = node.Attributes.GetNamedItem("height");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("height") : ((ret.Value == "NA") ? null : ret.Value);
            UIRoot.Instance.setProperty("height", ref strRet);
            if (strRet != null)
            {
                ui.height = strRet.castInt();
            }

            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
