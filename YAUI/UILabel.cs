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
using System.Xml;

namespace ns_YAUI
{
    public enum EStyle : int
    {
        normal = 0,
        bold = 1,
        italic = 2,
        underline = 4,
        strikeout = 8,
    }

    public class UILabel : UIWidget
    {
        string mText = "template";
        List<string> mLines = new List<string>();
        int mSz = 12;
        Font mFont;//todo, in font manager;
        uint mColor = 0xffffffff;
        Brush mBrush;

        float mWidth = 0;
        float mHeight = 0;
        float mLineheight = 0;

        float mLineWidthLimit = -1;

        public bool link
        {
            set
            {
                if (value)
                {
                    evtOnEnter += () =>
                    {
                        textStyle = textStyle | EStyle.underline;
                        setDirty(true);
                    };
                    evtOnExit += () =>
                    {
                        textStyle = textStyle & ~EStyle.underline;
                        setDirty(true);
                    };
                }
                else
                {
                    evtOnEnterClear();
                    evtOnExitClear();
                    textStyle = textStyle & ~EStyle.underline;
                    setDirty(true);
                }
            }
        }

        EStyle mStyle = EStyle.normal;

        public UILabel(string t = "Template", int sz = 12, EStyle st = EStyle.normal, uint color = 0xffffffff, float maxLength = -1)
        {
            mStyle = st;
            mSz = sz;
            textColor = color;
            text = t;
            mLineWidthLimit = maxLength;
            updateFont();
        }

        internal Size measureText(string text, int lines=1)
        {
            var sz = TextRenderer.MeasureText(text, mFont);
            sz.Width = ((lines - 1) * lineHeightGain) + sz.Height * lines;
            return sz;
        }

        void splitByLength(string str, float length)
        {
            if (length <= 0) length = float.MaxValue;
            string strRest = str;
            while (strRest != "")
            {
                if (mHeight != 0) mHeight += lineHeightGain;
                
                var splitIdx = ns_YAUtils.Algorithms.binarySearch(strRest.Length-1, idx => 
                    {
                        var strTry = strRest.Substring(0, idx+1);
                        var sz1 = TextRenderer.MeasureText(strTry, mFont);
                        return sz1.Width <= length;
                    });
                string line = strRest.Substring(0, splitIdx + 1);
                mLines.Add(line);

                var sz = TextRenderer.MeasureText(line, mFont);
                mWidth = max(mWidth, sz.Width);
                mHeight = mHeight + sz.Height;
                mLineheight = sz.Height;

                if (splitIdx == strRest.Length - 1)
                {
                    break;
                }
                strRest = strRest.Substring(splitIdx + 1);
            }
        }

        internal const int lineHeightGain = 2;
        public void updateFont()
        {
            mWidth = 0;
            mHeight = 0;
            mLines.Clear();
            mFont = new Font("msyh", mSz, (FontStyle)mStyle);
            splitByLength(mText, mLineWidthLimit);//calc mWidth & mHeight also
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

        public int textSize
        {
            get
            {
                return mSz;
            }

            set
            {
                mSz = value;
                updateFont();
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
                updateFont();
            }
        }

        public EStyle textStyle
        {
            get
            {
                return mStyle;
            }
            set
            {
                mStyle = value;
                updateFont();
            }
        }

        public override float width
        {
            get
            {
                return mWidth;
            }
        }

        public override float height
        {
            get
            {
                return mHeight;
            }
        }

        public override string typeName
        {
            get { return "label"; }
        }

        public override bool testSelfPick(PointF pos)
        {
            return true;
        }

        public override bool onDraw(Graphics g)
        {
            float h = 0;
            for (int i = 0; i < mLines.Count; ++i)
            {
                var line = mLines[i];
                g.DrawString(line, mFont, mBrush, new PointF(0, h));
                h += mLineheight + lineHeightGain;
            }

            return true;
        }

        internal static EStyle getStyle(XmlNode node)
        {
            string strRet = UIWidget.tryGetProp("style", node);
            if (strRet == null) return EStyle.normal;

            var strs = strRet.Split(',');
            var st = strs.Aggregate(EStyle.normal, (style, prop)=>
                {
                    if(prop.Contains("normal") )
                    {
                        style |= EStyle.normal;
                    }
                    else if(prop.Contains("bold") )
                    {
                        style |= EStyle.bold;
                    }
                    else if(prop.Contains("underline") )
                    {
                        style |= EStyle.underline;
                    }
                    else if(prop.Contains("strikeout") )
                    {
                        style |= EStyle.strikeout;
                    }
                    return style;
                });

            return st;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            string text = "template";
            int size = 12;
            float maxLineWidth = -1;
            uint color = schemes.textColor;
            EStyle style = EStyle.normal;
            bool br = false;

            text = getProp(node, "text", "template", out br);
            size = getProp(node, "size", 12, out br);
            color = (uint)getProp<EColorUtil>(node, "color", (EColorUtil)schemes.textColor, out br);
            if (!br)
            {
                color = getProp(node, "color", (uint)(schemes.textColor), out br);
            }
            style = getStyle(node);

            maxLineWidth = getProp(node, "maxLineWidth", maxLineWidth, out br);
            bool link = getProp(node, "link", false, out br);

            ui = new UILabel(text, size, style, color, maxLineWidth);
            (ui as UILabel).link = link;
            ui.fromXML(node);
            ui.paresent = p;

            return node.ChildNodes;
        }
    }
}
