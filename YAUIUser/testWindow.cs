using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class UIWindow
    {
        const string XMLLayout = @"
    <rect clip=""true"" shrink=""true"" layout=""vertical"" padding=""5"" dragAble=""true"" fillColor=""ff1ba1e2"">
        <lable name=""lable"" text=""top"" align=""leftTop""></lable>
        <lable name=""close"" text=""x"" align=""rightTop"" margin=""-3"" color=""ffe04343"" scaleX=""1.2""></lable>
        <blank width=""30"" height=""30""></blank>
        
        <blank name=""editor"" width=""512"" height=""512"" layout=""vertical"" layoutFilled=""true"">
            <rect name=""tabContainer"" layout=""horizen"" height=""30"" expandX=""true"" clip=""true"" fillColor=""0xffbfdbff"">
                <lable name=""tabToggle"" text=""∧"" color=""ff000000"" size=""10"" margin=""8"" align=""rightMiddle""></lable>
            </rect>
            <rect name=""toolContainer"" layout=""horizen"" height=""100"" expandX=""true"" clip=""true"" fillColor=""0xff3e4649""></rect>
            <rect name=""clientContainer"" layout=""horizen"" expandX=""true"" clip=""true"" fillColor=""0xff3e4649""></rect>
        </blank>
        
        <rect name=""resizer"" width=""32"" height=""32"" align=""rightBottom""></rect>
    </rect>
";
        const string XMLTab = @"
    <rect clip=""true"" rightBottomCorner=""false"" leftBottomCorner=""false"" width=""64"" expandY=""true"" radius=""8"" fillColor=""0xffd7e3f2"">
        <lable text=""tool"" color=""0xff555555"" style=""normal"" align=""center""></lable>
    </rect>
";
        const string XMLTool = @"
    <rect expand=""true"" layout=""horizen"" fillColor=""0xffd7e3f2""></rect>
";
        UIWidget mWindow = null;
        UIWidget mClientBlank = null;
        UIWidget mClose = null;
        UIWidget mResizer = null;
        UIWidget mLable = null;

        UIWidget mToolCtn = null;
        UIWidget mTabCtn = null;
        UIWidget mClientCtn = null;
        UIWidget mTabToggle = null;

        int mResizerStartX = 0;
        int mResizerStartY = 0;
        int mClientWidth = 0;
        int mClientHeight = 0;
        public UIWindow()
        {
            mWindow = UIRoot.Instance.loadFromXML(XMLLayout);
            mClientBlank = mWindow.childOfPath("editor");
            mClose = mWindow.childOfPath("close");
            mResizer = mWindow.childOfPath("resizer");
            mLable = mWindow.childOfPath("lable");
            mClientCtn = mWindow.childOfPath("editor/clientContainer");
            mTabCtn = mWindow.childOfPath("editor/tabContainer");
            mToolCtn = mWindow.childOfPath("editor/toolContainer");
            mTabToggle = mWindow.childOfPath("editor/tabContainer/tabToggle");

            Action<int, int> moveHandle = (x, y) =>
            {
                var newpt = mClientBlank.invertTransformParesentAbs(new System.Drawing.Point(x, y));
                var oldpt = mClientBlank.invertTransformParesentAbs(new System.Drawing.Point(mResizerStartX, mResizerStartY));
                var dx = newpt.X - oldpt.X;
                var dy = newpt.Y - oldpt.Y;
                mClientBlank.width = mClientWidth + dx;
                mClientBlank.height = mClientHeight + dy;
                mClientBlank.setDirty();
                UIRoot.Instance.dirtyRedraw();
            };

            mResizer.evtOnLMDown += (ui, x, y) =>
            {
                //var np = mResizer.transform(new System.Drawing.Point(x, y));
                mResizerStartX = x;
                mResizerStartY = y;

                mClientWidth = mClientBlank.width;
                mClientHeight = mClientBlank.height;
                UIRoot.Instance.evtMove += moveHandle;
                return false;
            };

            UIRoot.Instance.evtLeftUp += (x, y) =>
            {
                if (moveHandle != null)
                    UIRoot.Instance.evtMove -= moveHandle;
            };

            mClose.evtOnLMDown += (ui, x, y) =>
            {
                return false;
            };

            mClose.evtOnLMUp += (ui, x, y) =>
                {
                    dettach();
                    return false;
                };
            
            mWindow.evtChangeParesent += (oui, nui)=>
            {
                if (moveHandle != null && oui != null)
                    UIRoot.Instance.evtMove -= moveHandle;
            };

            mTabToggle.evtOnLMUp += (ui, x, y) =>
                {
                    if (mToolCtn.enable)
                    {
                        mToolCtn.enable = false;
                        mToolCtn.visible = false;
                        (mTabToggle as UILable).text = "∨";
                    }
                    else
                    {
                        mToolCtn.visible = true;
                        mToolCtn.enable = true;
                        (mTabToggle as UILable).text = "∧";
                    }
                    mToolCtn.setDirty(true);
                    return false;
                };
        }

        public void attach()
        {
            mWindow.paresent = UIRoot.Instance.root;
        }

        public void dettach()
        {
            mWindow.paresent = null;
            UIRoot.Instance.dirtyRedraw();
        }

        public void addTab()
        {
            var tab = UIRoot.Instance.loadFromXML(XMLTab);
            var tool = UIRoot.Instance.loadFromXML(XMLTool);
            tab.paresent = mTabCtn;
            tool.paresent = mToolCtn;
            mTabToggle.setDepthHead();
            mWindow.setDirty();
        }

        public void setLable(string name)
        {
            (mLable as UILable).text = name;
        }
    }


    class testWindow : Singleton<testWindow>
    {
        public testWindow()
        {
            UIWindow w = new UIWindow();
            w.setLable("lasdjfsadf");
            w.attach();
            w.addTab();
        }
    }
}
