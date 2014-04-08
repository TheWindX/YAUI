﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;

namespace ns_YAUI
{
    public class UIResizer : UIBlank
    {
        internal UIRect mClient = null;
        internal UIRect mResizer = null;
        public UIResizer()
        {
            width = 512;
            height = 512;
            appendFromXML(@"
<rect noInherent='true' length='512' name='client' expand='true'>
</rect>
");
            appendFromXML(@"
<rect noInherent='true' name='sizer' length='32' align='rightBottom'>
</rect>
");
            mClient = childOf("client") as UIRect;
            mResizer = childOf("sizer") as UIRect;
            int spx = 0;
            int spy = 0;
            int sw = 0;
            int sh = 0;
            Action<int, int> moveHandle = (nx, ny) =>
            {
                mResizer.updateFixpoint(nx, ny);
                var dspx = mResizer.px - spx;
                var dspy = mResizer.py - spy;
                width = sw + dspx;
                height = sh + dspy;
                this.setDirty(true);
            };

            Action<int, int> delHandle = null;
            delHandle += (nx, ny) =>
                {
                    UIRoot.Instance.evtMove -= moveHandle;
                    UIRoot.Instance.evtLeftUp -= delHandle;
                };
            mResizer.evtOnLMDown += (ui, x, y) =>
                {
                    mResizer.beginFixpoint(x, y);
                    spx = mResizer.px;
                    spy = mResizer.py;
                    sw = width;
                    sh = height;
                    UIRoot.Instance.evtMove += moveHandle;
                    UIRoot.Instance.evtLeftUp += delHandle;
                    return false;
                };
        }

        public override string typeName
        {
            get { return "resizer"; }
        }

        public void append(UIWidget ui)
        {
            ui.paresent = mClient;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIResizer();
            ui.fromXML(node);

            var ret = node.Attributes.GetNamedItem("length");
            string strRet = (ret == null) ? UIRoot.Instance.getProperty("length") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                ui.width = strRet.castInt();
                ui.height = ui.width;
                UIRoot.Instance.setProperty("length", strRet);
            }

            ret = node.Attributes.GetNamedItem("width");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("width") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                ui.width = strRet.castInt();
                UIRoot.Instance.setProperty("width", strRet);
            }

            ret = node.Attributes.GetNamedItem("height");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("height") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                ui.height = strRet.castInt();
                UIRoot.Instance.setProperty("height", strRet);
            }

            ret = node.Attributes.GetNamedItem("color");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("color") : ((ret.Value == "NA") ? null : ret.Value);
            if (strRet != null)
            {
                (ui as UIResizer).mClient.fillColor = (uint)strRet.castHex();
                UIRoot.Instance.setProperty("color", strRet);
            }

            ui.paresent = p;
            return node.ChildNodes;
        }
        
    }
}
