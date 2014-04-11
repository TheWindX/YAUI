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
<rect clip='true' shrink='true' color='DarkGoldenrod' layout='vertical' padding='5' dragAble='true' rotateAble='true' scaleAble='true' >
    <blank name='tabs' length='32' layout='horizon'></blank>
    <lable text='template' align='leftTop' size='12'></lable>
    <resizer name='client' length='512' layout='horizon' layoutFilled='true'>
    </resizer>
</rect>
";

        UIResizer mResizer = null;
        UIBlank mTabs = null;
        public UIEditor()
        {
            appendFromXML(XMLLAYOUT);
            mResizer = childOfPath("client") as UIResizer;
            mResizer.name = "";//no name polution
            mTabs = childOfPath("tabs") as UIBlank;
            mTabs.name = "";//no name polution
        }
        

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
            int pageWidth = (int)((float)mResizer.getClient().width / c);
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
                mPageMain = new CPage(name, mResizer.getClient() );
                mPageMain.mPre = mPageMain;
                mPageMain.mNext = mPageMain;
                mPageLast = mPageMain;
                mPageCurrent = mPageMain;
                ret = mPageCurrent;
            }
            else
            {
                mPageCurrent = new CPage(name, mResizer.getClient());
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
    }
}
