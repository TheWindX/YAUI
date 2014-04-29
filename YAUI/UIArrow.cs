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
        left,
        right,
        up,
        down,
    }

    class UIArrow : UIWidget
    {
        int _w = 0;
        int _h = 0;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;


        EForward mForward = EForward.up;

        public UIArrow(float w, float h, EForward forward = EForward.left, uint stroke = 0xffffffff, uint fill = 0xff888888)
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
                    case EForward.up:
                        return transform(new PointF(_w / 2, 0));
                    case EForward.down:
                        return transform(new PointF(_w / 2, _h));
                    case EForward.left:
                        return transform(new PointF(0, _h / 2));
                    case EForward.right:
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
                case EForward.up:
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
                case EForward.right:
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
                case EForward.left:
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
                case EForward.down:
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

        public override bool onDraw(Graphics g) 
        {
            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            switch (mForward)
            {
                case EForward.up:
                    p.AddLines(new PointF[] { new PointF(_w / 2, 0), new PointF(_w, _h), new PointF(0, _h) });
                    break;
                case EForward.right:
                    p.AddLines(new PointF[] { new PointF(0, 0), new PointF(0, _h), new PointF(_w, _h/2) });
                    break;
                case EForward.left:
                    p.AddLines(new PointF[] { new PointF(0, _h/2), new PointF(_w, 0), new PointF(_w, _h) });
                    break;
                case EForward.down:
                    p.AddLines(new PointF[] { new PointF(0, 0), new PointF(_w, 0), new PointF(_w/2, _h) });
                    break;
                default:
                    break;
            }

            p.CloseFigure();
            g.FillPath(mBrush, p);
            g.DrawPath(mPen, p);
            return true;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            float w = schemes.widgetWidth;
            float h = schemes.widgetHeight;
            float r = 8;
            bool[] corners = new bool[] { true, true, true, true };
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            bool br = true;

            h = w = getProp<float>(node, "length", schemes.widgetWidth, out br);
            if (!br)
            {
                w = getProp<float>(node, "width", schemes.widgetWidth, out br);
                h = getProp<float>(node, "height", schemes.widgetHeight, out br);
            }

            fc = (uint)getProp<EColorUtil>(node, "color", schemes.fillColor, out br);
            if (!br)
            {
                fc = getProp(node, "color", (uint)(schemes.fillColor), out br);
                if (!br)
                {

                    fc = (uint)getProp<EColorUtil>(node, "fillColor", schemes.fillColor, out br);
                    if (!br)
                    {
                        fc = getProp(node, "fillColor", (uint)(schemes.fillColor), out br);
                    }
                }
            }
            sc = (uint)getProp<EColorUtil>(node, "strokeColor", schemes.strokeColor, out br);
            if (!br)
            {
                sc = getProp(node, "strokeColor", (uint)(schemes.strokeColor), out br);
            }

            EForward fw = getProp<EForward>(node, "forward", EForward.up, out br);

            ui = new UIArrow(w, h, fw, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}


