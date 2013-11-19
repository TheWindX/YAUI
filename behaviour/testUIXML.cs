
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
    class ViewItemTemplate : UIStub
    {
        const string xmllayout = @"
    <rect name=""main"" width=""96"" height=""64"" 
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887"" 
        clip=""true"">
        <rect name=""typerc"" width=""18"" height=""18"" offsetx=""4"" offsety=""4"">
            <lable name=""type"" text=""NS""
                size=""8"" color=""0xff00ff00""
                align=""center"" alignParesent=""center"">
            </lable>
        </rect>
        <lable name=""lable"" text=""text"" size=""18""
                align=""center"" alignParesent=""center""> </lable>
    </rect>";

        UIRect mMainRc;
        UILable mLable;
        UILable mType;

        public ViewItemTemplate()
        {
            build();
        }

        void build()
        {
            this.dragAble = true;
            var ui = UIRoot.Instance.loadFromXML(xmllayout);
            ui.paresent = this;
            mType = this.childOfPath("main/typerc/type") as UILable;
            mLable = this.childOfPath("main/lable") as UILable;
            mMainRc = this.childOfPath("main") as UIRect;
        }

        public Point getSize()
        {
            return new Point(mMainRc.width, mMainRc.height);
        }

        public void setSize(int w, int h)
        {
            mMainRc.setSize(w, h);
        }

        public void setColor(uint color)
        {
            mMainRc.fillColor = color;
        }

        public void setLable(string lable, uint color = 0xffffffff)
        {
            mLable.text = lable;
            mLable.textColor = color;
        }

        public void setType(string strType)
        {
            mType.text = strType;
        }
    }


    class ViewWindowTemplate : UIStub
    {
        public const int titleHeight = 32;

        //note: dragAble cannot set in childs, for it in stub will seem right, but no this widget set position!!!
        const string xmllayout = @"
    <stub name=""root"">
        <rect name=""title_bar"" clip=""true"" width=""512"" height=""32"" 
            strokeColor=""0xffffffff"" fillColor=""0xFF0072E3"">
            <rect name=""typerc"" width=""18"" height=""18"" 
                align=""leftMiddle"" alignParesent=""leftMiddle"" offsetx=""4"" >
                <lable name=""type"" text=""NS""
                    size=""8"" color=""0xff00ff00""
                    align=""center"" alignParesent=""center"">
                </lable>
            </rect>
            <rect name=""close"" width=""32"" height=""32"" strokeColor=""0x0"" fillColor=""0x0""
                align=""rightTop"" alignParesent=""rightTop"" offsetx=""2"" offsety=""3"">
                <lable name=""lable"" text=""X"" size=""18"" color=""FF66B3FF""></lable>
            </rect>
            
            <lable name=""lable"" text=""text"" size=""18""
                    align=""leftMiddle"" alignParesent=""leftMiddle"" offsetx=""32"" >
            </lable>
        </rect>
        <rect name=""client"" clip=""true"" width=""512"" height=""512"" 
            align=""leftTop"" alignParesent=""leftTop"" offsety=""32"">
            <!--stub name=""clientArea"" dragAble=""true"" scaleAble=""true""></stub-->
            <lable name=""lable"" text=""text"" color=""0x77777777"" scalex=""4"" scaley=""4""
                 align=""center"" alignParesent=""center"">
            </lable>

            <rect name=""size_controller"" width=""32"" height=""32""
                align=""rightButtom"" alignParesent=""rightButtom"">
            </rect>
        </rect>
    </stub>";

        public void addItem(UIWidget ui, int x, int y)
        {
            if(ui == null) return;
            var client = this.childOfPath("root/client");
            ui.position = new Point(x, y);
            ui.paresent = client;
            //var sizeController = this.childOfPath("root/client/size_controller") as UIRect;
            //sizeController.setDepthHead();
        }

        public ViewWindowTemplate()
        {
            build();
        }

        void build()
        {
            this.dragAble = true;
            var ui = UIRoot.Instance.loadFromXML(xmllayout);
            ui.paresent = this;
            var sizeController = this.childOfPath("root/client/size_controller") as UIRect;
            sizeController.evtOnLMDown += onLMDown;

            var closeButton = this.childOfPath("root/title_bar/close") as UIRect;
            closeButton.evtOnLMDown += (sender, x, y) =>
                {
                    if (evtClose != null)
                        evtClose();
                    return false;
                };
        }

        Point mPtLast;
        UIWidget mControllUI;
        bool onLMDown(UIWidget _this, int x, int y)
        {
            mControllUI = _this;
            mPtLast = _this.invertTransformParesentAbs(new Point(x, y));
            Globals.Instance.mPainter.evtMove += onMMove;
            Globals.Instance.mPainter.evtLeftUp += onLMUp;
            return false;
        }

        void onLMUp(int x, int y)
        {
            Globals.Instance.mPainter.evtMove -= onMMove;
        }

        void onMMove(int x, int y)
        {
            var mPtDes = mControllUI.invertTransformParesentAbs(new Point(x, y));
            int dx = mPtDes.X - mPtLast.X;
            int dy = mPtDes.Y - mPtLast.Y;
            mPtLast = mPtDes;
            Point sz = getSize();
            setSize(sz.X + dx, sz.Y + dy);
        }

        public Point getSize()
        {
            var client = this.childOfPath("root/client") as UIRect;
            return new Point(client.width, client.height);
        }

        public void setSize(int w, int h)
        {
            var title_bar = this.childOfPath("root/title_bar") as UIRect;
            var client = this.childOfPath("root/client") as UIRect;
            title_bar.width = w;
            client.width = w;
            client.height = h;
            if (evtResize != null)
                evtResize(w, h);
        }

        public void setTitle(string title)
        {
            var lbTitle = this.childOfPath("root/title_bar/lable") as UILable;
            var lbTitleClient = this.childOfPath("root/client/lable") as UILable;
            lbTitle.text = title;
            lbTitleClient.text = title;
        }

        public void setType(string type)
        {
            var lbType = this.childOfPath("root/title_bar/typerc/type") as UILable;
            lbType.text = type;
        }

        public event Action<int, int> evtResize;
        public event Action evtClose;

    }

    class testUIXML : Singleton<testUIXML>
    {
        public testUIXML()
        {
            Globals.Instance.evtOnInit += main;
        }

        public void main()
        {
            var ui = new ViewWindowTemplate();
            ui.dragAble = true;
            //ui.setSize(256, 256);
            //ui.setType("b");
            //ui.setLable("test", 0xff0000ff);
            ui.paresent = UIRoot.Instance.root;
            ui.setSize(100, 100);
            ui.setTitle("lasjdf");
            ui.setType("LOL");
            //var lb = ui.childOfPath("main/text");
            //(lb as UILable).text = "12341234";
        }
    }
}
