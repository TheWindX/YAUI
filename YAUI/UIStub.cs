/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: null use container of UIs, cannot handler event & trasform
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace ns_YAUI
{
    public class UIStub : UIWidget
    {
        public override string typeName
        {
            get { return "stub"; }
        }

        public override bool testRectPick
        {
            get
            {
                return false;
            }
        }

        public override RectangleF drawRect
        {
	        get 
	        { 
                var rc = getChildrenOccupy();
                if (rc.Value != null)
                {
                    return rc.Value;
                }
                else
                {
                    return new RectangleF(0, 0, schemes.widgetWidth, schemes.widgetHeight);
                }
            }
        }
       
        public override bool testSelfPick(PointF pos)
        {
            return false;
        }

        public static XmlNodeList fromXML(XmlNode nd, out UIWidget ui, UIWidget p)
        {
            ui = new UIStub();
            ui.fromXML(nd);
            ui.paresent = p;
            return nd.ChildNodes;
        }
    }
}
