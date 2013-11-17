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
    class UIRect : UIWidget
    {
        int _w = 0;
        int _h = 0;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;
        public UIRect(float w, float h, uint stroke = 0xffffffff, uint fill = 0xff888888)
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
                mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor) );
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
                _w = value;
            }
        }

        public override Rectangle rect
        {
            get
            {
                return new Rectangle(0, 0, _w, _h);
            }
        }

        public override string typeName
        {
            get { return "rect"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g) 
        {
            GraphicsPath p = new GraphicsPath();
            p.AddRectangle(new Rectangle(0, 0, _w, _h));
            g.FillPath(mBrush, p);
            g.DrawPath(mPen, p);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
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

            ui = new UIRect(w, h, sc, fc);
            ui.paresent = p;
            ui.fromXML(node);

            return node.ChildNodes;
        }
    }
}
