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

namespace ns_behaviour
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

        public Point pivotPos
        {
            get
            {
                switch (mForward)
                {
                    case EForward.e_up:
                        return transform(new Point(_w / 2, 0));
                    case EForward.e_down:
                        return transform(new Point(_w / 2, _h));
                    case EForward.e_left:
                        return transform(new Point(0, _h / 2));
                    case EForward.e_right:
                        return transform(new Point(_w, _h / 2));
                    default:
                        break;
                }
                return new Point();
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

        public Point center
        {
            get
            {
                return new Point(mPos.X + _w / 2, mPos.Y + _h / 2);
            }
            set
            {
                mPos.X = value.X - _w / 2;
                mPos.Y = value.Y - _h / 2;
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
                    break;
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
                    break;
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
                    break;
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
                case EForward.e_up:
                    p.AddLines(new Point[] { new Point(_w / 2, 0), new Point(_w, _h), new Point(0, _h) });
                    break;
                case EForward.e_right:
                    p.AddLines(new Point[] { new Point(0, 0), new Point(0, _h), new Point(_w, _h/2) });
                    break;
                case EForward.e_left:
                    p.AddLines(new Point[] { new Point(0, _h/2), new Point(_w, 0), new Point(_w, _h) });
                    break;
                case EForward.e_down:
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


