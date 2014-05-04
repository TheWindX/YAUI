using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    class testFrame : Singleton<testFrame>
    {
        string XMLLayout = string.Format(@"
    <resizer layout='vertical, filled' size='1024, 768' editMode='dragAble, rotateAble, scaleAble' color='0xfff1f1f1'> <!--  root  -->
        <rect layout='expandX'> <!--  title  -->
            <div layout='horizon, shrink' align='left'>
                <label text='YAUI' color='0xff545453' size='20'></label>
                <label text='.com' color='0xff90c140' size='20' offsetX='-15'></label>
            </div>
        </rect>

        <rect layout='horizon, expandX' height='32' color='0xffd3d3d3'> <!--  example & demo  tab-->
            <div layout='expandY' width='128'><label link='true' text='example' align='center' id='example'></label></div>
            <div layout='expandY' width='128'><label link='true' text='demo' align='center' id='demo'></label></div>
        </rect>

        <rect layout='horizon, filled' height='32'> <!--  tab & content -->
            <round_rect margin='4' layout='vertical, expandY' width='128' id='contents'> <!--  contents -->
                <rect layout='expandX' height='32'><label text='test1' align='center'></label></rect>
                <rect layout='expandX' height='32'><label text='test2' align='center'></label></rect>
            </round_rect>
            <round_rect margin='4' layout='vertical, filled'> <!--  content -->
                <label size='20' color='0x{0:x}' id='title'></label> <!--  content title-->
                <div debugName='pre' layout='horizon, shrink' height='32'> <!--  prev/next -->
                    <rect layout='expandY' width='96' margin='*4' id='pre'>
                        <triangle forward='left' align='left' length='6' color='black'></triangle>
                        <label link='true' text='prev' align='center' color='0x{0:x}'></label>
                    </rect>
                    <rect debugName='next' layout='expandY' width='96' id='next'>
                        <triangle forward='left' align='left' length='6' color='black'></triangle>
                        <label link='true' text='next' align='center' color='0x{0:x}'></label>
                    </rect>
                </div>
                <label id='desc' color='0x{0:x}'></label> <!-- desc -->
                
                <rect margin='4'>
                    <scrolledMap id='client' expand='true' clip='true'>
                    </scrolledMap>
                </rect>
            </round_rect>
        </rect>
    </resizer>
", colorFontNormal);
        const uint colorFontMark = 0xff8ac007;
        const uint colorFontNormal = 0xff545453;

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

        string contentTemplate = string.Format(@"<div layout='expandX' height='32'><label link='true' align='center' color='0x{0:x}'></label></div>", colorFontNormal);

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

        void initCategory(ECategory cate)
        {
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

            ins = mExampleInstance[idx];
            cont = mExample2Contents[ins];
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

            addInstance(new TestInstance3() );
            addInstance(new TestInstance4() );
            addInstance(new testPrimarys());
            addInstance(new testTransform());
            addInstance(new testAlign());
            addInstance(new testHierarchy());
            addInstance(new testLayout());

            
            initCategory(ECategory.example);
            initCategory(ECategory.demo);
            initPreNext();

            setCategory(ECategory.example);
            setContent(mContentIdx, false);
            setContent(1, true);
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

    public class TestInstance3 : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.demo;
        }

        public string title()
        {
            return "dtest1";
        }

        public string desc()
        {
            return "";
        }

        public UIWidget getAttach() //content to show
        {
            return null;
        }

        public void lostAttach() //清理资源
        {
            return;
        }
    }

    public class TestInstance4 : iTestInstance
    {
        public ECategory category()
        {
            return ECategory.demo;
        }

        public string title()
        {
            return "dtest2";
        }

        public string desc()
        {
            return "";
        }

        public UIWidget getAttach() //content to show
        {
            return null;
        }

        public void lostAttach() //清理资源
        {
            return;
        }
    }
}

