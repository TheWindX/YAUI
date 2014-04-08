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
    public class UIRound : UIWidget
    {
        int _w;
        int _h;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;


        internal UIRound(float w, float h, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            _w = (int)w;
            _h = (int)h;
            scolor = stroke;
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            mPen = new Pen(Color.FromArgb((Int32)scolor));
        }

        public uint fillColor
        {
            get
            {
                return fcolor;
            }
            set
            {
                fcolor = value;
                mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            }
        }

        public uint strokeColor
        {
            get
            {
                return scolor;
            }
            set
            {
                scolor = value;
                mPen = new Pen(Color.FromArgb((Int32)scolor));
            }
        }

        public int width
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

        public int height
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

        public void setSize(int w, int h)
        {
            _w = w;
            _h = h;
        }

        public override Rectangle drawRect
        {
            get
            {
                return new Rectangle(0, 0, _w, _h);
            }
        }

        public override string typeName
        {
            get { return "round"; }
        }

        public override bool testPick(Point pos)
        {
            float a = (float)_w / 2;
            float b = (float)_h / 2;
            float x = (float)(pos.X - a);
            float y = (float)(pos.Y - b);

            var d = (x / a) * (x / a) + (y / b) * (y / b);
            if (d > 1) return false;
            else return true;
        }

        public override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();

            g.FillEllipse(mBrush, new Rectangle(0, 0, _w, _h));
            g.DrawEllipse(mPen, new Rectangle(0, 0, _w, _h));
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
            int r = 0;
            bool[] corners = new bool[] { true, true, true, true };
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;

            var ret = node.Attributes.GetNamedItem("width");
            if (ret != null)
            {
                w = ret.Value.castInt();
            }

            ret = node.Attributes.GetNamedItem("height");
            if (ret != null)
            {
                h = ret.Value.castInt();
            }

            ret = node.Attributes.GetNamedItem("radius");
            if (ret != null)
            {
                r = (int)ret.Value.castFloat() * 2;
                w = r;
                h = r;
            }

            ret = node.Attributes.GetNamedItem("strokeColor");
            if (ret != null)
            {
                sc = ret.Value.castHex(0xffffffff);
            }

            ret = node.Attributes.GetNamedItem("fillColor");
            if (ret != null)
            {
                fc = ret.Value.castHex(0xff888888);
            }
            ui = new UIRound(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
