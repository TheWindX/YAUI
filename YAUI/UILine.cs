using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace ns_YAUI
{
    public class UILine : UIWidget
    {
        PointF mBegin = new PointF();
        PointF mEnd = new PointF();

        uint mColor;
        Pen mPen;

        //start
        public void setBegin(float x, float y)
        {
            mBegin = new PointF(x, y);
        }

        public void setLineWidth(float w)
        {
            mPen.Width = w;
        }

        //end
        public void setEnd(float x, float y)
        {
            mEnd = new PointF(x, y);
        }

        //override
        public override float width
        {
            get
            {
                return Math.Abs(mEnd.X - mBegin.X);
            }
        }

        public override float height
        {
            get
            {
                return Math.Abs(mEnd.Y - mBegin.Y);
            }
        }

        public override RectangleF drawRect
        {
            get
            {
                float left, right, top, bottom = 0;
                left = min(mBegin.X, mEnd.X);
                top = min(mBegin.Y, mEnd.Y);
                right = max(mBegin.X, mEnd.X);
                bottom = max(mBegin.Y, mEnd.Y);
                return new RectangleF(left, top, right - left, bottom - top);
            }
        }

        public uint color
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
                mPen = new Pen(Color.FromArgb((Int32)mColor));
            }
        }

        public override string typeName
        {
            get { return "line"; }
        }

        public override bool onDraw(Graphics g)
        {
            g.DrawLine(mPen, mBegin, mEnd);
            return true;
        }

        

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            uint color = (uint)schemes.strokeColor;
            bool br = true;

            float lineWidth = (uint)getProp<float>(node, "lineWidth", 1, out br);

            color = (uint)getProp<EColorUtil>(node, "color", (EColorUtil)schemes.strokeColor, out br);
            string strBegin = tryGetProp("begin", node);
            PointF ptBegin = parsePt(strBegin);

            string strEnd = tryGetProp("end", node);
            PointF ptEnd = parsePt(strEnd);

            var uiLine = new UILine();
            uiLine.setLineWidth(lineWidth);//must be under color
            uiLine.setBegin(ptBegin.X, ptBegin.Y);
            uiLine.setEnd(ptEnd.X, ptEnd.Y);
            uiLine.color = color;

            uiLine.fromXML(node);

            ui = uiLine;
            return node.ChildNodes;
        }

        private static PointF parsePt(string strPt)
        {
            PointF ptRet = new PointF();
            if (strPt == null) return ptRet;
            var strs = strPt.Split(',').ToList();
            var strs1 = new List<string>();
            strs.ForEach(str => strs1.Add(str.Trim()));
            int c = strs1.Count();
            if (c > 0)
            {
                ptRet.Y = ptRet.X = strs1[0].castFloat();
            }
            if (c > 1)
            {
                ptRet.Y = strs1[1].castFloat();
            }
            return ptRet;
        }
    }
}
