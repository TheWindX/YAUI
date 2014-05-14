using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class UIMM : ns_YAUtils.InheriteBase
    {
        public override Type[] deriveFrom()
        {
            return null;
        }

        const string XMLLayout = @"
<div rectExclude='false' layout='horizon, shrink' dragAble='true'>
        <round_rect rectExclude='false' layout='shrink' padding='0'>
            <label text='template' name='label' margin='5'>
            </label>
            <round radius='8' align='left' alignParesent='right' name='subs' rectExclude='false'>
                <label text='+' align='center' offset='2'></label>
            </round>
            <round radius='4' align='right' alignParesent='left' rectExclude='false'>
            </round>
        </round_rect>
</div>
";
        public UIWidget mRoot = null;
        //public System.Drawing.RectangleF mRectangle = new System.Drawing.RectangleF();
        
        public UIMM()
        {
            mRoot = UI.Instance.fromXML(XMLLayout, false);
            

            var round = mRoot.findByPath("subs");
            round.evtOnLMUp += (ui, x, y) =>
            {
                var node = this.cast<YAMMNode>();
                node.toggleOpen();
                return false;
            };
        }


        public void setText(string text)
        {
            (mRoot.findByPath("label") as UILabel).text = text;
        }
        const float mConstYInterval = 40;
        const float mConstXoffset = 20;
        const float mConstYoffset = -17;
        float getHeight()
        {
            var rr = mRoot.findByTag("round_rect");
            return rr.height;
        }
        public void rerangeChildren()
        {
            treeNode tn = cast<treeNode>();
            var cs = tn.children();
            var h = getHeight();
            float height = (cs.Count-1) * h + (cs.Count - 1) * mConstYInterval;
            float xoffset = mConstXoffset;
            float yoffset = -height/2+mConstYoffset;
            for (int i = 0; i < cs.Count; ++i)
            {
                var c = cs[i];
                var cui = c.cast<UIMM>();
                cui.mRoot.px = xoffset;
                cui.mRoot.py = yoffset;
                yoffset += (h + mConstYInterval);
            }
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
                else
                {   
                    this.mNext.mPre = this.mPre;
                    this.mPre.mNext = this.mNext;
                }
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
            UI.Instance.input(100, 100, str =>
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
            node.cast<treeNode>().setParesent(this.cast<treeNode>());
            cast<UIMM>().rerangeChildren();
        }

        internal void setChoose(bool p)
        {
            var ui = cast<UIMM>();
            var rc = ui.mRoot.findByTag("round_rect") as UIRoundRect;
            if (p)
            {   
                rc.fillColor = (uint)EColorUtil.red;
            }
            else
            {
                rc.fillColor = (uint)EColorUtil.silver;
            }
        }

        internal void toggleOpen()
        {
            return;
        }

        //public void rerangeChildren()
        //{
        //    treeNode tn = cast<treeNode>();
        //    var cs = tn.children();
        //    for (int i = 0; i < cs.Count; ++i)
        //    {
        //        var c = cs[i];
        //        var cui = c.cast<UIMM>();
        //        cui.mRectangle;mRectangle
        //    }
        //}
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

    class YAMM : Singleton<YAMM>
    {
        YAMMRootNode mRoot = null;
        YAMMNode mCurrent = null;
        public void init()
        {
            mRoot = new YAMMRootNode();
            var node = mRoot.cast<YAMMNode>();
            
            mCurrent = node;
            node.setText("mainTopic");

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
            
        }

        public void setCurrent(YAMMNode node)
        {
            if (mCurrent != null) mCurrent.setChoose(false);
            mCurrent = node;
            if (mCurrent != null) mCurrent.setChoose(true);
        }

        public YAMM()
        {
            init();
            var ui = mRoot.cast<UIMM>();
            UI.Instance.setUICenter(ui.mRoot);
            ui.mRoot.paresent = UI.Instance.root;
        }
    }
}
