using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class UIWindow
    {
        const string XMLLayout = @"
    <rect name='root' size='512' clip='true' layout='vertical' layoutFilled='true' padding='5' dragAble='true' rotateAble='true' scaleAble='true' strokeColor='*red' color='ff1ba1e2'>
        <label  text='top' align='leftTop'></label>
        <rect size='24' fillColor='ffe04343' align='rightTop' offsetY='-5' >
            <label name='close' text='x' align='center' offsetX='1' offsetY='-1'></label>
        </rect>
        <div name='' size='30'></div>
        <rect name='tabContainer' layout='horizon' height='30' expandX='true' clip='true' color='0xffbfdbff'>
        <label name='tabToggle' text='∧' color='ff000000' size='10' margin='8' align='rightMiddle'></label>
        </rect>
        <rect name='toolContainer' layout='horizon' height='100' expandX='true' fillColor='*0xff3e4649'></rect>
        <rect name='clientContainer' clip='true'></rect>
        
        <rect name='resizer' size='32' align='rightBottom' fillColor='*'></rect>
    </rect>
";
        const string XMLTab = @"
    <rect clip='true' rightBottomCorner='false' leftBottomCorner='false' width='64' expandY='true' radius='8' fillColor='0xffd7e3f2'>
        <label text='name' color='0xff555555' style='normal' align='center'></label>
    </rect>
";
        const string XMLTool = @"
    <rect expand='true' layout='horizon' fillColor='0xffd7e3f2'></rect>
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

        float mResizerStartX = 0;
        float mResizerStartY = 0;
        float mClientWidth = 0;
        float mClientHeight = 0;
        public UIWindow()
        {
            mWindow = UIRoot.Instance.loadFromXML(XMLLayout);
            mClientBlank = mWindow.findByPath("editor");
            mClose = mWindow.findByPath("close");
            mResizer = mWindow.findByPath("resizer");
            mLable = mWindow.findByPath("label");
            mClientCtn = mWindow.findByPath("clientContainer");
            mTabCtn = mWindow.findByPath("tabContainer");
            mToolCtn = mWindow.findByPath("toolContainer");
            mTabToggle = mWindow.findByPath("tabContainer/tabToggle");

            Action<int, int> moveHandle = (x, y) =>
            {
                mResizer.updateFixPoint(x, y);
                float mResizerX = mResizer.px;
                float mResizerY = mResizer.py;
                var dx = mResizerX - mResizerStartX;
                var dy = mResizerY - mResizerStartY;
                mWindow.width = mClientWidth + dx;
                mWindow.height = mClientHeight + dy;
                mWindow.setDirty(true);
            };

            mResizer.evtOnLMDown += (ui, x, y) =>
            {
                //var np = mResizer.transform(new System.Drawing.PointF(x, y));
                mResizer.beginFixPoint((float)x, (float)y);
                mResizerStartX = mResizer.px;
                mResizerStartY = mResizer.py;
                mClientWidth = mWindow.width;
                mClientHeight = mWindow.height;
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

            mWindow.evtChangeParesent += (oui, nui) =>
            {
                if (moveHandle != null && oui != null)
                    UIRoot.Instance.evtMove -= moveHandle;
            };

            mTabCtn.evtOnLMUp += (ui, x, y) =>
            {
                if (mToolCtn.visible)
                {
                    mToolCtn.visible = false;
                    (mTabToggle as UILabel).text = "∨";
                }
                else
                {
                    mToolCtn.visible = true;
                    (mTabToggle as UILabel).text = "∧";
                }
                mToolCtn.setDirty(true);
                return false;
            };
            mWindow.setDirty(true);
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

        List<Tuple<UIWidget, UIWidget, int>> mToolTabs = new List<Tuple<UIWidget, UIWidget, int>>();
        public int addTab(string name)
        {
            var tab = UIRoot.Instance.loadFromXML(XMLTab);
            (tab.findByPath("label") as UILabel).text = name;
            var tool = UIRoot.Instance.loadFromXML(XMLTool);
            tab.paresent = mTabCtn;
            tool.paresent = mToolCtn;
            mTabToggle.setDepthHead();
            mWindow.setDirty();
            int id = mToolTabs.Count;
            mToolTabs.Add(Tuple.Create(tab, tool, id));
            return id;
        }

        public bool setTabName(int id, string name)
        {
            if (id >= mToolTabs.Count) return false;
            var tab = mToolTabs[id];
            (tab.Item1.findByPath("label") as UILabel).text = name;
            tab.Item1.setDirty(true);
            return true;
        }

        public void setWindowName(string name)
        {
            (mLable as UILabel).text = name;
            mLable.setDirty(true);
        }

        public UIWidget getToolContainer(int id)
        {
            if (id >= mToolTabs.Count) return null;
            var tab = mToolTabs[id];

            return tab.Item2;
        }
    }


    class testWindow : Singleton<testWindow>
    {
        
        public testWindow()
        {
            UIWindow mMain = new UIWindow();
            mMain.setWindowName("root");
            mMain.attach();
            mMain.addTab("tools");
            var ctn = mMain.getToolContainer(0);

            var ui = new UIRect(200f, 200f, 0xffffffff, 0xff888888);
            ui.paresent = ctn;
            
        }
    }
}
