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
    public enum EStyle
    {
        normal,
        bold,
        italic,
    }

    public class UILable : UIWidget
    {
        string mText = "template";
        int mSz = 12;
        Font mFont;//todo, in font manager;
        uint mColor = 0xffffffff;
        Brush mBrush;
        Rectangle mRect = new Rectangle();
        
        EStyle mStyle = EStyle.normal;

        public UILable(string t = "Template", int sz = 12, EStyle st = EStyle.normal, uint color = 0xffffffff)
        {
            mStyle = st;
            mSz = sz;
            textColor = color;
            if (mStyle == EStyle.normal)
                mFont = new Font("Arial", sz, FontStyle.Regular);
            else if (mStyle == EStyle.bold)
                mFont = new Font("Arial", sz, FontStyle.Bold);
            else if (mStyle == EStyle.italic)
                mFont = new Font("Arial", sz, FontStyle.Italic);
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
            get { return "lable"; }
        }

        public override bool testSelfPick(Point pos)
        {
            return true;
        }

        public override void onDraw(Graphics g)
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
            EStyle style = EStyle.normal;
            bool br = false;

            text = getAttr(node, "text", "template", out br);
            size = getAttr(node, "size", 12, out br);
            color = (uint)getAttr<EColorUtil>(node, "color", EColorUtil.silver, out br);
            if (!br)
            {
                color = getAttr(node, "color", (uint)(EColorUtil.silver), out br);
            }
            style = getAttr(node, "color", EStyle.normal, out br);

            ui = new UILable(text, size, style, color);
            ui.fromXML(node);
            ui.paresent = p;

            return node.ChildNodes;
        }
    }
}
