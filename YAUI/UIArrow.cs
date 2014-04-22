/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: primary TRIANGLE
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
    public enum EForward 
    {
        e_left,
        e_right,
        e_up,
        e_down,
    }

    class UIArrow : UIWidget
    {
        int _w = 0;
        int _h = 0;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;


        EForward mForward = EForward.e_up;

        public UIArrow(float w, float h, EForward forward = EForward.e_left, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            _w = (int)w;
            _h = (int)h;
            scolor = stroke;
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            mPen = new Pen(Color.FromArgb((Int32)scolor));
            mForward = forward;
        }

        public void setFillColor(uint fill = 0xff888888)
        {
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
        }

        public void setStrokeColor(uint stroke = 0xff888888)
        {
            scolor = stroke;
            mPen = new Pen(Color.FromArgb((Int32)scolor));
        }

        public PointF pivotPos
        {
            get
            {
                switch (mForward)
                {
                    case EForward.e_up:
                        return transform(new PointF(_w / 2, 0));
                    case EForward.e_down:
                        return transform(new PointF(_w / 2, _h));
                    case EForward.e_left:
                        return transform(new PointF(0, _h / 2));
                    case EForward.e_right:
                        return transform(new PointF(_w, _h / 2));
                    default:
                        break;
                }
                return new PointF();
            }
        }

        public EForward forward
        {
            get
            {
                return mForward;
            }
            set
            {
                mForward = value;
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

        public PointF center
        {
            get
            {
                return new PointF(px + _w / 2, py + _h / 2);
            }
            set
            {
                px = value.X - _w / 2;
                py = value.Y - _h / 2;
            }
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
            get { return "Arrow"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            switch (mForward)
            {
                case EForward.e_up:
                    {
                        float x;
                        if (pos.X < _w / 2)
                        {
                            x = _w / 2 - pos.X;
                        }
                        else
                        {
                            x = pos.X - _w / 2;
                        }
                        if ((float)pos.Y / x < (2*_h/_w))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case EForward.e_right:
                    {
                        float x = _w - pos.X;
                        float y;
                        if (pos.Y < (float)_h / 2)
                        {
                            y = (float)_h / 2 - pos.Y;
                        }
                        else
                        {
                            y = pos.Y - (float)_h / 2;
                        }
                        if (x/y < 2.0f*_w/_h)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case EForward.e_left:
                    {
                        float x = pos.X;
                        float y;
                        if (pos.Y < (float)_h / 2)
                        {
                            y = (float)_h / 2 - pos.Y;
                        }
                        else
                        {
                            y = pos.Y - (float)_h / 2;
                        }
                        if (x / y < 2.0f * _w / _h)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case EForward.e_down:
                    {
                        float x;
                        float y = _h - pos.Y;
                        if (pos.X < _w / 2)
                        {
                            x = _w / 2 - pos.X;
                        }
                        else
                        {
                            x = pos.X - _w / 2;
                        }
                        if (y / x < (2 * _h / _w))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                default:
                    break;
            }
            return true;
        }

        public override void onDraw(Graphics g) 
        {
            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            switch (mForward)
            {
                case EForward.e_up:
                    p.AddLines(new PointF[] { new PointF(_w / 2, 0), new PointF(_w, _h), new PointF(0, _h) });
                    break;
                case EForward.e_right:
                    p.AddLines(new PointF[] { new PointF(0, 0), new PointF(0, _h), new PointF(_w, _h/2) });
                    break;
                case EForward.e_left:
                    p.AddLines(new PointF[] { new PointF(0, _h/2), new PointF(_w, 0), new PointF(_w, _h) });
                    break;
                case EForward.e_down:
                    p.AddLines(new PointF[] { new PointF(0, 0), new PointF(_w, 0), new PointF(_w/2, _h) });
                    break;
                default:
                    break;
            }

            p.CloseFigure();
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

            EForward fw = getAttr<EForward>(node, "forward", EForward.e_up, out br);

            ui = new UIArrow(w, h, fw, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}


