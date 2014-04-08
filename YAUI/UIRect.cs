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
using System.Xml;

namespace ns_YAUI
{
    public class UIRect : UIWidget
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


        protected override Rectangle dirtyRect
        {
            get { return drawRect; }// rect
        }

        public uint fillColor
        {
            get
            {
                return fcolor;
            }
            set
            {
                fcolor = value;
                mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor) );
            }
        }

        public uint strokeColor
        {
            get
            {
                return scolor;
            }
            set
            {
                scolor = value;
                mPen = new Pen(Color.FromArgb((Int32)scolor));
            }
        }

        public override int width
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

        public override int height
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
            }
        }

        public void setSize(int w, int h)
        {
            _w = w;
            _h = h;
        }

        public override Rectangle drawRect
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

        public override void onDraw(Graphics g) 
        {
            g.FillRectangle(mBrush, 0, 0, _w, _h);
            g.DrawRectangle(mPen, 0, 0, _w, _h);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;
            string strRet = null;

            var ret = node.Attributes.GetNamedItem("length");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("length") : ((ret.Value == "NA") ? null : ret.Value);
            UIRoot.Instance.setProperty("length", ref strRet);
            if (strRet != null)
            {
                h = strRet.castInt();
                w = h;
            }

            ret = node.Attributes.GetNamedItem("width");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("width") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("width", ref strRet);
            if (strRet != null)
            {
                w = strRet.castInt();
            }

            ret = node.Attributes.GetNamedItem("height");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("height") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("height", ref strRet);
            if (strRet != null)
            {
                h = strRet.castInt();
            }
            
            ret = node.Attributes.GetNamedItem("color");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("color") : ((ret.Value == "NA") ? null : ret.Value);
            UIRoot.Instance.setProperty("color", ref strRet);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
            }

            ret = node.Attributes.GetNamedItem("strokeColor");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("strokeColor") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("strokeColor", ref strRet);
            if (strRet != null)
            {
                sc = strRet.castHex(0xffffffff);
            }

            ret = node.Attributes.GetNamedItem("fillColor");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("fillColor") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("fillColor", ref strRet);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
            }

            ui = new UIRect(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
