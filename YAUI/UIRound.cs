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

        public override bool testSelfPick(Point pos)
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

            bool br = true;

            w = h = getAttr(node, "radius", 6, out br) * 2;
            if (!br)
            {
                h = w = getAttr<int>(node, "length", 64, out br);
                if (!br)
                {
                    w = getAttr<int>(node, "width", 64, out br);
                    h = getAttr<int>(node, "height", 64, out br);
                }
            }

            fc = (uint)getAttr<EColorUtil>(node, "color", EColorUtil.silver, out br);
            if (!br)
            {
                fc = getAttr(node, "color", (uint)(EColorUtil.silver), out br);
                if (!br)
                {

                    fc = (uint)getAttr<EColorUtil>(node, "fillColor", EColorUtil.silver, out br);
                    if (!br)
                    {
                        fc = getAttr(node, "fillColor", (uint)(EColorUtil.silver), out br);
                    }
                }
            }
            sc = (uint)getAttr<EColorUtil>(node, "strokeColor", EColorUtil.white, out br);
            if (!br)
            {
                sc = getAttr(node, "strokeColor", (uint)(EColorUtil.white), out br);
            }

            ui = new UIRound(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
