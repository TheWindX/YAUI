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
    }
}
