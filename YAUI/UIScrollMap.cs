using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_YAUI
{
    public class UIScrolledMap : UIBlank
    {

        internal UIMap mMapClient = null;
        UIRect mMiniMapRect = null;
        UIMap mMiniMap = null;
        float mMiniMapSize = 128;

        public override string typeName
        {
            get { return "scrolledMap"; }
        }

        public UIScrolledMap()
        {
            width = schemes.frameWidth;
            height = schemes.frameHeight;
            mMapClient = appendFromXML(@"<map dragAble='true'></map>") as UIMap;
            
            UIRoot.Instance.evtLeftUp += (x, y) =>
                {
                    showMini();
                    setDirty(true);
                };

            mMiniMapRect = appendFromXML(@"
<blank clip='true' padding='16' shrink='true' fillColor='transparent' align='rightTop'>
<rect size='128' strokeColor='transparent' fillColor='transparent'><map>
</map></rect></blank>").findByTag("rect") as UIRect;
            mMiniMap = mMiniMapRect.findByTag("map") as UIMap;
            mMiniMapRect.height = mMiniMapRect.width = mMiniMapSize;
            mMiniMapRect.alignParesent = EAlign.rightTop;

            mMiniMap.evtOnPostDraw += g =>
                {
                    mMapClient.doDraw(g);
                };
        }

        public void showMini()
        {
            mMiniMap.clear();
            RectangleF? rcop = mMapClient.getChildrenOccupy();
            if (!rcop.HasValue) return;
            var rcChildren = rcop.Value.transform(mMapClient.getLocalMatrix() );
            var rcMap = new RectangleF(0, 0, width, height);
            var rcTotal = rcChildren.expand(rcMap);

            var scaleX = mMiniMapSize / rcTotal.Width;
            var scaleY = mMiniMapSize / rcTotal.Height;

            scaleY = scaleX = min(scaleX, scaleY);

            mMiniMap.setScale(scaleX, scaleY);
            var rcInMap = rcTotal.transform(mMiniMap.getLocalMatrix () );
            
            mMiniMap.px -= rcInMap.Left;
            mMiniMap.py -= rcInMap.Top;

            string xmlFmt = "<rect px='{0}' py='{1}' width='{2}' height='{3}' fillColor='0x33ffffff' strokeColor='{4}'></rect>";
            string xmlChildren = string.Format(xmlFmt, rcChildren.Left, rcChildren.Top, rcChildren.Width, rcChildren.Height, "0x44ff0000");
            string xmlWindow = string.Format(xmlFmt, rcMap.Left, rcMap.Top, rcMap.Width, rcMap.Height, "0x440000ff");

            var childrenUi = mMiniMap.appendFromXML(xmlChildren);
            var windowUI = mMiniMap.appendFromXML(xmlWindow);

            //show mini of chindren
        }

        public override bool onDraw(Graphics g)
        {
            if (clip)
            {
                var r = clipRect;
                GraphicsPath p = new GraphicsPath();
                p.AddRectangle(r);
                g.SetClip(p, CombineMode.Intersect);
            }

            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                (mChildrent[i] as UIWidget).doDraw(g);
            }

            //mMapClient.doDraw(g);

            return false;
        }

        public void appendUI(UIWidget ui)
        {
            ui.paresent = mMapClient;
            showMini();
            setDirty(true);
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIScrolledMap();
            
            ui.fromXML(node);

            UIRoot.Instance.loadXMLChildren(node.ChildNodes, (ui as UIScrolledMap).mMapClient, null);

            ui.paresent = p;
            (ui as UIScrolledMap).showMini();
            return null;
        }
    }
}
