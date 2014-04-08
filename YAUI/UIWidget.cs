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

using System.Drawing.Drawing2D;
using System.Drawing;

using System.Xml;

using ns_utils;



namespace ns_YAUI
{
    public class UIWidget : Entity
    {
        #region Hierarchy
        void sortSibling()
        {
            if (mParesent != null)
            {
                mParesent.sortChildrent((a, b) =>
                {
                    var a1 = a as UIWidget;
                    var b1 = b as UIWidget;
                    if (a1.depth < b1.depth)
                    {
                        return -1;
                    }
                    else if (a1.depth == b1.depth)
                    {
                        return 0;
                    }
                    else
                        return 1;
                });

                for (int i = 0; i < (mParesent as UIWidget).mChildrent.Count(); ++i)
                {
                    var u = (mParesent as UIWidget).mChildrent[i] as UIWidget;
                    u.mDepth = i;
                    u.mGlobalDepth = (mParesent as UIWidget).mGlobalDepth + ((float)i) / 10;

                    for (int j = 0; j < u.mChildrent.Count(); ++j)
                    {
                        (u.mChildrent[j] as UIWidget).mGlobalDepth = u.mGlobalDepth + ((float)j) / 10;
                    }
                }
            }
        }

        public override Entity paresent
        {
            set
            {
                var oldParesent = base.paresent;
                if (value == oldParesent) return;

                base.paresent = value;
                if (base.paresent != null)
                {
                    (base.paresent as UIWidget).setDirty();
                }
                if (value != null)
                {
                    setDepthHead();
                    (paresent as UIWidget).setDirty();
                }
                //event
                if(evtChangeParesent != null)
                    evtChangeParesent(oldParesent as UIWidget, value as UIWidget);
            }
        }
        
        

        public IEnumerable<UIWidget> children()
        {
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                yield return mChildrent[i] as UIWidget;
            }
        }
        
        public UIWidget childOf(string name)
        {
            List<UIWidget> noNames = new List<UIWidget>();
            foreach (var elem in children() )
            {
                if (elem.name == name)
                {
                    return elem;
                }
                else if (elem.typeName == name)
                {
                    return elem;
                }
                else if (elem.name == null || elem.name == "")
                {
                    noNames.Add(elem);
                }
            }

            foreach (var elem in noNames)
            {
                var ui = elem.childOf(name);
                if (ui != null)
                    return ui;
            }

            return null;
        }

        public UIWidget childOfPath(string path)
        {
            string[] pathItems = path.Split('/');
            UIWidget p = this;
            foreach (var elem in pathItems)
            {
                p = p.childOf(elem);
                if (p == null) break;
            }
            return p;
        }

        public UIWidget topParesent
        {
            get
            {
                if (mParesent != null)
                {
                    return (mParesent as UIWidget).topParesent;
                }
                else
                {
                    return this;
                }
            }
        }

        float mGlobalDepth = 0;
        float mDepth = 0;
        public float depth
        {
            get { return mDepth; }
            set
            {
                mDepth = value;
                sortSibling();
            }
        }

        public void setDepthTail()
        {
            mDepth = mChildrent.Count + 1;
            sortSibling();
        }

        public void setDepthHead()
        {
            depth = -100;
            sortSibling();
        }

        public void setDepthDeeper()
        {
            mDepth += 1.5f;
            sortSibling();
        }

        public void setDepthShallower()
        {
            mDepth -= 1.5f;
            sortSibling();
        }

        ////在同一层次上的比较
        //static int levelCompare(UIWidget u1, UIWidget u2)
        //{
        //    UIWidget v1;
        //    UIWidget v2;

        //    var c = commonParesent(u1, u2, out v1, out v2);

        //    if (v1 == v2) return 0;
        //    else
        //    {
        //        if (v1.depth < v2.depth)
        //        {
        //            return 1;
        //        }
        //        else if (v1.depth == v2.depth)
        //        {
        //            return 0;
        //        }
        //        else
        //            return 1;
        //    }
        //}

        public static UIWidget commonParesent(UIWidget u1, UIWidget u2)
        {
            List<UIWidget> u1Plist = new List<UIWidget>();
            UIWidget t1 = u1;
            while (t1 != null)
            {
                u1Plist.Add(t1);
                t1 = t1.mParesent as UIWidget;
            }

            List<UIWidget> u2Plist = new List<UIWidget>();
            UIWidget t2 = u2;
            while (t2 != null)
            {
                u2Plist.Add(t2);
                t2 = t2.mParesent as UIWidget;
            }

            UIWidget c = null;
            int i1 = u1Plist.Count - 1;
            int i2 = u2Plist.Count - 1;
            UIWidget sub1 = null;
            UIWidget sub2 = null;
            for (; i1 >= 0 && i2 >= 0; )
            {
                var c1 = u1Plist[i1];
                var c2 = u2Plist[i2];
                sub1 = c1;
                sub2 = c2;

                if (c1 == c2)
                {
                    c = c1;
                }
                i1--;
                i2--;
            }
            return c;
        }
        #endregion

