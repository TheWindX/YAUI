using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;

namespace ns_YAUI
{
    class UIEdit : UILabel
    {
        UIRect mBackGround = new UIRect(128, 22, 0xffffffff, 0);
        int mMaxCharLength;
        public UIEdit(string t = "click to input", int maxCharLength = 16, uint color = 0xff444444):base(t, 18, EStyle.normal, color)
        {
            mMaxCharLength = maxCharLength;
            this.evtOnLMDown += onClick;
            mBackGround.px = -2;
            mBackGround.py = -2;
            mBackGround.paresent = this;
            mBackGround.depth = -1;
            mBackGround.width = (maxCharLength+1) * 18/2+2;
        }

        public override RectangleF drawRect
        {
            get
            {
                return mBackGround.drawRect;
            }
        }

        public bool onClick(UIWidget _this, int x, int y)
        {
            UIRoot.Instance.mHandleInputShow(true, x, y);
            Action<string> inputDone = null;
            inputDone = (text) =>
            {
                if (mMaxCharLength < text.Length)
                    this.text = text.Substring(0, mMaxCharLength);
                else
                    this.text = text;
                UIRoot.Instance.mEvtInputDone -= inputDone;
            };
            UIRoot.Instance.mEvtInputDone += inputDone;
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

        public override string typeName
        {
            get { return "edit"; }
        }
    }
}
