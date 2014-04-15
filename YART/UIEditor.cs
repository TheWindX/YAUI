using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class UIEditor : UIStub
    {
        const string XMLLAYOUT = @"
<rect clip='true' shrink='true' color='DarkGoldenrod' layout='vertical' padding='5' dragAble='true' rotateAble='true' scaleAble='true'>
    <resizer length='512' layout='vertical' layoutFilled='true'>
        <blank length='32' layout='horizon' layoutFilled='true' layoutInverse='true' expandX='true'>
            <lable text='x'></lable>
            <blank layout='horizon' layoutFilled='true'>
                <lable text='template' size='12'></lable>
                <blank name='tabCtn' expandY='true' shrink='true' layout='horizon'>
                </blank>
                <lable name='toggleCommandCtr' text='∧' style='bold' offsetY='5'></lable>
            </blank>
        </blank>
        <rect name='commandCtn' expandX='true' height='64'></rect>
        <blank name='clients' layout='horizon' layoutFilled='true'></blank>
    </resizer>
</rect>
";

        UIWidget mPages = null;
        public UIWidget mTabCtn = null;
        public UIWidget mCommandCtn = null;
        public UILable mToggleCommandCtr = null;
        public UIEditor()
        {
            appendFromXML(XMLLAYOUT);
            mPages = childOfPath("clients");
            mPages.name = "";//no name polution

            mTabCtn = childOfPath("tabCtn");
            mTabCtn.name = "";
            mCommandCtn = childOfPath("commandCtn");
            mCommandCtn.name = "";

            mToggleCommandCtr = childOfPath("toggleCommandCtr") as UILable;
            mToggleCommandCtr.evtOnLMUp += (ui, x, y) =>
                {
                    toggleMenu();
                    return false;
                };
            this.adjustLayout();
        }

        #region
        public class CMenu
        {
            const string xmltabCtn = @"
                    <rect derived='false' width='128' expandY='true'>
                        <lable name='tabName' text='tools' align='center' ></lable>
                    </rect>
";
            const string xmlcommandCtn = @"
                    <rect derived='false' expand='true'>
                        <lable name='cmdName' align='center'></lable>
                    </rect>
";
            public string mMenuName;
            public UIEditor mEditor;
            public CMenu(UIEditor edt, string menuName)
            {
                mMenuName = menuName;
                mEditor = edt;

                mTab = UIRoot.Instance.loadFromXML(xmltabCtn);
                var lb = mTab.childOf("tabName") as UILable;
                lb.text = menuName;
                mCommands = UIRoot.Instance.loadFromXML(xmlcommandCtn);
                lb = mCommands.childOfPath("cmdName") as UILable;
                lb.text = menuName;

                mTab.paresent = edt.mTabCtn;
                mCommands.paresent = edt.mCommandCtn;

                mTab.evtOnLMDown += (ui, x, y) =>
                {
                    return false;
                };
                mTab.evtOnLMUp += (ui, x, y) =>
                    {
                        mEditor.setCurrentMenu(mMenuName);
                        return false;
                    };
            }

            public void show(bool bshow)
            {
                mCommands.visible = bshow;
            }

            public void select(bool bSelect)
            {
                if (bSelect)
                {
                    (mTab as UIRect).fillColor = (uint)EColorUtil.red;
                    show(true);
                }
                else
                {
                    (mTab as UIRect).fillColor = (uint)EColorUtil.silver;
                    show(false);
                }
            }

            const string xmlItem = @"
                    <rect width='64' expandY='true' rotateAble='false'>
                        <lable name='itemName' align='center'></lable>
                    </rect>
";
            public void addItem(string itemName, Action act)
            {
                var item = UIRoot.Instance.loadFromXML(xmlItem);
                var lb = item.childOf("itemName") as UILable;
                lb.text = itemName;
                item.evtOnLMDown += (ui, x, y) =>
                {
                    if (act != null) act();
                    return false;
                };

                item.paresent = mCommands;
            }

            public UIWidget mTab = null;
            public UIWidget mCommands = null;
        }

        Dictionary<string, CMenu> mMenus = new Dictionary<string, CMenu>();
        public CMenu currentMenu = null;

        public void toggleMenu()
        {
            mCommandCtn.visible = !mCommandCtn.visible;
            if (mCommandCtn.visible)
                mToggleCommandCtr.text = "∧";
            else
                mToggleCommandCtr.text = "∨";
            setDirty(true);
        }

        public void addMenu(string menuName)
        {
            var m = new CMenu(this, menuName);
            mMenus.Add(menuName, m);
            setCurrentMenu(menuName);
        }

        CMenu getMenu(string mName)
        {
            CMenu ret;
            mMenus.TryGetValue(mName, out ret);
            return ret;
        }

        public void addMenuItem(string menuName, string itemName, Action cmd)
        {
            var m = getMenu(menuName);
            if (m != null)
            {
                m.addItem(itemName, cmd);
            }

        }

        public void setCurrentMenu(string menuName)
        {
            if (currentMenu != null && currentMenu.mMenuName == menuName)
            {
                return;
            }
            CMenu m;
            if (mMenus.TryGetValue(menuName, out m))
            {
                m.select(true);
            }
            else return;

            if (currentMenu != null)
            {
                currentMenu.select(false);
            }
            currentMenu = m;
            setDirty(true);
        }

        #endregion

        #region page
        public class CPage
        {
            const string XMLPAGE = @"
<rect name='root' length='256' layout='horizon' layoutInverse='true' layoutFilled='true' expandY='true'>
    <rect name='split' width='8' expandY='true' color='red'></rect>
    <rect layout='vertical' layoutFilled='true' color='yellow'>
        <rect name='tab' color='orange' width='128' height='24'>
            <lable name='tabName' text='tab' align='center'></lable>
        </rect>
        <rect name='client' color='blue'></rect>
    </rect>
</rect>
";
            static int idCount = 0;

            public int id = 0;
            public UIWidget mRoot = null;
            public UIRect mSplit = null;
            public UIWidget mTab = null;
            public UILable mTabName  = null;
            public UIWidget mClient = null;
            
            public CPage mNext = null;
            public CPage mPre = null;
            public IEnumerable<CPage> getIter()
            {
                CPage pg = this;
                do
                {
                    if (pg == null) break;
                    CPage ret = pg;
                    pg = pg.mNext;
                    yield return ret;
                } while (pg != this);

            }

            public CPage(string name, UIWidget p)
            {
                id = idCount++;
                mRoot = UIRoot.Instance.loadFromXML(XMLPAGE);
                mRoot.name = "";
                mSplit = mRoot.childOfPath("split") as UIRect;
                mSplit.name = "";
                mTab = mRoot.childOfPath("tab") as UIRect;
                mTab.name = "";
                mTabName = mTab.childOf("tabName") as UILable;
                mTabName.name = "";
                mTabName.text = name;
                mClient = mRoot.childOfPath("client");
                mClient.name = "";
                mRoot.paresent = p;
            }

            public void detach()
            {
                mRoot.paresent = null;
            }
        }





        CPage mPageMain = null;
        CPage mPageLast = null;
        CPage mPageCurrent = null;

        public void relayoutPages()
        {
            CPage cur = mPageMain;
            int c = cur.getIter().Count();
            int pageWidth = (int)((float)mPages.width / c);
            foreach (var elem in cur.getIter())
            {
                elem.mRoot.width = pageWidth;
            }
        }

        public CPage pushPage(string name)
        {
            CPage ret = null;
            if (mPageMain == null)
            {
                mPageMain = new CPage(name, mPages );
                mPageMain.mPre = mPageMain;
                mPageMain.mNext = mPageMain;
                mPageLast = mPageMain;
                mPageCurrent = mPageMain;
                ret = mPageCurrent;
            }
            else
            {
                mPageCurrent = new CPage(name, mPages);
                mPageCurrent.mPre = mPageLast;
                mPageLast.mNext = mPageCurrent;
                mPageCurrent.mNext = mPageMain;
                mPageMain.mPre = mPageCurrent;
                mPageLast = mPageCurrent;
                ret = mPageCurrent;
            }

            ret.mSplit.evtOnLMDownClear();
            Action<int, int> moveHandle = (x, y) =>
            {
                var dt = ret.mSplit.updateFixpointDelta(x, y);
                //Console.WriteLine(dt.X);
                ret.mRoot.width = ret.mRoot.width + dt.X;
                ret.mNext.mRoot.width = ret.mNext.mRoot.width - dt.X;
                
                //Console.WriteLine(ret.mRoot.width);
                UIRoot.Instance.root.adjustLayout();
                ret.mSplit.beginFixpoint(x, y);
                ret.mRoot.attrs["oringWidth"] = ret.mRoot.width;
                ret.mRoot.setDirty(true);
            };

            Action<int, int> upHandle = null; ;
            upHandle = (x, y) =>
            {
                UIRoot.Instance.evtMove -= moveHandle;
                UIRoot.Instance.evtLeftUp -= upHandle;
            };
            mPageCurrent.mSplit.evtOnLMDown += (ui, x, y) =>
                {
                    ret.mSplit.beginFixpoint(x, y);
                    ret.mRoot.attrs["oringWidth"] = ret.mRoot.width;
                    ret.mSplit.attrs["oringPx"] = ret.mSplit.px;
                    UIRoot.Instance.evtMove += moveHandle;
                    UIRoot.Instance.evtLeftUp += upHandle;
                    return false;
                };
            relayoutPages();
            return mPageCurrent;
        }

        public void popPage()
        {
            if (mPageMain == null) return;
            if (mPageLast == mPageMain) return;
            removePage(mPageLast);
            if (mPageCurrent == mPageLast)
            {
                mPageCurrent = mPageLast.mPre;
            }
            mPageLast = mPageLast.mPre;
        }

        public CPage getMainPage()
        {
            return mPageMain;
        }

        public CPage getCurrentPage()
        {
            return mPageCurrent;
        }

        public CPage setPrePage()
        {
            if (mPageMain == null) return null;
            mPageCurrent = mPageCurrent.mPre;
            return mPageCurrent;
        }

        public CPage setNextPage()
        {
            if (mPageMain == null) return null;
            mPageCurrent = mPageCurrent.mNext;
            return mPageCurrent;
        }

        void removePage(CPage pg)
        {
            pg.mPre.mNext = pg.mNext;
            pg.mNext.mPre = pg.mPre;
            pg.detach();
        }

        public void removeCurrentPage()
        {
            if (mPageMain == null) return;
            if (mPageCurrent == mPageMain) return;
            var oldPage = mPageCurrent;
            removePage(mPageCurrent);
        }

        public UIWidget getPageClient(int id)
        {
            var pg = getMainPage().getIter().First(page => page.id == id);
            return (pg == null) ? null : pg.mClient;
        }

        #endregion
    }
}
