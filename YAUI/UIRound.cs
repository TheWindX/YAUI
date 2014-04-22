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
        float _w;
        float _h;
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
                return new RectangleF(-_w / 2, -_h / 2, _w , _h);
            }
        }

        public override string typeName
        {
            get { return "round"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            pos.X = pos.X + _w / 2;
            pos.Y = pos.Y + _h / 2;
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
            var gs = g.Save();
            var m = g.Transform;
            m.Translate(-_w / 2.0f, -_h / 2.0f);
            g.Transform = m;
            g.FillEllipse(mBrush, new RectangleF(0, 0, _w, _h));
            g.DrawEllipse(mPen, new RectangleF(0, 0, _w, _h));
            g.Restore(gs);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = schemes.width;
            int h = schemes.height;
            int r = 0;
            bool[] corners = new bool[] { true, true, true, true };
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;

            bool br = true;

            w = h = getAttr(node, "radius", schemes.width/2, out br) * 2;
            
            fc = (uint)getAttr<EColorUtil>(node, "color", schemes.fillColor, out br);
            if (!br)
            {
                fc = getAttr(node, "color", (uint)(schemes.fillColor), out br);
                if (!br)
                {

                    fc = (uint)getAttr<EColorUtil>(node, "fillColor", schemes.fillColor, out br);
                    if (!br)
                    {
                        fc = getAttr(node, "fillColor", (uint)(schemes.fillColor), out br);
                    }
                }
            }
            sc = (uint)getAttr<EColorUtil>(node, "strokeColor", schemes.strokeColor, out br);
            if (!br)
            {
                sc = getAttr(node, "strokeColor", (uint)(schemes.strokeColor), out br);
            }

            ui = new UIRound(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
