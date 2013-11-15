using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class UILine : UIWidget
    {
        int _length = 0;
        int _lineWidth = 0;
        uint fcolor;
        Brush mBrush;
        bool mHorizen;
        public UILine(int length, int lineWidth, uint fillcolor = 0xffffffff, bool horizen = true)
        {
            mHorizen = horizen;
            _length = length;
            _lineWidth = lineWidth;
            fcolor = fillcolor;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
        }

        public void setFillColor(uint fill = 0xff888888)
        {
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
        }

        public int length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        public int lineWidth
        {
            get
            {
                return _lineWidth;
            }
            set
            {
                _length = value;
            }
        }

        public override Rectangle rect
        {
            get
            {
                if(mHorizen)
                    return new Rectangle(0, 0, _length, _lineWidth);
                else
                    return new Rectangle(0, 0, _lineWidth, _length);
            }
        }

        public override string typeName
        {
            get { return "line"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();
            p.AddRectangle(rect);
            g.FillPath(mBrush, p);
        }
    }
}
