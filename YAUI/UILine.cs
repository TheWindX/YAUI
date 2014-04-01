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
    class UILine : UIWidget
    {
        int _length = 0;
        int _lineWidth = 0;
        uint fcolor;
        Brush mBrush;
        bool mHorizen;
        public UILine(int length, int lineWidth, uint fillcolor = 0xffffffff, bool horizon = true)
        {
            mHorizen = horizon;
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

        public override Rectangle drawRect
        {
            get
            {
                if(mHorizen)
                    return new Rectangle(0, -_lineWidth, _length, _lineWidth*2);
                else
                    return new Rectangle(-_lineWidth, 0, _lineWidth*2, _length);
            }
        }

        public override string typeName
        {
            get { return "line"; }
        }

        public override bool postTestPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();
            p.AddRectangle(drawRect);
            g.FillPath(mBrush, p);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int length = 16;
            uint color = 0xffffffff;
            int lineWidth = 2;
            bool horizon = true;
            var ret = node.Attributes.GetNamedItem("length");
            if (ret != null) length = ret.Value.castInt(16);

            ret = node.Attributes.GetNamedItem("line_width");
            if (ret != null) lineWidth = ret.Value.castInt(12);

            ret = node.Attributes.GetNamedItem("color");
            if (ret != null) color = ret.Value.castHex(0xffffffff);

            ret = node.Attributes.GetNamedItem("horizon");
            if (ret != null) horizon = ret.Value.castBool(true);

            ui = new UILine(length, lineWidth, color, horizon);
            ui.fromXML(node);
            ui.paresent = p;

            return node.ChildNodes;
        }
    }
}
