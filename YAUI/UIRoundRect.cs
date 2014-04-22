﻿/*
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
    public class UIRoundRect : UIWidget
    {
        float _w = 64;
        float _h = 64;
        float _r = 16;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;
        bool[] mCorner = new bool[] { true, true, true, true };


        internal UIRoundRect(float w, float h, float radius, bool[] corners, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            _w = (int)w;
            _h = (int)h;
            _r = (int)radius;
            scolor = stroke;
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            mPen = new Pen(Color.FromArgb((Int32)scolor));
            if (corners != null) mCorner = corners;
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

        public bool leftTopCorner
        {
            get
            {
                return mCorner[0];
            }
            set
            {
                mCorner[0] = value;
            }
        }

        public bool rightTopCorner
        {
            get
            {
                return mCorner[1];
            }
            set
            {
                mCorner[1] = value;
            }
        }

        public bool rightBottomCorner
        {
            get
            {
                return mCorner[2];
            }
            set
            {
                mCorner[2] = value;
            }
        }

        public bool leftBottomCorner
        {
            get
            {
                return mCorner[3];
            }
            set
            {
                mCorner[3] = value;
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

        protected override RectangleF dirtyRect
        {
            get{ return new RectangleF(_r/2, _r/2, _w-_r, _h-_r*2); }// rect
        }

        public override string typeName
        {
            get { return "round_rect"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            return true;
        }

        public override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();

            if (mCorner[0])
            {
                p.AddArc(0, 0, _r, _r, 180, 90);
            }
            else
            {
                p.AddLine(0, _r, 0, 0);
                p.AddLine(0, 0, _r, 0);
            }
            p.AddLine(_r, 0, _w - _r, 0);

            if (mCorner[1])
            {
                p.AddArc(_w - _r, 0, _r, _r, 270, 90);
            }
            else
            {
                p.AddLine(_w - _r, 0, _w, 0);
                p.AddLine(_w, 0, _w, _r);
            }
            p.AddLine(_w, _r, _w, _h - _r);

            if (mCorner[2])
            {
                p.AddArc(_w - _r, _h - _r, _r, _r, 0, 90);
            }
            else
            {
                p.AddLine(_w, _h - _r, _w, _h);
                p.AddLine(_w, _h, _w - _r, _h);
            }
            p.AddLine(_w - _r, _h, _r, _h);

            if (mCorner[3])
            {
                p.AddArc(0, _h - _r, _r, _r, 90, 90);
            }
            else
            {
                p.AddLine(_r, _h, 0, _h);
                p.AddLine(0, _h, 0, _h-_r);
            }
            p.AddLine(0, _h - _r, 0, _r/2);
            g.FillPath(mBrush, p);
            g.DrawPath(mPen, p);
        }



        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = schemes.width;
            int h = schemes.height;
            int r = 8;
            bool[] corners = new bool[] { true, true, true, true };
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            bool br = true;

            h = w = getAttr<int>(node, "length", schemes.width, out br);
            if (!br)
            {
                w = getAttr<int>(node, "width", schemes.width, out br);
                h = getAttr<int>(node, "height", schemes.height, out br);
            }

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
            
            r = getAttr(node, "radius", 6, out br)*2;

            corners[0] = getAttr(node, "leftTopCorner", true, out br);
            corners[1] = getAttr(node, "rightTopCorner", true, out br);
            corners[2] = getAttr(node, "rightBottomCorner", true, out br);
            corners[3] = getAttr(node, "leftBottomCorner", true, out br);

            ui = new UIRoundRect(w, h, r, corners, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
