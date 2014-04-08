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
    public class UIRoundRect : UIWidget
    {
        int _w = 64;
        int _h = 64;
        int _r = 16;
        uint scolor;
        uint fcolor;
        Brush mBrush;
        Pen mPen;
        bool[] mCorner = new bool[] { true, true, true, true };


        internal UIRoundRect(float w, float h, float radius, bool[] corners, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            _w = (int)w;
            _h = (int)h;
            _r = (int)radius;
            scolor = stroke;
            fcolor = fill;
            mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
            mPen = new Pen(Color.FromArgb((Int32)scolor));
            if (corners != null) mCorner = corners;
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
                mBrush = new SolidBrush(Color.FromArgb((Int32)fcolor));
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

        public bool leftTopCorner
        {
            get
            {
                return mCorner[0];
            }
            set
            {
                mCorner[0] = value;
            }
        }

        public bool rightTopCorner
        {
            get
            {
                return mCorner[1];
            }
            set
            {
                mCorner[1] = value;
            }
        }

        public bool rightBottomCorner
        {
            get
            {
                return mCorner[2];
            }
            set
            {
                mCorner[2] = value;
            }
        }

        public bool leftBottomCorner
        {
            get
            {
                return mCorner[3];
            }
            set
            {
                mCorner[3] = value;
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

        protected override Rectangle dirtyRect
        {
            get{ return new Rectangle(_r/2, _r/2, _w-_r, _h-_r*2); }// rect
        }

        public override string typeName
        {
            get { return "round_rect"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        public override void onDraw(Graphics g)
        {
            GraphicsPath p = new GraphicsPath();

            if (mCorner[0])
            {
                p.AddArc(0, 0, _r, _r, 180, 90);
            }
            else
            {
                p.AddLine(0, _r, 0, 0);
                p.AddLine(0, 0, _r, 0);
            }
            p.AddLine(_r, 0, _w - _r, 0);

            if (mCorner[1])
            {
                p.AddArc(_w - _r, 0, _r, _r, 270, 90);
            }
            else
            {
                p.AddLine(_w - _r, 0, _w, 0);
                p.AddLine(_w, 0, _w, _r);
            }
            p.AddLine(_w, _r, _w, _h - _r);

            if (mCorner[2])
            {
                p.AddArc(_w - _r, _h - _r, _r, _r, 0, 90);
            }
            else
            {
                p.AddLine(_w, _h - _r, _w, _h);
                p.AddLine(_w, _h, _w - _r, _h);
            }
            p.AddLine(_w - _r, _h, _r, _h);

            if (mCorner[3])
            {
                p.AddArc(0, _h - _r, _r, _r, 90, 90);
            }
            else
            {
                p.AddLine(_r, _h, 0, _h);
                p.AddLine(0, _h, 0, _h-_r);
            }
            p.AddLine(0, _h - _r, 0, _r/2);
            g.FillPath(mBrush, p);
            g.DrawPath(mPen, p);
        }



        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
            int r = 8;
            bool[] corners = new bool[] { true, true, true, true };
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;

            var ret = node.Attributes.GetNamedItem("length");
            string strRet = (ret == null) ? UIRoot.Instance.getProperty("length") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                h = strRet.castInt();
                w = h;
                UIRoot.Instance.setProperty("length", strRet);
            }

            ret = node.Attributes.GetNamedItem("width");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("width") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                w = strRet.castInt();
                UIRoot.Instance.setProperty("width", strRet);
            }

            ret = node.Attributes.GetNamedItem("height");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("height") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                h = strRet.castInt();
                UIRoot.Instance.setProperty("height", strRet);
            }

            ret = node.Attributes.GetNamedItem("color");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setProperty("color", strRet);
            }

            ret = node.Attributes.GetNamedItem("strokeColor");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("strokeColor") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                sc = strRet.castHex(0xffffffff);
                UIRoot.Instance.setProperty("strokeColor", strRet);
            }

            ret = node.Attributes.GetNamedItem("fillColor");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("fillColor") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setProperty("fillColor", strRet);
            }
           
            ret = node.Attributes.GetNamedItem("radius");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("radius") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                r = (int)strRet.castFloat() * 2;
                UIRoot.Instance.setProperty("radius", strRet);
            }

            ret = node.Attributes.GetNamedItem("leftTopCorner");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("leftTopCorner") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                corners[0] = strRet.castBool();
                UIRoot.Instance.setProperty("leftTopCorner", strRet);
            }

            ret = node.Attributes.GetNamedItem("rightTopCorner");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("rightTopCorner") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                corners[1] = strRet.castBool();
                UIRoot.Instance.setProperty("rightTopCorner", strRet);
            }

            ret = node.Attributes.GetNamedItem("rightBottomCorner");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("rightBottomCorner") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                corners[2] = strRet.castBool();
                UIRoot.Instance.setProperty("rightBottomCorner", strRet);
            }

            ret = node.Attributes.GetNamedItem("leftBottomCorner");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("leftBottomCorner") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                corners[3] = strRet.castBool();
                UIRoot.Instance.setProperty("leftBottomCorner", strRet);
            }

            ui = new UIRoundRect(w, h, r, corners, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }
}
