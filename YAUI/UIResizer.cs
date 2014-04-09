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
            mClient = appendFromXML(@"
<rect noInherent='true' length='512' expand='true'>
</rect>
") as UIRect;
            mResizer = appendFromXML(@"
<rect noInherent='true' length='32' align='rightBottom'>
</rect>
") as UIRect;
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

            int w = 64;
            int h = 64;
            uint fc = 0xffaaaaaa;
            uint sc = 0xffffffff;
            bool br = true;

            h = w = getAttr<int>(node, "length", 64, out br);
            if (!br)
            {
                w = getAttr<int>(node, "width", 64, out br);
                h = getAttr<int>(node, "height", 64, out br);
            }

            fc = (uint)getAttr<EColorUtil>(node, "color", EColorUtil.silver, out br);
            if (!br)
            {
                fc = getAttr(node, "color", (uint)(EColorUtil.silver), out br);
            }

            (ui as UIResizer).mClient.fillColor = fc;

            UIRoot.Instance.loadXMLChildren(node.ChildNodes, (ui as UIResizer).mClient, null);

            ui.paresent = p;
            return null;
        }
        
    }
}