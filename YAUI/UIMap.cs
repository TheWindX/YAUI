/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: an infinite space for UIs, can handler event & trasform, but no seen
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
    public class UIMap : UIWidget
    {
        Brush mBrush = null;
        uint mColor;
        public UIMap(uint c=0xff000000)
        {
            mColor = c;
            mBrush = new SolidBrush(Color.FromArgb((int)c));
            testPickRectExclude = false;
        }


        public uint color
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
                mBrush = new SolidBrush(Color.FromArgb((int)mColor));
            }
        }

        public override float width
        {
            get
            {
                return 1000000;
            }
            set
            {
            }
        }

        public override float height
        {
            get
            {
                return 1000000;
            }
            set
            {
            }
        }

        public override RectangleF drawRect
        {
            get
            {
                return new RectangleF(-1000000, -1000000, 2000000, 2000000);
            }
        }

        public override string typeName
        {
            get { return "map"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            return true;
        }

        public override bool onDraw(Graphics g) 
        {
            g.FillRectangle(mBrush, drawRect);
            return true;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            uint fc = 0;
            XmlNode ret = node.Attributes.GetNamedItem("color");
            string strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setPropertyInTable("color", ref strRet);
            }

            var m = new UIMap();
            m.color = fc;
            ui = m;
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}

