using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;

namespace ns_behaviour
{
    class UIEdit : UILable
    {
        UIRect mBackGround = new UIRect(128, 22, 0xffffffff, 0);
        int mMaxCharLength;
        public UIEdit(string t = "click to input", int maxCharLength = 16, uint color = 0xffffffff):base(t, 18, color)
        {
            mMaxCharLength = maxCharLength;
            this.evtOnLMDown += onClick;
            mBackGround.position = new Point(-2, -2);
            mBackGround.paresent = this;
            mBackGround.depth = -1;
            mBackGround.width = (maxCharLength+1) * 18/2+2;
        }

        public override Rectangle drawRect
        {
            get
            {
                return mBackGround.drawRect;
            }
        }

        public bool onClick(UIWidget _this, int x, int y)
        {
            InputForm.Instance.show(true, x, y);
            InputForm.Instance.evtInputExit = (text) =>
                {
                    if (mMaxCharLength < text.Length)
                        this.text = text.Substring(0, mMaxCharLength);
                    else
                        this.text = text;
                };
            return true;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            string text = "";
            int length = 16;
            uint color = 0xffffffff;
            var ret = node.Attributes.GetNamedItem("text");
            if (ret != null) text = ret.Value;

            ret = node.Attributes.GetNamedItem("length");
            if (ret != null) length = ret.Value.castInt(12);

            ret = node.Attributes.GetNamedItem("color");
            if (ret != null) color = ret.Value.castHex(0xffffffff);

            ui = new UIEdit(text, length, color);
            ui.fromXML(node);
            ui.paresent = p;
            
            return node.ChildNodes;
        }
    }
}
