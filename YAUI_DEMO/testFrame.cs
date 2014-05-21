using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ns_YAUI;
using ns_YAUtils;
namespace ns_YAUIUser
{
    class testFrame : Singleton<testFrame>
    {
        string XMLLayout = string.Format(@"
    <resizer scale='0.9' location='24' layout='vertical, filled' size='700, 768' editMode='dragAble, rotateAble, scaleAble' color='0xfff1f1f1'> <!--  root  -->
        <rect layout='expandX,'> <!--  title  -->
            <div layout='horizon, shrink' align='left'>
                <label text='YAUI' color='0xff545453' size='20'></label>
                <label text='.demo' color='0xff90c140' size='20' offsetX='-15'></label>
            </div>
            <label id='about' text='about' style='underline' link='true' align='right' color='0xff90c140' marginX='6'></label>
        </rect>

        <rect layout='horizon, expandX' height='32' color='0xffd3d3d3'> <!--  example & demo  tab-->
            <div layout='expandY' width='128'><label link='true' text='example' align='center' id='example'></label></div>
            <div layout='expandY' width='128'><label link='true' text='demo' align='center' id='demo'></label></div>
        </rect>

        <rect layout='horizon, filled' height='32'> <!--  tab & content -->
            <round_rect lineWidth='2' margin='4' layout='vertical, expandY' width='128' id='contents'> <!--  contents -->
            </round_rect>
            <round_rect lineWidth='2' margin='4' layout='vertical, filled'> <!--  content -->
                <label size='20' color='0x{0:x}' id='title'></label> <!--  content title-->
                <div layout='horizon, shrink' height='32'> <!--  prev/next -->
                    <rect layout='expandY' width='96' margin='*4' id='pre'>
                        <triangle forward='left' align='left' length='6' color='black'></triangle>
                        <label link='true' xlinkColor='0xff8ac007' text='prev' align='center' color='0x{0:x}'></label>
                    </rect>
                    <rect layout='expandY' width='96' id='next'>
                        <triangle forward='left' align='left' length='6' color='black'></triangle>
                        <label link='true' xlinkColor='0xff8ac007' text='next' align='center' color='0x{0:x}'></label>
                    </rect>
                </div>
                <label margin='4' id='desc' color='0x{0:x}' size='10'></label> <!-- desc -->
                
                <rect id='clientBG' margin='8' color='black'>
                    <scrolledMap id='client' expand='true' clip='true'>
                    </scrolledMap>
                </rect>
            </round_rect>
        </rect>
        <round_rect visible='false' id='aboutPage' size='384, 128' offsetY='0' strokeColor='transparent' color='0xff000000' align='rightTop'>
            <label name='url' text='https://github.com/TheWindX/YAUI' style='underline' color='0xff90c140' align='center' rectExclude='false'>
                <!--label text='Yet Another gUI' offsetY='-30' align='center'></label-->
                <label text='453588006@qq.com' offsetY='30' align='center'></label>
            </label>
            <label name='close' text='X' style='bold' color='0xff90c140' align='rightTop'></label>
        </round_rect>
    </resizer>
", colorFontNormal);
        const uint colorFontMark = 0xff8ac007;
        const uint colorFontNormal = 0xff545453;

        void setClientColor(uint col)
        {
            var ui = mRoot.findByPath("clientBG");
            (ui as UIRect).fillColor = col;
        }

        List<iTestInstance> mExampleInstance = new List<iTestInstance>();
        Dictionary<iTestInstance, UIWidget> mExample2Contents = new Dictionary<iTestInstance, UIWidget>();
        
        List<iTestInstance> mDemoInstance = new List<iTestInstance>();
        Dictionary<iTestInstance, UIWidget> mDemo2Contents = new Dictionary<iTestInstance, UIWidget>();

        ECategory mCurrentCategory = ECategory.example;
        int mContentIdx = 0;

        void getCategoryEnv(out List<iTestInstance> ins, out Dictionary<iTestInstance, UIWidget> ins2ui)
        {
            if(mCurrentCategory == ECategory.example)
            {
                ins = mExampleInstance;
                ins2ui = mExample2Contents;
            }
            else //if(mCurrentCategory == ECategory.demo)
            {
                ins = mDemoInstance;
                ins2ui = mDemo2Contents;
            }
        }

        int getCategorySize()
        {
            List<iTestInstance> ins = null;
            Dictionary<iTestInstance, UIWidget> ins2ui = null;
            getCategoryEnv(out ins, out ins2ui);
            return ins.Count;
        }
        
        UIWidget mRoot = null;

        string contentTemplate = string.Format(@"<div padding='4' layout='expandX' height='32'><label link='true' align='left' color='0x{0:x}'></label></div>", colorFontNormal);

        void initScheme()
        {
            schemes.strokeColor = 0xffd4d4d4;
            schemes.fillColor = 0xfff1f1f1;
            schemes.textColor = 0xff404040;
        }

        void initPreNext()
        {
            UIWidget pre = mRoot.findByID("pre");
            UIWidget next = mRoot.findByID("next");

            pre.evtOnLMUpClear();
            pre.evtOnLMUp += (ui, x, y) =>
                {
                    if (mContentIdx == 0) return false;
                    else
                    {
                        setContent(mContentIdx, false);
                        setContent(mContentIdx - 1, true);
                    }
                    return false;
                };

            next.evtOnLMUpClear();
            next.evtOnLMUp += (ui, x, y) =>
            {
                if (mContentIdx == getCategorySize()-1 ) return false;
                else
                {
                    setContent(mContentIdx, false);
                    setContent(mContentIdx + 1, true);
                }
                return false;
            };
        }

        int mTimeIDAboutPage = -1;
        bool mFadeIn = true;
        uint mFadeTime = 1000;
        void initFrame()
        {
            UILabel lbAbout = mRoot.findByID("about") as UILabel;
            UIRoundRect rrAbout = mRoot.findByID("aboutPage") as UIRoundRect;
            UILabel lbClose = mRoot.findByPath("close") as UILabel;

            UILabel lbURl = rrAbout.findByPath("url") as UILabel;
            UILabel lbAuth = lbURl.findByTag("label") as UILabel;

            Action<uint, uint> fade = (det, last) =>
            {
                rrAbout.visible = true;
                float rate = ((float)last / mFadeTime);
                if (rate > 1) rate = 1;
                if (!mFadeIn) rate = 1 - rate;
                uint color = (uint)(0x1000000 * (uint)(256 * rate));
                rrAbout.fillColor = color;
                lbClose.textColor = color + 0x0090c140;
                lbURl.textColor = color + 0x0090c140;
                lbAuth.textColor = color + 0x004f4f4f;

                UI.Instance.flush();
            };

            lbAbout.evtOnLMUp += (ui, x, y) =>
            {
                if (mTimeIDAboutPage != -1) return false;
                mTimeIDAboutPage = TimerManager.get().setInterval(fade, 10, t => { mFadeIn = !mFadeIn; mTimeIDAboutPage = -1; UI.Instance.flush(); }, mFadeTime);
                return false;
            };

            lbClose.evtOnLMUp += (ui, x, y) =>
            {
                if (mTimeIDAboutPage != -1) return false;
                mTimeIDAboutPage = TimerManager.get().setInterval(fade, 10, t => { mFadeIn = !mFadeIn; mTimeIDAboutPage = -1; rrAbout.visible = false; UI.Instance.flush(); }, mFadeTime);

                return false;
            };

            lbURl.evtOnLMUp += (ui, x, y) =>
            {
                System.Diagnostics.Process.Start(lbURl.text);
                return false;
            };

            lbAuth.evtOnLMUp += (ui, x, y) =>
            {
                System.Diagnostics.Process.Start("mailto:" + lbAuth.text);
                return false;
            };

            UILabel lbExample = mRoot.findByID("example") as UILabel;
            UILabel lbDemo = mRoot.findByID("demo") as UILabel;

            lbExample.evtOnLMUpClear();
            lbExample.evtOnLMUp += (ui, x, y) =>
            {
                setCategory(ECategory.example);
                return false;
            };

            lbDemo.evtOnLMUpClear();
            lbDemo.evtOnLMUp += (ui, x, y) =>
            {
                setCategory(ECategory.demo);
                return false;
            };
        }
        void initCategory(ECategory cate)
        {
            List<iTestInstance> contents = null;
            Dictionary<iTestInstance, UIWidget> content2ui = null;
            mCurrentCategory = cate;
            getCategoryEnv(out contents, out content2ui);

            for (int i = 0; i < contents.Count; ++i)
            {
                var cont = UI.Instance.fromXML(contentTemplate, false);
                var lb = cont.findByTag("label") as UILabel;
                lb.text = contents[i].title();

                content2ui[contents[i]] = cont;

                cont.evtOnLMUpClear();
                int idx = i;
                cont.evtOnLMUp += (ui, x, t) =>
                {
                    setContent(mContentIdx, false);
                    setContent(idx, true);
                    return false;
                };
            }
        }

        //选择category
        void setCategory(ECategory cate)
        {
            UILabel lbExample = mRoot.findByID("example") as UILabel;
            UILabel lbDemo = mRoot.findByID("demo") as UILabel;
            var contents = mRoot.findByID("contents");
            contents.clear();

            List<iTestInstance> testInstance = null;
            Dictionary<iTestInstance, UIWidget> testInstance2ui = null;

            if (cate == ECategory.example)
            {
                lbDemo.textStyle = EStyle.normal;
                lbDemo.textColor = colorFontNormal;

                lbExample.textStyle = EStyle.bold | EStyle.underline;
                lbExample.textColor = colorFontMark;

                testInstance = mExampleInstance;
                testInstance2ui = mExample2Contents;
            }
            else if (cate == ECategory.demo)
            {
                lbExample.textStyle = EStyle.normal;
                lbExample.textColor = colorFontNormal;

                lbDemo.textStyle = EStyle.bold | EStyle.underline;
                lbDemo.textColor = colorFontMark;

                testInstance = mDemoInstance;
                testInstance2ui = mDemo2Contents;
                
            }

            for (int i = 0; i < testInstance.Count; ++i)
            {
                var cont = testInstance2ui[testInstance[i]];
                cont.paresent = contents;
            }
            setContent(mContentIdx, false);
            mCurrentCategory = cate;
            setContent(0, true);
        }

        //选择 content
        void setContent(int idx, bool select)
        {
            
            List<iTestInstance> instances;
            Dictionary<iTestInstance, UIWidget> ins2ui;
            getCategoryEnv(out instances, out ins2ui);

            iTestInstance ins = null;
            UIWidget cont = null;

            ins = instances[idx];
            cont = ins2ui[ins];
            var client = mRoot.findByID("client") as UIScrolledMap;
            if (select)
            {
                (cont.findByTag("label") as UILabel).textColor = colorFontMark;
                (cont.findByTag("label") as UILabel).textStyle = EStyle.underline | EStyle.bold;
                mContentIdx = idx;

                (mRoot.findByID("desc") as UILabel).text = ins.desc();
                (mRoot.findByID("title") as UILabel).text = ins.title();
                //UIRoot.Instance.setPropertyderived(false);
                client.resetTransform();
                client.clear();
                client.appendUI(ins.getAttach());
            }
            else
            {
                (cont.findByTag("label") as UILabel).textColor = colorFontNormal;
                (cont.findByTag("label") as UILabel).textStyle = EStyle.normal;
                client.clear();
                ins.lostAttach();
            }

            mRoot.setDirty(true);
        }

        void addInstance(iTestInstance ins)
        {
            if (ins.category() == ECategory.example)
            {
                mExampleInstance.Add(ins);
            }
            else if (ins.category() == ECategory.demo)
            {
                mDemoInstance.Add(ins);
            }
        }

        public testFrame()
        {
            initScheme();
            mRoot = UIRoot.Instance.root.appendFromXML(XMLLayout);

            addInstance(new testPrimarys());
            addInstance(new testTransform());
            addInstance(new testAlign());
            addInstance(new testHierarchy());
            addInstance(new testLayout());
            addInstance(new testLayout2());
            addInstance(new testTips());
            addInstance(new testMenu());

            addInstance(new YAGame());
            addInstance(new YAMM());

            initFrame();
            initCategory(ECategory.example);
            initCategory(ECategory.demo);
            initPreNext();

            setCategory(ECategory.example);
            setContent(mContentIdx, false);
            setContent(1, true);
            UI.Instance.setWindowSize(768, 768);
        }
    }

    public enum ECategory
    {
        example,
        demo,
    }

    public interface iTestInstance
    {
        ECategory category();
        string title();
        string desc();
        UIWidget getAttach(); //content to show
        void lostAttach(); //清理资源
    }

}

