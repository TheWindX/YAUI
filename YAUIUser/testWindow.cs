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
    <rect clip=""true"" resizeAble=""true"" layout=""vertical"" padding=""5"" dragAble=""true"" fillColor=""ff1ba1e2"">
        <lable name=""lable"" text=""top"" align=""leftTop""></lable>
        <lable name=""close"" text=""x"" align=""rightTop"" margin=""-3"" color=""ffe04343"" scaleX=""1.2""></lable>

        <blank width=""30"" height=""30""></blank>
        <blank name=""clientBlank"" width=""512"" height=""512"">
            <rect name=""client"" clip=""true"" expandAble=""true"" fillColor=""0xff3e4649""></rect>
        </blank>
        
        <rect name=""resizer"" width=""30"" height=""30"" align=""rightBottom""></rect>
    </rect>
";
        UIWidget mWindow = null;
        UIWidget mClientBlank = null;
        UIWidget mClose = null;
        UIWidget mResizer = null;
        UIWidget mLable = null;

        int mResizerStartX = 0;
        int mResizerStartY = 0;
        int mClientWidth = 0;
        int mClientHeight = 0;
        public UIWindow()
        {
            mWindow = UIRoot.Instance.loadFromXML(XMLLayout);
            mClientBlank = mWindow.childOfPath("clientBlank");
            mClose = mWindow.childOfPath("close");
            mResizer = mWindow.childOfPath("resizer");
            mLable = mWindow.childOfPath("lable");

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
        }
    }
}
