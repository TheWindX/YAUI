using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace YAUIUser
{
    using ns_YAUI;
    using System.Xml;
    class newRect : ns_YAUI.UIWidget
    {
        float wpreserver;
        float hpreserver;
        UIRect mRect;
        public newRect(float w, float h, uint stroke = 0xffffffff, uint fill = 0xff888888)
        {
            mRect = new UIRect(w, h, stroke, fill);
            mRect.paresent = this;
            wpreserver = w;
            hpreserver = h;
            ns_YAUI.TimerManager.get().setInterval(t => { mRect.width = (int)(Math.Abs(Math.Sin(t / 1000.0)) * (double)wpreserver); setDirty(true); }, 10);
        }

        public override int width
        {
            get
            {
                return mRect.width;
            }
            set
            {
                mRect.width = value;
            }
        }

        public override int height
        {
            get
            {
                return mRect.height;
            }
            set
            {
                mRect.height = value;
            }
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            int w = 64;
            int h = 64;
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;
            string strRet = null;

            var ret = node.Attributes.GetNamedItem("width");
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

            ret = node.Attributes.GetNamedItem("length");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("length") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                h = strRet.castInt();
                w = h;
                UIRoot.Instance.setProperty("length", strRet);
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

            ret = node.Attributes.GetNamedItem("color");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setProperty("color", strRet);
            }

            ui = new newRect(w, h, sc, fc);
            ui.fromXML(node);
            ui.paresent = p;
            return node.ChildNodes;
        }
    }

    class testNewControl : Singleton<testNewControl>
    {
        public testNewControl()
        {
            UIRoot.Instance.initXMLWidget("newrect", newRect.fromXML)
                .root.appendFromXML(
                @"<newrect width='512' height='512' dragAble='true' ></newrect>");
            
        }
    }
}
