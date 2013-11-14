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
    class UILable : UIWidget
    {
        string mText = "template";
        int mSz = 12;
        uint mColor = 0xffffffff;
        Brush mBrush;
        Rectangle mRect = new Rectangle();
        public UILable(string t = "Template", int sz = 12, uint color = 0xffffffff)
        {
            mSz = sz;
            textColor = color;
            text = t;
        }

        public uint textColor
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
                mBrush = new SolidBrush(Color.FromArgb((Int32)mColor));
            }
        }

        public string text
        {
            get
            {
                return mText;
            }

            set
            {
                mText = value;
                mRect.X = 0;
                mRect.Y = 0;
                mRect.Width = (mSz + 2) * mText.Count();
                mRect.Height = mSz + 2;
            }
        }

        public override Rectangle rect
        {
            get
            {
                return mRect;
            }
        }

        public override string typeName
        {
            get { return "string"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            var td = g.MeasureString(mText, UIWidget.mDrawFont);
            mRect.Width = (int)td.Width;
            mRect.Height = (int)td.Height;
            g.DrawString(mText, UIWidget.mDrawFont, mBrush, new PointF());
        }
    }
}
