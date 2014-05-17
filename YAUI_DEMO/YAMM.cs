using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class UIMM : ns_YAUtils.InheriteBase
    {
        public override Type[] deriveFrom()
        {
            return null;
        }

        //rect exclude is used for when children is outside of paresent pick rect
        const string XMLLayout = @"
        <round_rect strokeColor='*white' fillColor='*dimgray' rectExclude='false' layout='shrink' padding='0' dragAble='true'>
            <label color='white' text='template' name='label' margin='5'>
            </label>
            <round radius='6' align='left' fillColor='dimgray' alignParesent='right' rectExclude='false' name='subs'>
                <round radius='6' fillColor='dimgray' align='center'></round>
                <label text='—' size='10' color='white' align='center' offsetX='1' offsetY='-2'></label>
            </round>
            <round name='end' fillColor='gold' radius='4' align='right' alignParesent='left' rectExclude='false'>
            </round>
        </round_rect>
";
        public UIWidget mRoot = null;
        //public System.Drawing.RectangleF mRectangle = new System.Drawing.RectangleF();
        public UILine mline1 = null;
        public UILine mline2 = null;

        public UIMM()
        {
            mRoot = UI.Instance.fromXML(XMLLayout, false);
            mRoot.adjustLayout();
            var round = mRoot.findByPath("subs");
            mRoot.evtOnLMUp += (ui, x, y) => { return false; };//to disable paresent.round to handle event 
            round.evtOnLMUp += (ui, x, y) =>
            {
                var node = this.cast<YAMMNode>();
                node.toggleOpen();
                return false;
            };

            mline1 = new UILine();//line1
            mline1.color = (uint)EColorUtil.dimgray;
            mline1.setLineWidth(3);

            mline2 = new UILine();
            mline2.color = (uint)EColorUtil.dimgray;
            mline2.setLineWidth(3);

            mRoot.evtOnDragMove += (x, y) =>
                {
                    adjustLink();
                };
            mRoot.evtOnDragBegin += (x, y) =>
                {
                    var node = this.cast<YAMMNode>();
                    YAMM.Instance.setCurrent(node);
                };
            mRoot.evtOnDClick += (ui, x, y) =>
                {
                    rerangeChildren();
                    return false;
                };
        }

        public void adjustLink()
        {
            var root = cast<YAMMRootNode>();
            if (root != null) return;
            if (mRoot.paresent == null)
            {
                mline1.visible = false;
                mline2.visible = false;
                return;
            }
            if (mRoot.visible)
            {
                mline1.visible = true;
                mline2.visible = true;
            }
            mline1.paresent = mRoot.paresent;
            mline2.paresent = mRoot.paresent;
            //tow point
            PointF p1 = new PointF(0, 0);
            var endUi = mRoot.findByPath("end");
            PointF p2 = new PointF(mRoot.px + endUi.px, mRoot.py + endUi.py);

            mline1.setBegin(p1.X, p1.Y);
            mline1.setEnd(p1.X, p2.Y);
            mline2.setBegin(p1.X, p2.Y);
            mline2.setEnd(p2.X, p2.Y);

            mline1.setDepthTail();
            mline2.setDepthTail();
        }

        public void setText(string text)
        {
            (mRoot.findByPath("label") as UILabel).text = text;
        }
        const float mConstYInterval = 40;
        const float mConstXoffset = 30;
        const float mConstYoffset = -17;
        RectangleF getRect()
        {
            return mRoot.getRetangleInParesent();
        }
        public void rerangeChildren()
        {
            //clearLines();
            //Console.WriteLine("rerangeChildren:" + (mRoot.findByPath("label") as UILabel).text);
            treeNode tn = cast<treeNode>();
            var cs = tn.children();
            float height = 0;
            foreach (var elem in cs)
            {
                var uiElem = elem.cast<UIMM>();
                uiElem.rerangeChildren();
                height += uiElem.getRect().Height;
            }

            if (height == 0) return;

            height = height + (cs.Count - 1) * mConstYInterval;
            float xoffset = mConstXoffset;
            float yoffset = -height / 2 + mConstYoffset;
            for (int i = 0; i < cs.Count; ++i)
            {
                var c = cs[i];
                var cui = c.cast<UIMM>();
                float h = cui.getRect().Height;
                cui.mRoot.px = xoffset;
                cui.mRoot.py = yoffset + h / 2;
                yoffset += (h + mConstYInterval);
                cui.adjustLink();
            }
        }

        internal void show(bool p)
        {
            if(p)
            {
                this.mRoot.visible = true;
                this.mline1.visible = true;
                this.mline2.visible = true;
            }
            else
            {
                this.mRoot.visible = false;
                this.mline1.visible = false;
                this.mline2.visible = false;
            }
        }

        internal void breakLink()
        {
            show(false);
            this.mRoot.paresent = null;
            this.mline1.paresent = null;
            this.mline2.paresent = null;
        }
    }

    class treeNode : ns_YAUtils.InheriteBase
    {
        public treeNode mParesent = null;
        treeNode mNext = null;
        treeNode mPre = null;
        treeNode mFirstChild = null;

        public treeNode present()
        {
            return mParesent;
        }

        public treeNode root()
        {
            var n = this;
            var ret = n;
            while (n != null)
            {
                ret = n;
                n = n.mParesent;
            }
            return ret;
        }

        public treeNode firstChild()
        {
            return mFirstChild;
        }

        public treeNode next()
        {
            return mNext;
        }

        public treeNode pre()
        {
            return mPre;
        }

        public treeNode first()
        {
            if (mParesent != null)
            {
                return mParesent.firstChild();
            }
            return null;
        }

        public treeNode last()
        {
            if (mParesent == null) return null;
            treeNode ret = null;
            var firstC = mParesent.mFirstChild;
            treeNode n = firstC;
            if (n != null)
            {
                ret = n;
                n = n.next();
                while (n != firstC)
                {
                    ret = n;
                    n = n.next();
                }
            }
            return ret;
        }

        public void setParesent(treeNode t)
        {
            if (t == this || t == mParesent) return;
            if (mParesent != null)
            {
                if (this == mParesent.firstChild() )
                {
                    if (this == this.last()) mParesent.mFirstChild = null;
                    else mParesent.mFirstChild = this.mNext;
                }
                if(this.mNext != null)
                    this.mNext.mPre = this.mPre;
                if (this.mPre != null)
                    this.mPre.mNext = this.mNext;
            }
            if (t != null)
            {
                if (t.mFirstChild == null)
                {
                    t.mFirstChild = this;
                    this.mParesent = t;
                    this.mPre = this;
                    this.mNext = this;
                }
                else
                {
                    this.mParesent = t;
                    var f = t.firstChild();
                    var l = f.last();
                    this.mNext = l.next();
                    this.mPre = l;
                    l.mNext = this;
                    f.mPre = this;
                }
            }
        }

        public List<treeNode> children()
        {
            List<treeNode> ret = new List<treeNode>();
            treeNode n = mFirstChild;
            if (n != null)
            {
                ret.Add(n);
                n = n.next();
                while (n != mFirstChild)
                {
                    ret.Add(n);
                    n = n.next();
                }
            }

            return ret;
        }

        public override Type[] deriveFrom()
        {
            return null;
        }
    }

    
    class YAMMNode : ns_YAUtils.InheriteBase
    {
        public string mText = "template";
        public override Type[] deriveFrom()
        {
            return new Type[]{
                typeof(UIMM),typeof(treeNode),
            };
        }

        public YAMMNode()
        {
            UIMM ui = cast<UIMM>();
        }

        public void setParesent(YAMMNode t)
        {
            treeNode tselfNode = cast<treeNode>();
            treeNode pNode = t.cast<treeNode>();
            tselfNode.setParesent(pNode);
            //调整 自身UI
        }

        public void setText(string str)
        {
            UIMM ui =  cast<UIMM>();
            ui.setText(str);
        }

        public void attachChildrent(string text)
        {
            var c = new YAMMNode();
            c.setText(text);
            c.setParesent(this);
        }

        public void tryAdd(Action<YAMMNode> continuous)
        {
            var ui = cast<UIMM>();
            var pt = ui.mRoot.transformAbs(new PointF(100, 0));
            UI.Instance.input((int)pt.X, (int)pt.Y, str =>
            {
                var node = new YAMMNode();
                node.setText(str);
                appendChild(node);
                
                if (continuous != null)
                {
                    continuous(node);
                }
            });
        }

        public void appendChild(YAMMNode node)
        {
            if (node == null) return;
            node.cast<UIMM>().mRoot.paresent = this.cast<UIMM>().mRoot.findByPath("subs");
            //生成连线
            var selfNode = cast<treeNode>();
            node.cast<treeNode>().setParesent(selfNode);
            selfNode.root().cast<UIMM>().rerangeChildren();
        }

        internal void setChoose(bool p)
        {
            var ui = cast<UIMM>();
            var rc = ui.mRoot as UIRoundRect;
            if (p)
            {
                rc.fillColor = (uint)EColorUtil.darkkhaki;
            }
            else
            {
                rc.fillColor = (uint)EColorUtil.dimgray;
            }
        }

        bool mExpandOpen = true;
        internal void toggleOpen()
        {
            open(!mExpandOpen);
            return;
        }

        internal void open(bool b)
        {
            mExpandOpen = b;
            setExpand();
        }

        void setExpand()
        {
            UIMM ui = cast<UIMM>();
            var subs = ui.mRoot.findByPath("subs");
            if (mExpandOpen)
            {
                var lb = (subs.findByTag("label") as UILabel);
                lb.text = "—";
                lb.offsety = -2;
                lb.offsetx = 1;
            }
            else
            {
                var lb = (subs.findByTag("label") as UILabel);
                lb.text = "+";
                lb.offsety = lb.offsetx = 0;
                lb.offsetx = 2;
            }
            treeNode tn = cast<treeNode>();
            var cs = tn.children().ToArray();
            for (int i = 0; i < cs.Length; ++i)
            {
                var c = cs[i];
                c.cast<UIMM>().show(mExpandOpen);
            }
        }
    }

    class YAMMRootNode : ns_YAUtils.InheriteBase
    {
        public override Type[] deriveFrom()
        {
            return new Type[]{
                typeof(YAMMNode)
            };
        }
    }

    class YAMM : iTestInstance
    {
        YAMMRootNode mRoot = null;
        YAMMNode mCurrent = null;
        public void init()
        {
            mRoot = new YAMMRootNode();
            var node = mRoot.cast<YAMMNode>();
            
            mCurrent = node;
            node.setText("main_topic");

            var ui = mRoot.cast<UIMM>();
            ui.mRoot.evtOnChar += (self, k, isC, isS) =>
                {
                    Console.WriteLine(k +" is pressed");
                    if (k == 45/*insert*/)
                    {
                        mCurrent.tryAdd(delegate(YAMMNode n) { 
                            setCurrent(n);
                            setCenter(n);
                        });
                        
                    }
                    if (k == 46/*del*/)
                    {
                        if(mCurrent.cast<YAMMRootNode>() == null)
                            deleteNode(mCurrent);
                    }
                    else if (k == 37/*left*/)
                    {
                        var p = mCurrent.cast<treeNode>().mParesent;
                        if(p != null)
                        {
                            node = p.cast<YAMMNode>();
                            setCurrent(node);
                        }

                    }
                    else if (k == 39/*right*/)
                    {
                        var f = mCurrent.cast<treeNode>().firstChild();
                        if (f != null)
                        {
                            mCurrent.cast<YAMMNode>().open(true);
                            node = f.cast<YAMMNode>();
                            setCurrent(node);
                        }
                    }
                    else if (k == 38/*up*/)
                    {
                        var p = mCurrent.cast<treeNode>().pre();
                        if (p != null)
                        {
                            node = p.cast<YAMMNode>();
                            setCurrent(node);
                        }
                    }
                    else if (k == 40/*down*/)
                    {
                        var n = mCurrent.cast<treeNode>().next();
                        if (n != null)
                        {
                            node = n.cast<YAMMNode>();
                            setCurrent(node);
                        }
                    }
                    UI.Instance.flush();
                    return false;
                };
        }

        //使某个node 居中显示
        private void setCenter(YAMMNode node)
        {
            //
        }

        public void setCurrent(YAMMNode node)
        {
            if (mCurrent != null) mCurrent.setChoose(false);
            mCurrent = node;
            if (mCurrent != null) mCurrent.setChoose(true);
        }
        static public YAMM Instance = null;
        public void initMM()
        {
            init();
            var ui = mRoot.cast<UIMM>();
            ui.mRoot.px = 50;
            ui.mRoot.py = 256;
            ui.mRoot.paresent = UI.Instance.root;
            test();
            Instance = this;
        }

        public YAMMNode addNode(string name)
        {
            var node = new YAMMNode();
            node.setText(name);
            mCurrent.appendChild(node);

            return node;
        }

        public void deleteNode(YAMMNode node)
        {
            node.cast<treeNode>().setParesent(null);
            node.cast<UIMM>().breakLink();
            YAMM.Instance.mRoot.cast<UIMM>().rerangeChildren();
            UI.Instance.flush();
        }

        public void test()
        {
            var root = mCurrent;
            var n1 = addNode("topic1");
            var n2 = addNode("topic2");
            //var n3 = addNode("topic3");
            setCurrent(n1);
            var n11 = addNode("sub_topic11");
            var n12 = addNode("sub_topic12");

            setCurrent(n2);
            var n21 = addNode("sub_topic21");
            var n22 = addNode("sub_topic22");

            //setCurrent(n3);
            //var n31 = addNode("sub_topic31");
            //var n32 = addNode("sub_topic32");

            setCurrent(root);
            TimerManager.get().setTimeout(t =>
                {
                    root.cast<UIMM>().rerangeChildren();
                    UI.Instance.flush();
                }, 100);

            UI.Instance.flush();
        }

        ECategory iTestInstance.category()
        {
            return ECategory.demo;
        }

        string iTestInstance.title()
        {
            return "YAMM";
        }

        string iTestInstance.desc()
        {
            return @"   Yet Another Mind Manager
1. you can drag nodes;
2. pick a node then 'insert(key)' to add;
3. pick a node then 'del(key)' to remove;
4. pick a node then 'left, right, up, down' to iterator nodes;
5. double click to relayout;";
        }

        UIWidget iTestInstance.getAttach()
        {
            initMM();
            return mCurrent.cast<UIMM>().mRoot;
        }

        void iTestInstance.lostAttach()
        {
            return;
        }
    }
}
