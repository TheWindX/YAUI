using System;
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
            width = 256;
            height = 256;
            mClient = appendFromXML(@"
<rect debugName='UIResizer_client' size='512' clip='true' expand='true'>
</rect>
") as UIRect;
            mResizer = appendFromXML(@"
<rect debugName='UIResizer_resizer' size='32' align='rightBottom'>
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

        public override string typeName
        {
            get { return "resizer"; }
        }

        public void append(UIWidget ui)
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

            int w = schemes.width;
            int h = schemes.height;
            uint fc = (uint)schemes.fillColor;
            uint sc = (uint)schemes.strokeColor;
            bool br = true;

            h = w = getAttr<int>(node, "length", schemes.width, out br);
            if (!br)
            {
                w = getAttr<int>(node, "width", schemes.width, out br);
                h = getAttr<int>(node, "height", schemes.height, out br);
            }

            fc = (uint)getAttr<EColorUtil>(node, "color", schemes.fillColor, out br);
            if (!br)
            {
                fc = getAttr(node, "color", (uint)(schemes.fillColor), out br);
            }

            ui.width = w;
            ui.height = h;

            UIRoot.Instance.loadXMLChildren(node.ChildNodes, (ui as UIResizer).mClient, null);
            
            var rsz = (ui as UIResizer);
            rsz.fromXML(node);//属性属于client
            rsz.mClient.fromXML(node);//属性属于client
            rsz.mClient.name = null;//name 是 resizer的属性

            //edit 不能设置到client里
            rsz.mClient.dragAble = false;
            rsz.mClient.rotateAble = false;
            rsz.mClient.scaleAble = false;

            ui.paresent = p;
            return null;
        }
        
    }
}