        #region properties
        public int ID = -1;

        //properties start
        public virtual Rectangle drawRect
        {
            get { return new Rectangle(); }
        }

        //properties start
        public virtual Rectangle clipRect
        {
            get 
            {
                var r = drawRect;
                r.Offset(1, 1);
                r.Width -= 1;
                r.Height -= 1;
                return r;
            }
        }

        public virtual int width
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public virtual int height
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }
        

        public Rectangle layoutRect
        {
            get
            {
                var rc = drawRect;
                rc.Location = new Point(rc.Location.X - marginX, rc.Location.Y - marginY);
                rc.Size = new Size(rc.Size.Width + marginX*2, rc.Size.Height + marginY*2);
                return rc;
            }
        }

        //遮挡矩形，如果 occupyRect 超过了 dirtyRect, 就需要向上 paresent重绘了，否则本地重绘就行
        protected virtual Rectangle dirtyRect
        {
            get{return new Rectangle(); }// rect
        }

        Rectangle? mOccupyRect = null;
        internal void invalidRect()
        {
            mOccupyRect = null;
        }
        
        public void setDirty(bool redrawImmediatly = false)
        {
            //this.invalidRect(); //no need, children count for occupy, //occupy count for dirty
            UIWidget p = this.paresent as UIWidget;
            if (p != null)
            {
                //if (p.name == "r3") Console.WriteLine("r3 is invalid");
                p.invalidRect();
                if (UIRoot.Instance.dirtyRoot == null) UIRoot.Instance.dirtyRoot = p;
                var d = commonParesent(UIRoot.Instance.dirtyRoot, p);
                d.invalidRect();
                UIRoot.Instance.dirtyRoot = d;
            }
            else
            {
                UIRoot.Instance.dirtyRoot = null;
            }
            if (redrawImmediatly)
            {
                UIRoot.Instance.dirtyRedraw();
            }
        }

        //向上找到
        internal UIWidget getDirtyRoot()
        {
            if (dirtyRect.Contains(occupyRect))
            {
                return this;
            }
            else
            {
                if (paresent != null)
                {
                    (paresent as UIWidget).invalidRect();
                    return (paresent as UIWidget).getDirtyRoot();
                }
                return this;
            }
        }

        protected Rectangle occupyRect
        {
            get
            {
                if (mOccupyRect != null)
                {
                    return mOccupyRect.Value;
                }

                mOccupyRect = null;
                foreach (var elem in children())
                {
                    if (!elem.visible) continue;//not count for invisble
                    if (mOccupyRect == null)
                    {
                        var rc = elem.occupyRect;
                        rc = expandRect(rc, elem.drawRect);
                        mOccupyRect = rc.transform(elem.transformMatrix);
                        //if (this.name == "r3") Console.WriteLine(elem.name + ":" + rc);
                    }
                    else
                    {
                        var rc = elem.occupyRect;
                        rc = expandRect(rc, elem.drawRect);
                        var elemRc = rc.transform(elem.transformMatrix);
                        mOccupyRect = expandRect(mOccupyRect.Value, elemRc);
                    }
                }

                if (mOccupyRect == null || clip) mOccupyRect = drawRect;
                
                return mOccupyRect.Value;
            }
        }

        public int borderX
        {
            get { return drawRect.Width; }
        }

        public int borderY
        {
            get { return drawRect.Height; }
        }

        public virtual Rectangle pickRect
        {
            get
            {
                return drawRect;
            }
        }
        public bool focus
        {
            set
            {
                if (value)
                {
                    topParesent.attrs["focus"] = this;
                }
                else
                {
                    topParesent.attrs["focus"] = null;
                }
            }
            get
            {
                object o = null;
                bool r = topParesent.attrs.TryGetValue("focus", out o);
                if (r)
                {
                    return object.ReferenceEquals(this, r);
                }
                return false;
            }
        }

        public int px
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public int py
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }


        bool mClip = false;
        public bool clip { get { return mClip; } set { mClip = value; } }

        bool mEnable = true;
        public bool enable { get { return mEnable; } set { mEnable = value; } }

        string mName;
        public string name
        {
            get { return mName; }
            set {mName = value;}
        }

        protected bool mVisiable = true;
        public bool visible
        {
            get
            {
                return mVisiable;
            }
            set
            {
                mVisiable = value;
            }
        }

        bool mPropagate = true;
        public bool propagate
        {
            get
            {
                return mPropagate;
            }
            set
            {
                mPropagate = value;
            }
        }

        public virtual string typeName
        {
            get { return "nulltype"; }
        }
        public Dictionary<string, object> attrs = new Dictionary<string, object>();

        //box model
        public int marginX = 0;
        public int marginY = 0;
        public int paddingX = 0;
        public int paddingY = 0;
        #endregion

        #region layout
        #region align
        public Point center
        {
            get
            {
                return transform(drawRect.center());
            }

            set
            {
                var pt = center;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftTop
        {
            get
            {
                return transform(drawRect.leftTop());
            }
            set
            {
                var pt = leftTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftMiddle
        {
            get
            {
                return transform(drawRect.leftMiddle());
            }
            set
            {
                var pt = leftMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point leftBottom
        {
            get
            {
                return transform(drawRect.leftBottom());
            }
            set
            {
                var pt = leftBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightTop
        {
            get
            {
                return transform(drawRect.rightTop());
            }
            set
            {
                var pt = rightTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightMiddle
        {
            get
            {
                return transform(drawRect.rightMiddle());
            }
            set
            {
                var pt = rightMiddle;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point rightBottom
        {
            get
            {
                return transform(drawRect.rightBottom());
            }
            set
            {
                var pt = rightBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point middleTop
        {
            get
            {
                return transform(drawRect.middleTop());
            }
            set
            {
                var pt = middleTop;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public Point middleBottom
        {
            get
            {
                return transform(drawRect.middleBottom());
            }
            set
            {
                var pt = middleBottom;
                int dx = value.X - pt.X;
                int dy = value.Y - pt.Y;
                position.X += dx;
                position.Y += dy;
            }
        }

        public enum EAlign
        {
            noAlign,
            center,
            leftTop,
            leftMiddle,
            leftBottom,
            rightTop,
            rightMiddle,
            rightBottom,
            middleTop,
            middleBottom,
        }

        EAlign mAlign = EAlign.noAlign;
        public EAlign align
        {
            get
            {
                return mAlign;
            }
            set
            {
                mAlign = value;
            }
        }

        EAlign mAlignParesent = EAlign.noAlign;
        public EAlign alignParesent
        {
            get
            {
                return mAlignParesent;
            }
            set
            {
                mAlignParesent = value;
            }

        }

        int mOffsetx = 0;
        public int offsetx
        {
            get
            {
                return mOffsetx;
            }
            set
            {
                mOffsetx = value;
            }
        }

        int mOffsety = 0;
        public int offsety
        {
            get
            {
                return mOffsety;
            }
            set
            {
                mOffsety = value;
            }
        }

        /// <returns> need ajust </returns>
        public virtual bool adjustAlign()
        {
            if(paresent == null)
                return false;

            if (mAlign == EAlign.noAlign || mAlignParesent == EAlign.noAlign)
            {
                return false;
            }

            var pui = (paresent as UIWidget);

            Point alignPos = new Point();
            switch (mAlignParesent)
            {
                case EAlign.center:
                    alignPos = paresent.invertTransform((paresent as UIWidget).center);
                    alignPos.X += offsetx;
                    alignPos.Y += offsety;
                    break;
                case EAlign.leftTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftTop);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    alignPos.Y += offsety + pui.paddingY + marginY;
                    break;
                case EAlign.leftMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftMiddle);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    break;
                case EAlign.leftBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).leftBottom);
                    alignPos.X += offsetx + pui.paddingX + marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.rightTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightTop);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety + pui.paddingY + marginY;
                    break;
                case EAlign.rightMiddle:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightMiddle);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    break;
                case EAlign.rightBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightBottom);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.middleTop:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleTop);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.middleBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleBottom);
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                default:
                    break
                    ;
            }

            switch (mAlign)
            {
                case EAlign.center:
                    this.center = alignPos;
                    break;
                case EAlign.leftTop:
                    this.leftTop = alignPos;
                    break;
                case EAlign.leftMiddle:
                    this.leftMiddle = alignPos;
                    break;
                case EAlign.leftBottom:
                    this.leftBottom = alignPos;
                    break;
                case EAlign.rightTop:
                    this.rightTop = alignPos;
                    break;
                case EAlign.rightMiddle:
                    this.rightMiddle = alignPos;
                    break;
                case EAlign.rightBottom:
                    this.rightBottom = alignPos;
                    break;
                case EAlign.middleTop:
                    this.middleTop = alignPos;
                    break;
                case EAlign.middleBottom:
                    this.middleBottom = alignPos;
                    break;
                default:
                    break;
            }

            return true;
        }
        #endregion

        #region layout implement
        
        protected void adjustExpand()
        {
            var p = paresent as UIWidget;
            if (this.expandAbleX)
            {
                if (p != null)
                {
                    position.X = p.paddingX + marginX;
                    width = p.width - (p.paddingX + marginX) * 2;
                }
            }

            if (this.expandAbleY)
            {
                if (p != null)
                {
                    position.Y = p.paddingY + marginY;
                    height = p.height - (p.paddingY + marginY) * 2;
                }
            }
        }

        public bool shrinkAble = false;//if shrinkAble, wrap is invalid
        public bool expandAbleX = false;//if expandAble, resizeAble & wrap is invalid //expand 到它的layoutRect
        public bool expandAbleY = false;//if expandAble, resizeAble & wrap is invalid //expand 到它的layoutRect

        public enum ELayout
        {
            none,
            vertical,
            horizon,
        }
        public ELayout mLayout = ELayout.none;
        public bool mLayoutInverse = false;

        public bool wrap = false;
        public bool layoutFilled = false;//最后一个layout的，填满
        //这个因与渲染的遍历次序不同,因此不能放到draw里
        protected virtual void adjustLayout()
        {
            if (!enable) return;

            mOccupyRect = null;//重新layout, 当然要重计 mOccupyRect

            bool bAjust = adjustAlign();
            adjustExpand();
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                var c = mChildrent[i] as UIWidget;
                c.adjustLayout();
            }
            
            List<UIWidget> noInLayout = new List<UIWidget>();//for if layout and child align coexist

            #region layout calc
            Point rb = new Point();
            Rectangle rc = new Rectangle(new Point(paddingX, paddingY), new Size(0, 0));
            rb = rc.rightBottom();
            int idxCount = 0;

            UIWidget lastLayoutUi = null;
            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                var c = mChildrent[i] as UIWidget;
                if (!c.enable) continue;

                if (c.align != EAlign.noAlign)
                {
                    noInLayout.Add(c);
                    continue;
                }
                
                if (mLayout == ELayout.vertical)
                {
                    if (c.expandAbleY)
                    {
                        noInLayout.Add(c);
                        continue;
                    }
                    else if(c.expandAbleX)
                    {
                        c.adjustExpand();
                    }
                    lastLayoutUi = c;

                    var pt = rc.leftBottom();
                    pt.X += c.marginX;
                    pt.Y += c.marginY;
                    
                    var rcOld = rc;
                    rc.Width = max(c.marginX*2+c.width, rc.Width);
                    rc.Height = rc.Height + c.marginY* 2 + c.height;
                    rb.X = max(rc.Right, rb.X);
                    rb.Y = max(rc.Bottom, rb.Y);
                    if (mLayoutInverse)
                    {
                        var ptInv = pt;
                        ptInv.Y = ( (drawRect.X*2+height)-pt.Y);
                        c.leftBottom = ptInv;
                    }
                    else
                    {
                        c.leftTop = pt;
                    }


                    idxCount++;
                    if ( this.wrap && rc.Bottom > this.drawRect.Height - paddingY && idxCount > 1)//只能容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new Point(rc.Location.X + rc.Width, paddingY);
                        rc.Size = new Size();
                    }
                }
                else if (mLayout == ELayout.horizon)
                {
                    if (c.expandAbleX)
                    {
                        noInLayout.Add(c);
                        continue;
                    }
                    else if (c.expandAbleY)
                    {
                        c.adjustExpand();
                    }
                    lastLayoutUi = c;

                    var pt = rc.rightTop();
                    pt.X += c.marginX;
                    pt.Y += c.marginY;
                    //c.leftTop = pt;

                    //var lr = c.layoutRect;
                    var rcOld = rc;
                    rc.Height = max(c.marginY*2+c.height, rc.Height);
                    rc.Width = rc.Width + c.marginX * 2 + c.width;
                    rb.X = max(rc.Right, rb.X);
                    rb.Y = max(rc.Bottom, rb.Y);

                    if (mLayoutInverse)
                    {
                        var ptInv = pt;
                        ptInv.X = ((drawRect.X * 2 + width) - pt.X);
                        c.rightTop = ptInv;
                    }
                    else
                    {
                        c.leftTop = pt;
                    }

                    idxCount++;
                    if ( this.wrap && rc.Right > this.drawRect.Width - paddingX && idxCount > 1)//至少容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new Point(paddingX, rc.Location.Y + rc.Height);
                        rc.Size = new Size();
                    }
                }
            }
            if (!this.wrap && this.shrinkAble) //有wrap不可shrink， shrink覆盖expand, shrink后需要重新计算 expand和align
            {
                width = (rb.X + paddingX);//right padding
                height = (rb.Y + paddingY);//button padding
            }
            else if(layoutFilled && lastLayoutUi != null) //layout的最后一个填满
            {
                if (mLayout == ELayout.horizon)
                {
                    lastLayoutUi.height = height - lastLayoutUi.py - paddingY - lastLayoutUi.marginY;

                    if (mLayoutInverse)
                    {
                        lastLayoutUi.width = lastLayoutUi.px + lastLayoutUi.width - paddingX - lastLayoutUi.marginX;
                        lastLayoutUi.px = paddingX + lastLayoutUi.marginX;
                    }
                    else
                    {
                        lastLayoutUi.width = width - lastLayoutUi.px - paddingX - lastLayoutUi.marginX;
                    }
                }
                else if (mLayout == ELayout.vertical)
                {
                    lastLayoutUi.width = width - lastLayoutUi.px - paddingX - lastLayoutUi.marginX;

                    if (mLayoutInverse)
                    {
                        lastLayoutUi.height = lastLayoutUi.py + lastLayoutUi.height - paddingY - lastLayoutUi.marginY;
                        lastLayoutUi.py = paddingY + lastLayoutUi.marginY;
                    }
                    else
                    {
                        lastLayoutUi.height = height - lastLayoutUi.py - paddingY - lastLayoutUi.marginY;
                    }
                }
            }

            for (int i = 0; i < noInLayout.Count; ++i)
            {
                var c = noInLayout[i];
                if (c.align != EAlign.noAlign)
                    c.adjustAlign();
                if (c.expandAbleX || c.expandAbleY)
                    c.adjustExpand();
            }
            #endregion
        }
        #endregion

        #endregion

        #region transform
        public Matrix transformMatrix
        {
            get
            {
                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.Rotate(mDir);
                m.Scale(mScalex, mScaley);
                return m;
            }
        }
        
        public void translate(int dx, int dy)
        {
            position.X += dx;
            position.Y += dy;
        }
        #endregion

        #region methods
        public UIWidget(){}


        #endregion

        #region events

        public Action<UIWidget, UIWidget> evtChangeParesent;

        public Action evtEnter;
        public Action evtExit;

        public delegate bool EvtMouse(UIWidget _this, int x, int y);
        public delegate bool EvtKeyboard(UIWidget _this, int kc, bool isControl, bool isShift);
        public delegate void EvtSizeChanged(UIWidget _this, int w, int h);


        public event EvtMouse evtOnMMove;
        public void evtOnMMoveClear()
        {
            evtOnMMove = null;
        }

        public event EvtMouse evtOnLMDown;
        public void evtOnLMDownClear()
        {
            evtOnLMDown = null;
        }
        public event EvtMouse evtOnLMUp;
        public void evtOnLMUpClear()
        {
            evtOnLMUp = null;
        }
        public event EvtMouse evtOnRMDown;
        public void evtOnRMDownClear()
        {
            evtOnRMDown = null;
        }
        public event EvtMouse evtOnRMUp;
        public void evtOnRMUpClear()
        {
            evtOnRMUp = null;
        }
        public event EvtMouse evtOnMMDown;
        public void evtOnMMDownClear()
        {
            evtOnMMDown = null;
        }
        public event EvtMouse evtOnMMUp;
        public void evtOnMMUpClear()
        {
            evtOnMMUp = null;
        }
        public event EvtMouse evtOnEnter;
        public void evtOnEnterClear()
        {
            evtOnEnter = null;
        }
        public event EvtMouse evtOnExit;
        public void evtOnExitClear()
        {
            evtOnExit = null;
        }
        public event EvtMouse evtOnDClick;
        public void evtOnDClickClear()
        {
            evtOnDClick = null;
        }

        public event Action evtPreDraw;
        public void evtPreDrawClear()
        {
            evtPreDraw = null;
        }

        public event EvtKeyboard evtOnChar;
        public void evtOnCharClear()
        {
            evtOnChar = null;
        }

        internal bool doEvtLeftDown(int x, int y)
        {
            if (evtOnLMDown == null)
            {
                return true;
            }
            return evtOnLMDown(this, x, y);
        }

        internal bool evtMove(int x, int y)
        {
            if (evtOnMMove == null)
            {
                return true;
            }
            return evtOnMMove(this, x, y);
        }

        internal bool doEvtLeftUp(int x, int y)
        {
            if (evtOnLMUp == null)
            {
                return true;
            }
            return evtOnLMUp(this, x, y);
        }

        internal bool doEvtRightDown(int x, int y)
        {
            if (evtOnRMDown == null)
            {
                return true;
            }
            return evtOnRMDown(this, x, y);
        }

        internal bool doEvtRightUp(int x, int y)
        {
            if (evtOnRMUp == null)
            {
                return true;
            }
            return evtOnRMUp(this, x, y);
        }

        internal bool doEvtMiddleDown(int x, int y)
        {
            if (evtOnMMDown == null)
            {
                return true;
            }
            return evtOnMMDown(this, x, y);
        }

        internal bool doEvtMiddleUp(int x, int y)
        {
            if (evtOnMMUp == null)
            {
                return true;
            }
            return evtOnMMUp(this, x, y);
        }

        internal bool doEvtOnEnter(int x, int y)
        {
            if (evtOnEnter == null)
            {
                return true;
            }
            return evtOnEnter(this, x, y);
        }

        internal bool doEvtOnExit(int x, int y)
        {
            if (evtOnExit == null)
            {
                return true;
            }
            return evtOnExit(this, x, y);
        }

        internal bool doEvtOnChar(int kc, bool isControl, bool isShift)
        {
            if (evtOnChar == null)
            {
                return true;
            }
            return evtOnChar(this, kc, isControl, isShift);
        }

        internal bool doEvtDoubleClick(int x, int y)
        {
            if (evtOnDClick == null)
            {
                return true;
            }
            return evtOnDClick(this, x, y);
        }
        #endregion

        #region utils
        static int idcount = 1000000;
        public static int newID
        {
            get
            {
                return idcount++;
            }
        }
        //for test 
        /// <summary>
        /// dragable
        /// </summary>
        bool mDragAble = false;
        public bool dragAble
        {
            set
            {
                mDragAble = value;
                if (mDragAble)
                {
                    evtOnLMDown += onDragBegin;
                }
                else
                {
                    evtOnLMDown -= onDragBegin;
                    onDragEnd(0, 0);
                }

            }

            get
            {
                return mDragAble;
            }
        }

        void onDragMove(int x, int y)
        {
            updateFixpoint(x, y);
            setDirty(true);
            return;
        }

        bool onDragBegin(UIWidget _this, int x, int y)
        {
            beginFixpoint(x, y);
            //这个改变先后关系
            this.setDepthHead();

            UIRoot.Instance.evtMove += onDragMove;
            UIRoot.Instance.evtLeftUp += onDragEnd;
            return false;
        }

        void onDragEnd(int x, int y)
        {
            UIRoot.Instance.evtMove -= onDragMove;
            UIRoot.Instance.evtLeftUp -= onDragEnd;
        }


        bool mRotateAble = false;
        public bool rotateAble
        {
            set
            {
                mRotateAble = value;
                if (mRotateAble)
                {
                    evtOnRMDown += onRotateBegin;
                }
                else
                {
                    evtOnRMDown -= onRotateBegin;
                    onRotateEnd(0, 0);
                }
            }

            get
            {
                return mRotateAble;
            }
        }

        bool onRotateBegin(UIWidget _this, int x, int y)
        {
            UIRoot.Instance.mEvtWheel += onRotateMove;
            UIRoot.Instance.mEvtRightUp += onRotateEnd;
            return false;
        }

        void onRotateMove(int delta)
        {
            beginFixpoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            mDir += delta * 0.2f;
            updateFixpoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            setDirty(true);
            return;
        }

        void onRotateEnd(int x, int y)
        {
            UIRoot.Instance.mEvtWheel -= onRotateMove;
            UIRoot.Instance.mEvtRightUp -= onRotateEnd;
        }

        /// <summary>
        /// dragable end
        /// </summary>


        /// <summary>
        /// scaleAble begin
        /// </summary>
        bool mScaleAble = false;
        public bool scaleAble
        {
            set
            {
                mScaleAble = value;
                if (mScaleAble)
                {
                    evtOnLMDown += onScaleBegin;
                    UIRoot.Instance.evtLeftUp += onScaleEnd;
                }
                else
                {
                    evtOnLMDown -= onScaleBegin;
                    UIRoot.Instance.evtLeftUp -= onScaleEnd;
                    onScaleEnd(0, 0);
                }
            }

            get
            {
                return mDragAble;
            }
        }

        void onScaleWheel(int delta)
        {
            beginFixpoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            float sc = 1;
            if (delta > 0) sc = 1.1f;
            else sc = 0.9f;
            mScalex += sc - 1;
            mScaley += sc - 1;
            updateFixpoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            setDirty(true);
            return;
        }

        bool onScaleBegin(UIWidget ui, int x, int y)
        {
            UIRoot.Instance.mEvtWheel += onScaleWheel;
            return false;
        }

        void onScaleEnd(int x, int y)
        {
            UIRoot.Instance.mEvtWheel -= onScaleWheel;
        }
        /// <summary>
        /// scaleAble end
        /// </summary>

        //utils
        protected static int min(int a, int b) { if (a < b)return a; else return b; }
        protected static int max(int a, int b) { if (a > b)return a; else return b; }
        protected static Rectangle expandRect(Rectangle r1, Rectangle r2)
        {
            int left = min(r1.Left, r2.Left);
            int top = min(r1.Top, r2.Top);
            int right = max(r1.Right, r2.Right);
            int buttom = max(r1.Bottom, r2.Bottom);

            return new Rectangle(left, top, right - left, buttom - top);
        }

        public XmlNodeList fromXML(XmlNode node)
        {
            string strRet = null;

            var ret = node.Attributes.GetNamedItem("name");
            if (ret != null) name = ret.Value;

            ret = node.Attributes.GetNamedItem("px");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("px") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("px", strRet);
            if (strRet != null)
            {
                px = strRet.castInt();
            }

            ret = node.Attributes.GetNamedItem("py");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("py") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("py", strRet);
            if (strRet != null)
            {
                py = strRet.castInt();
            }
            

            ret = node.Attributes.GetNamedItem("rotate");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("rotate") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("rotate", strRet);
            if (strRet != null)
            {
                mDir = strRet.castFloat();
            }
            

            ret = node.Attributes.GetNamedItem("scaleX");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("scaleX") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("scaleX", strRet);
            if (strRet != null)
            {
                mScalex = strRet.castFloat(1);
            }
            
            ret = node.Attributes.GetNamedItem("scaleY");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("scaleY") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("scaleY", strRet);
            if (strRet != null)
            {
                mScaley = strRet.castFloat(1);
            }

            ret = node.Attributes.GetNamedItem("scale");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("scale") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("scale", strRet);
            if (strRet != null)
            {
                mScalex = strRet.castFloat(1);
                mScaley = mScalex;
            }

            ret = node.Attributes.GetNamedItem("align");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("align") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("align", strRet);
            if (strRet != null) 
            {
                mAlign = (EAlign)Enum.Parse(typeof(EAlign), strRet);
                mAlignParesent = mAlign;
            }

            ret = node.Attributes.GetNamedItem("alignParesent");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("alignParesent") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("alignParesent", strRet);
            if (strRet != null)
            {
                mAlignParesent = (EAlign)Enum.Parse(typeof(EAlign), strRet);
            }

            ret = node.Attributes.GetNamedItem("offset");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("offset") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("offset", strRet);
            if (strRet != null)
            {
                mOffsetx = ret.Value.castInt();
                mOffsety = mOffsetx;
            }
            else
            {
                //note //offset is after align
                ret = node.Attributes.GetNamedItem("offsetX");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("offsetX") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("offsetX", strRet);
                if (strRet != null) 
                {
                    var ox = strRet.castInt(); mOffsetx = ox;
                }

                ret = node.Attributes.GetNamedItem("offsetY");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("offsetY") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("offsetY", strRet);
                if (strRet != null) 
                {
                    var oy = strRet.castInt(); mOffsety = oy;
                }
            }


            ret = node.Attributes.GetNamedItem("clip");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("clip") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("clip", strRet);
            if (strRet != null)
            {
                clip = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("enable");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("enable") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("enable", strRet);
            if (strRet != null)
            {
                enable = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("visible");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("visible") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("visible", strRet);
            if (strRet != null)
            {
                visible = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("dragAble");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("dragAble") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("dragAble", strRet);
            if (strRet != null)
            {
                dragAble = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("scaleAble");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("scaleAble") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("scaleAble", strRet);
            if (strRet != null)
            {
                scaleAble = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("rotateAble");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("rotateAble") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("rotateAble", strRet);
            if (strRet != null)
            {
                rotateAble = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("layout");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("layout") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("layout", strRet);
            if (strRet != null)
            {
                mLayout = (ELayout)Enum.Parse(typeof(ELayout), strRet);
            }

            ret = node.Attributes.GetNamedItem("layoutInverse");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("layoutInverse") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("layoutInverse", strRet);
            if (strRet != null)
            {
                mLayoutInverse = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("wrap");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("wrap") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("wrap", strRet);
            if (strRet != null)
            {
                wrap = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("layoutFilled");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("layoutFilled") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("layoutFilled", strRet);
            if (strRet != null)
            {
                layoutFilled = strRet.castBool();
            }

            ret = node.Attributes.GetNamedItem("shrink");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("shrink") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("shrink", strRet);
            if (strRet != null)
            {
                shrinkAble = strRet.castBool();
            }


            ret = node.Attributes.GetNamedItem("expand");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("expand") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("expand", strRet);
            if (strRet != null)
            {
                expandAbleX = strRet.castBool();
                expandAbleY = expandAbleX;
            }
            else
            {
                ret = node.Attributes.GetNamedItem("expandX");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("expandX") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("expandX", strRet);
                if (strRet != null)
                {
                    expandAbleX = strRet.castBool();
                }

                ret = node.Attributes.GetNamedItem("expandY");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("expandY") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("expandY", strRet);
                if (strRet != null)
                {
                    expandAbleY = strRet.castBool();
                }
            }

            ret = node.Attributes.GetNamedItem("margin");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("margin") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("margin", strRet);
            if (strRet != null)
            {
                marginX = strRet.castInt();
                marginY = marginX;
            }
            else
            {
                ret = node.Attributes.GetNamedItem("marginX");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("marginX") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("marginX", strRet);
                if (strRet != null)
                {
                    marginX = strRet.castInt();
                }

                ret = node.Attributes.GetNamedItem("marginY");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("marginY") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("marginY", strRet);
                if (strRet != null)
                {
                    marginY = strRet.castInt();
                }
            }

            ret = node.Attributes.GetNamedItem("padding");
            strRet = (ret == null) ? UIRoot.Instance.getProperty("padding") : ((ret.Value=="NA")?null:ret.Value);
            UIRoot.Instance.setProperty("padding", strRet);
            if (strRet != null)
            {
                paddingY = strRet.castInt();
                paddingX = paddingY;
            }
            else
            {
                ret = node.Attributes.GetNamedItem("paddingX");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("paddingX") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("paddingX", strRet);
                if (strRet != null)
                {
                    paddingX = strRet.castInt();
                }

                ret = node.Attributes.GetNamedItem("paddingY");
                strRet = (ret == null) ? UIRoot.Instance.getProperty("paddingY") : ((ret.Value=="NA")?null:ret.Value);
                UIRoot.Instance.setProperty("paddingY", strRet);
                if (strRet != null)
                {
                    paddingY = strRet.castInt();
                }
            }

            return node.ChildNodes;
        }

        public bool appendFromXML(string xml)
        {
            var ui = UIRoot.Instance.loadFromXML(xml);
            if (ui == null) return false;
            else ui.paresent = this;
            return true;
        }

        #endregion

        #region others

        public virtual bool postTestPick(Point pos)
        {
            return false;
        }


        public virtual bool preTestPick(Point pos)
        {
            return true;
        }

        public bool doTestPick(Point pos, out UIWidget ui, bool testEnable = true)
        {
            if (testEnable && !enable)
            {
                ui = null;
                return false;
            }

            var t = transformMatrix.Clone();
            t.Invert();
            var ps = new Point[] { pos };
            t.TransformPoints(ps);
            var newpos = ps[0];

            var r = pickRect;
            if (!r.Contains(newpos))
            {
                ui = null;
                return false;
            }

            bool ret = preTestPick(newpos);
            if (!ret)
            {
                ui = null;
                return false;
            }

            UIWidget picked = null;
            foreach (UIWidget elem in children())
            {
                if (elem.doTestPick(newpos, out picked, testEnable))
                {
                    //uilist.Add(picked);
                    break;
                }
            }

            if (picked != null)
            {
                ui = picked;
                return true;
            }

            ret = postTestPick(newpos);
            if (!ret)
            {
                ui = null;
                return false;
            }
            ui = this;
            return true;
        }

        internal void doDrawAlone(Graphics g)
        {
            var pList = new List<UIWidget>();
            adjustLayout();
            UIWidget cur = paresent as UIWidget;
            for (; cur != null; )
            {
                pList.Add(cur);
                cur = cur.paresent as UIWidget;
            }
            var gs = g.Save();
            foreach(UIWidget u in pList.Reverse<UIWidget>() )
            {
                Matrix _mtx = g.Transform;
                _mtx.Multiply(u.getLocalMatrix());
                g.Transform = _mtx;
                if (u.clip)
                {
                    var r = u.drawRect;
                    r.Offset(1, 1);
                    r.Width -= 1;
                    r.Height -= 1;
                    GraphicsPath p = new GraphicsPath();
                    p.AddRectangle(r);
                    g.SetClip(p, CombineMode.Intersect);
                }
            }
            doDraw(g);
            g.Restore(gs);
        }

        internal void doDraw(Graphics g)
        {
            if (!mVisiable)
                return;
            if (evtPreDraw != null)
                    evtPreDraw();

            var gs = g.Save();

            Matrix _mtx = g.Transform;
            _mtx.Multiply(this.getLocalMatrix());
            g.Transform = _mtx;

            //catch it?
            onDraw(g);

            if (clip)
            {
                var r = clipRect;
                GraphicsPath p = new GraphicsPath();
                p.AddRectangle(r);
                g.SetClip(p, CombineMode.Intersect);
            }

            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                (mChildrent[i] as UIWidget).doDraw(g);
            }

            //for text drawing            

            g.Restore(gs);
            //popMatrix(g);
        }

        internal virtual void onDraw(Graphics g) { }
        #endregion
    }
}
