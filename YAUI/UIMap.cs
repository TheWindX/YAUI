

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

        public override Rectangle drawRect
        {
            get
            {
                return new Rectangle(-1000000, -1000000, 2000000, 2000000);
            }
        }

        public override string typeName
        {
            get { return "map"; }
        }

        public override bool pickRectTest
        {
            get
            {
                return false;
            }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        public override void onDraw(Graphics g) 
        {
            g.FillRectangle(mBrush, drawRect);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            uint fc = 0;
            XmlNode ret = node.Attributes.GetNamedItem("color");
            string strRet = (ret == null) ? UIRoot.Instance.getProperty("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setProperty("color", strRet);
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

