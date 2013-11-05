using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class UITriangle : UIWidget
    {
        int _w = 0;
        int _h = 0;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;

        public enum EDir
        {
            e_left,
            e_right,
            e_up,
            e_down,
        }
        EDir mForward = EDir.e_up;

        public UITriangle(float w, float h, EDir forward = EDir.e_left, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            _w = (int)w;
            _h = (int)h;
            scolor = stroke;
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            mPen = new Pen(Color.FromArgb((Int32)scolor));
            mForward = forward;
        }

        void setFillColor(uint fill = 0xff888888)
        {
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
        }

        void setStrokeColor(uint stroke = 0xff888888)
        {
            scolor = stroke;
            mPen = new Pen(Color.FromArgb((Int32)scolor));
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
            switch (mForward)
            {
                case EDir.e_up:
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
                    break;
                case EDir.e_right:
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
                    break;
                case EDir.e_left:
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
                    break;
                case EDir.e_down:
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
                    break;
                default:
                    break;
            }
            return true;
        }

        internal override void onDraw(Graphics g) 
        {
            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            switch (mForward)
            {
                case EDir.e_up:
                    p.AddLines(new Point[] { new Point(_w / 2, 0), new Point(_w, _h), new Point(0, _h) });
                    break;
                case EDir.e_right:
                    p.AddLines(new Point[] { new Point(0, 0), new Point(0, _h), new Point(_w, _h/2) });
                    break;
                case EDir.e_left:
                    p.AddLines(new Point[] { new Point(0, _h/2), new Point(_w, 0), new Point(_w, _h) });
                    break;
                case EDir.e_down:
                    p.AddLines(new Point[] { new Point(0, 0), new Point(_w, 0), new Point(_w/2, _h) });
                    break;
                default:
                    break;
            }

            p.CloseFigure();
            g.FillPath(mBrush, p);
            g.DrawPath(mPen, p);
        }
    }
}


