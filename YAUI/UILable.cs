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
using System.Windows.Forms;
using System.Xml;

namespace ns_YAUI
{
    class UILable : UIWidget
    {
        string mText = "template";
        int mSz = 12;
        Font mFont;//todo, in font manager;
        uint mColor = 0xffffffff;
        Brush mBrush;
        Rectangle mRect = new Rectangle();
        public UILable(string t = "Template", int sz = 12, uint color = 0xffffffff)
        {
            mSz = sz;
            mFont = new Font("Arial", sz, FontStyle.Bold);
            textColor = color;
            text = t;
            var fontsz = TextRenderer.MeasureText(t, mFont);
            mRect = new Rectangle(new Point(0, 0), fontsz);
        }

        public uint textColor
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
                mBrush = new SolidBrush(Color.FromArgb((Int32)mColor));
            }
        }

        public string text
        {
            get
            {
                return mText;
            }

            set
            {
                mText = value;
                var fontsz = TextRenderer.MeasureText(mText, mFont);
                mRect = new Rectangle(new Point(0, 0), fontsz);
                adjustAlign();
            }
        }

        public override Rectangle drawRect
        {
            get
            {
                return mRect;
            }
        }

        public override string typeName
        {
            get { return "string"; }
        }

        public override bool postTestPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            //var td = g.MeasureString(mText, mFont);
            //mRect.Width = (int)td.Width;
            //mRect.Height = (int)td.Height;
            g.DrawString(mText, mFont, mBrush, new PointF());
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            string text = "template";
            int size = 12;
            uint color = 0xffffffff;

            var ret = node.Attributes.GetNamedItem("text");
            if (ret != null) text = ret.Value;

            ret = node.Attributes.GetNamedItem("size");
            if (ret != null) size = ret.Value.castInt(12);

            ret = node.Attributes.GetNamedItem("color");
            if (ret != null) color = ret.Value.castHex(0xffffffff);

            ui = new UILable(text, size, color);
            ui.fromXML(node);
            ui.paresent = p;

            return node.ChildNodes;
        }
    }
}
