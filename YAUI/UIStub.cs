﻿/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
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
    class UIStub : UIWidget
    {
        public override string typeName
        {
            get { return "stub"; }
        }

        public override bool pickRectTest
        {
            get
            {
                return false;
            }
        }

        public override bool testPick(Point pos)
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
