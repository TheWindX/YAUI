﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            width = 256;
            height = 256;
            mClient = appendFromXML(@"
<rect size='512' clip='true' expand='true'>
</rect>
") as UIRect;
            mResizer = appendFromXML(@"
<rect size='32' color='silver' align='rightBottom'>
</rect>
") as UIRect;
            float spx = 0;
            float spy = 0;
            float sw = 0;
            float sh = 0;
            Action<int, int> moveHandle = (nx, ny) =>
            {
                mResizer.updateFixPoint(nx, ny);
                var dspx = (float)(mResizer.px - spx);
                var dspy = (float)(mResizer.py - spy);
                width = max(sw + dspx, 0);
                height = max(sh + dspy, 0);
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
                    mResizer.beginFixPoint((float)x, (float)y);
                    spx = mResizer.px;
                    spy = mResizer.py;
                    sw = width;
                    sh = height;
                    UIRoot.Instance.evtMove += moveHandle;
                    UIRoot.Instance.evtLeftUp += delHandle;
                    return false;
                };
        }

        public override void clear()
        {
            mClient.clear();
        }

        public override string typeName
        {
            get { return "resizer"; }
        }

        public void appendUI(UIWidget ui)
        {
            ui.paresent = mClient;
        }

        public UIWidget getClient()
        {
            return mClient;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIResizer();
            //var stubNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "nouse", "");//TODO, how to remove
            //ui.fromXML(stubNode);
            var ret = node.Attributes.GetNamedItem("name");
            if (ret != null) ui.name = ret.Value;

            float w = schemes.widgetWidth;
            float h = schemes.widgetHeight;
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            bool br = true;

            //h = w = getAttr<float>(node, "length", schemes.frameWidth, out br);
            //if (!br)
            //{
            //    w = getAttr<float>(node, "width", schemes.frameWidth, out br);
            //    h = getAttr<float>(node, "height", schemes.frameHeight, out br);
            //}

            fc = (uint)getProp<EColorUtil>(node, "color", (EColorUtil)schemes.fillColor, out br);
            if (!br)
            {
                fc = getProp(node, "color", (uint)(schemes.fillColor), out br);
            }

            
            
            UIRoot.Instance.loadXMLChildren(node.ChildNodes, (ui as UIResizer).mClient, null);
            
            var rsz = (ui as UIResizer);
            rsz.fromXML(node);//属性属于client
            rsz.mClient.fromXML(node);//属性属于client
            rsz.mClient.name = null;//name 是 resizer的属性
            rsz.mClient.fillColor = fc;
            ////resizer 属性不能覆盖的属性
            rsz.layout = ELayout.none;
            rsz.layoutInverse = false;
            rsz.layoutFilled = false;
            
            //rsz.mScalex = 1.0f;
            //rsz.mScaley = 1.0f;
            //rsz.direction = 0;



            rsz.wrap = false;
            //rsz.expandAbleY = rsz.expandAbleX = false;
            rsz.shrinkAble = false;
            
            //rsz.dragAble = false;
            //rsz.rotateAble = false;
            //rsz.scaleAble = false;

            rsz.mClient.setScale(1, 1);
            rsz.mClient.direction = 0;

            //client 不能覆盖的属性
            //rsz.mClient.layout = ELayout.none;
            //rsz.mClient.layoutInverse = false;
            //rsz.mClient.layoutFilled = false;
            //rsz.mClient.wrap = false;
            rsz.mClient.expandAbleY = rsz.mClient.expandAbleX = true;
            rsz.mClient.shrinkAble = false;

            rsz.mClient.dragAble = false;
            rsz.mClient.rotateAble = false;
            rsz.mClient.scaleAble = false;

            
            

            ui.paresent = p;
            return null;
        }
        
    }
}
