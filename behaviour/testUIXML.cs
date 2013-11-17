
/*
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
using System.Xml.Linq;
namespace ns_behaviour
{
    class testUIXML : Singleton<testUIXML>
    {
        public testUIXML()
        {
            Globals.Instance.evtOnInit += main;
        }

        const string xmllayout = @"
<stub name=""root"">
    <rect width=""96"" height=""64"" 
        offsetx=""4"" offsety=""4""
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887""> </rect>
    <rect width=""96"" height=""64"" 
        offsetx=""2"" offsety=""2""
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887""> </rect>
    <rect name=""main"" width=""96"" height=""64"" 
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887"">
        <rect width=""18"" height=""18"" offsetx=""4"" offsety=""4"">
            <lable text=""NS""
                size=""8""
                color=""0xff00ff00""
                align=""center"" alignParesent=""center"">
            </lable>
        </rect>
        <lable name=""text"" align=""center"" alignParesent=""center""> </lable>
    </rect>
</stub>";

        public void main()
        {
            var t = new NamespaceViewInstance(null);
            t.paresent = UIRoot.Instance.root;
            //UIStub r = new UIStub();
            //var ui = UIRoot.Instance.loadFromXML(xmllayout);
            //ui.dragAble = true;
            //ui.paresent = r;
            //r.paresent = UIRoot.Instance.root;
            //var lb = ui.childOfPath("main/text");
            //(lb as UILable).text = "12341234";
        }
    }
}
