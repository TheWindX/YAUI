using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ns_YAUIUser
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
            ns_YAUI.TimerManager.get().setInterval((i, t) => { mRect.width = (int)(Math.Abs(Math.Sin(t / 1000.0)) * (double)wpreserver); setDirty(true); }, 10);
        }

        public override float width
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

        public override float height
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
            float w = schemes.widgetWidth;
            float h = schemes.widgetHeight;
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            string strRet = null;

            var ret = node.Attributes.GetNamedItem("width");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("width") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                w = strRet.castInt();
                UIRoot.Instance.setPropertyInTable("width", ref strRet);
            }

            ret = node.Attributes.GetNamedItem("height");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("height") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                h = strRet.castInt();
                UIRoot.Instance.setPropertyInTable("height", ref strRet);
            }

            ret = node.Attributes.GetNamedItem("length");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("length") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                h = strRet.castInt();
                w = h;
                UIRoot.Instance.setPropertyInTable("length", ref strRet);
            }

            ret = node.Attributes.GetNamedItem("strokeColor");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("strokeColor") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                sc = strRet.castHex(0xffffffff);
                UIRoot.Instance.setPropertyInTable("strokeColor", ref strRet);
            }

            ret = node.Attributes.GetNamedItem("fillColor");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("fillColor") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setPropertyInTable("fillColor", ref strRet);
            }

            ret = node.Attributes.GetNamedItem("color");
            strRet = (ret == null) ? UIRoot.Instance.getPropertyInTable("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                fc = strRet.castHex(0xff888888);
                UIRoot.Instance.setPropertyInTable("color", ref strRet);
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
