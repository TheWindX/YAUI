using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Xml;

namespace ns_YAUI
{
    class UIEdit : UIBlank
    {
        UIRect mBackGround = new UIRect(128, 22, 0xffffffff, 0);
        UILabel mLabel = null;
        public UIEdit(string bgText, int fontSz = 10, int w=128, int lines=1, uint color = 0xffffffff, EStyle style=EStyle.normal)
        {
            shrinkAble = true;
            mBackGround.width = w;
            mBackGround.height = fontSz*lines+UILabel.lineHeightGain*(lines-1);
            mLabel = new UILabel(bgText, fontSz, EStyle.normal, color, color, w);
            
            this.evtOnLMDown += onClick;
            mBackGround.paresent = this;
            mLabel.paresent = this;
            mBackGround.setDepthTail();
        }

        public override float width
        {
            get
            {
                return mBackGround.width;
            }
        }

        public override float height
        {
            get
            {
                return mBackGround.height;
            }
        }

        public bool onClick(UIWidget _this, int x, int y)
        {
            var m = mBackGround.getAbsMatrix();
            var rc = drawRect;
            rc = rc.transform(m);
            
            UIRoot.Instance.mHandleInputShow(true, (int)rc.Left, (int)rc.Top, (int)rc.Width, (int)rc.Height);
            Action<string> inputDone = null;
            inputDone = text =>
            {
                this.mLabel.text = text;
                UIRoot.Instance.mEvtInputDone -= inputDone;
                setDirty(true);
            };
            UIRoot.Instance.mEvtInputDone += inputDone;
            return true;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            string text = "";
            float width = 128;
            int rows = 1;
            int fontSz = 10;
            uint color = 0xffffffff;
            EStyle style = EStyle.normal;

            var ret = node.Attributes.GetNamedItem("text");
            if (ret != null) text = ret.Value;

            bool br = false;
            text = getProp<string>(node, "text", "template", out br);
            width = getProp(node, "width", 128, out br);
            rows = getProp(node, "rows", rows, out br);
            color = (uint)getProp<EColorUtil>(node, "color", (EColorUtil)schemes.textColor, out br);
            if (!br)
            {
                color = getProp(node, "color", (uint)(schemes.textColor), out br);
            }
            style = UILabel.getStyle(node);
            fontSz = getProp(node, "size", fontSz, out br);

            ui = new UIEdit(text, fontSz, (int)width, rows, color, style);
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
