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
    public class UIRect : UIWidget
    {
        float _w = 0;
        float _h = 0;
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


        protected override RectangleF dirtyRect
        {
            get { return drawRect; }// rect
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

        public override float width
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

        public override float height
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

        public override RectangleF drawRect
        {
            get
            {
                return new RectangleF(0, 0, _w, _h);
            }
        }

        public override string typeName
        {
            get { return "rect"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            return true;
        }

        public override void onDraw(Graphics g) 
        {
            g.FillRectangle(mBrush, 0, 0, _w, _h);
            g.DrawRectangle(mPen, 0, 0, _w, _h);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;
            bool br = true;
            
            h = w = getAttr<int>(node, "length", 64, out br);
            if (!br)
            {
                w = getAttr<int>(node, "width", 64, out br);
                h = getAttr<int>(node, "height", 64, out br);
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
            
            ui = new UIRect(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
