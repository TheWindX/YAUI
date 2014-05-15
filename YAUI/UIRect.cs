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

        public override string typeName
        {
            get { return "rect"; }
        }

        public override bool onDraw(Graphics g) 
        {
            g.FillRectangle(mBrush, 0, 0, _w, _h);
            g.DrawRectangle(mPen, 0, 0, _w, _h);
            return true;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            float w = schemes.widgetWidth;
            float h = schemes.widgetHeight;
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            bool br = true;

            fc = (uint)getProp<EColorUtil>(node, "color", (EColorUtil)schemes.fillColor, out br);
            if (!br)
            {
                fc = getProp(node, "color", (uint)(schemes.fillColor), out br);
                if (!br)
                {

                    fc = (uint)getProp<EColorUtil>(node, "fillColor", (EColorUtil)schemes.fillColor, out br);
                    if (!br)
                    {
                        fc = getProp(node, "fillColor", (uint)(schemes.fillColor), out br);
                    }
                }
            }
            sc = (uint)getProp<EColorUtil>(node, "strokeColor", (EColorUtil)schemes.strokeColor, out br);
            if (!br)
            {
                sc = getProp(node, "strokeColor", (uint)(schemes.strokeColor), out br);
            } 
            
            ui = new UIRect(w, h, sc, fc);
            ui.fromXML(node);
            if(p != null)
                ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
