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
using System.Windows.Forms;

namespace ns_behaviour
{
    class UILable : UIWidget
    {
        string mText = "template";
        int mSz = 12;
        Font mFont;//todo, in font manager;
        uint mColor = 0xffffffff;
        Brush mBrush;
        Rectangle mRect = new Rectangle();
        public UILable(string t = "Template", int sz = 12, uint color = 0xffffffff)
        {
            mSz = sz;
            mFont = new Font("Arial", sz, FontStyle.Bold);
            textColor = color;
            text = t;
            var fontsz = TextRenderer.MeasureText(t, mFont);
            mRect = new Rectangle(new Point(0, 0), fontsz);
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
                var fontsz = TextRenderer.MeasureText(mText, mFont);
                mRect = new Rectangle(new Point(0, 0), fontsz);
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
            //var td = g.MeasureString(mText, mFont);
            //mRect.Width = (int)td.Width;
            //mRect.Height = (int)td.Height;
            g.DrawString(mText, mFont, mBrush, new PointF());
        }
    }
}
