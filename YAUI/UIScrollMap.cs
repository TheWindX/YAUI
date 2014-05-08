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
        UIBlank mMiniMapDiv = null;
        UIRect mMiniMapRect = null;
        UIMap mMiniMap = null;
        UIRect mMiniWinow = null;
        float mMiniMapSize = 128;

        public override string typeName
        {
            get { return "scrolledMap"; }
        }

        const string xmlFmt = "<rect px='{0}' py='{1}' width='{2}' height='{3}' fillColor='0x33aaaaaa' ></rect>";
        public UIScrolledMap()
        {
            width = schemes.frameWidth;
            height = schemes.frameHeight;
            
            dragAble = true;//默认dragAble
            rotateAble = true;//默认dragAble
            scaleAble = true;//默认scaleAble
            mMapClient = appendFromXML(@"<map name='client'></map>") as UIMap;
            
            evtOnLMUp += (ui, x, y) =>
                {
                    showMini();
                    setDirty(true);
                    return false;
                };

            mMiniMapDiv = appendFromXML(@"
<div name='mini' clip='true' padding='16' shrink='true' align='rightTop'>
<rect size='128' fillColor='transparent' strokeColor='transparent'><map>
</map></rect></div>") as UIBlank;
            mMiniMapRect = mMiniMapDiv.findByTag("rect") as UIRect;
            mMiniMap = mMiniMapRect.findByTag("map") as UIMap;
            mMiniMapRect.height = mMiniMapRect.width = mMiniMapSize;
            mMiniMapRect.alignParesent = EAlign.rightTop;

            mMiniMap.evtOnPostDraw += g =>
                {
                    mMapClient.doDraw(g);
                };


            string xmlWindow = string.Format(xmlFmt, 0, 0, 128, 128);

            //var childrenUi = mMiniMap.appendFromXML(xmlChildren);
            mMiniWinow = mMiniMap.appendFromXML(xmlWindow) as UIRect;
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

            
            //string xmlChildren = string.Format(xmlFmt, rcChildren.Left, rcChildren.Top, rcChildren.Width, rcChildren.Height, "0x44ff0000");
            string xmlWindow = string.Format(xmlFmt, rcMap.Left, rcMap.Top, rcMap.Width, rcMap.Height);

            mMiniWinow.px = rcMap.Left;
            mMiniWinow.py = rcMap.Top;
            mMiniWinow.width = rcMap.Width;
            mMiniWinow.height = rcMap.Height;
            ////var childrenUi = mMiniMap.appendFromXML(xmlChildren);
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
            mMiniMapDiv.doDraw(g);
            mMapClient.doDraw(g);
            
            return false;
        }

        public override void clear()
        {
            mMapClient.clear();
        }

        public void appendUI(UIWidget ui)
        {
            if (ui == null) return;
            ui.paresent = mMapClient;
            showMini();
            setDirty(true);
        }

        public void resetTransform()
        {
            mMapClient.setScale(1, 1);
            mMapClient.py = mMapClient.px = 0;
            mMapClient.direction = 0;
        }

        public static XmlNodeList fromXML(XmlNode node, out UIWidget ui, UIWidget p)
        {
            ui = new UIScrolledMap();
            
            ui.fromXML(node);

            //mMapClient, 继承dragAble, scaleAble, rotateAble
            (ui as UIScrolledMap).mMapClient.dragAble = ui.dragAble;
            (ui as UIScrolledMap).mMapClient.rotateAble = ui.rotateAble;
            (ui as UIScrolledMap).mMapClient.scaleAble = ui.scaleAble;
            //ui.dragAble = false ; 
            ui.rotateAble = false;
            ui.scaleAble = false;

            UIRoot.Instance.loadXMLChildren(node.ChildNodes, (ui as UIScrolledMap).mMapClient, null);

            ui.paresent = p;
            (ui as UIScrolledMap).showMini();
            return null;
        }
    }
}
