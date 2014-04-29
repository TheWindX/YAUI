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
    using EvtMouse = Func<UIWidget, int, int, bool>;
    using EvtKeyboard = Func<UIWidget, int, bool, bool, bool>;
    using EvtSizeChanged = Func<UIWidget, int, int, bool>;
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

        public UIWidget childOfTag(string name)
        {
            List<UIWidget> noNames = new List<UIWidget>();
            var clist = children().ToList();
            foreach (var elem in clist)
            {
                if (elem.typeName == name)
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
                var ui = elem.childOfTag(name);
                if (ui != null)
                    return ui;
            }

            return null;
        }

        public UIWidget childOfName(string name)
        {
            List<UIWidget> noNames = new List<UIWidget>();
            var clist = children().ToList();
            foreach (var elem in clist)
            {
                if (elem.name == name)
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
                var ui = elem.childOfName(name);
                if (ui != null)
                    return ui;
            }

            return null;
        }

        public UIWidget findByPath(string path)
        {
            string[] pathItems = path.Split('/');
            UIWidget p = this;
            foreach (var elem in pathItems)
            {
                p = p.childOfName(elem);
                if (p == null) break;
            }
            return p;
        }

        public UIWidget findByTag(string path)
        {
            string[] pathItems = path.Split('/');
            UIWidget p = this;
            foreach (var elem in pathItems)
            {
                p = p.childOfTag(elem);
                if (p == null) break;
            }
            return p;
        }

        public UIWidget findByID(string id)
        {
            return UIRoot.Instance.getIDWidget(id);
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
            mDepth = -1;
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
        //TODO, is it too big properties for a tiny widget? 
        //may be i can use proto inherent and table look up.
        public int ID = -1;
        public string StringID = "";
        //properties start
        public virtual RectangleF drawRect
        {
            get { return new RectangleF(0, 0, width, height); }
        }

        //properties start
        public virtual RectangleF clipRect
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

        public virtual float width
        {
            get
            {
                return 64;
            }
            set
            {
            }
        }

        public virtual float height
        {
            get
            {
                return 64;
            }
            set
            {
            }
        }
        

        public RectangleF layoutRect
        {
            get
            {
                var rc = drawRect;
                rc.Location = new PointF(rc.Location.X - marginX, rc.Location.Y - marginY);
                rc.Size = new SizeF(rc.Size.Width + marginX*2, rc.Size.Height + marginY*2);
                return rc;
            }
        }

        //遮挡矩形，如果 occupyRect 超过了 dirtyRect, 就需要向上 paresent重绘了，否则本地重绘就行
        protected virtual RectangleF dirtyRect
        {
            get{return new RectangleF(); }// rect
        }

        RectangleF? mOccupyRect = null;
        internal void invalidRect()
        {
            mOccupyRect = null;
        }
        
        public void setDirty(bool redrawImmediatly = false)
        {
#if DIRTYRECTOPTIMAS
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
#else
            UIRoot.Instance.dirtyRoot = UIRoot.Instance.root;
#endif
            if (redrawImmediatly)
            {
                UIRoot.Instance.dirtyRedraw();
            }
        }

        //向上找到
        internal UIWidget getDirtyRoot()
        {
            #if DIRTYRECTOPTIMAS
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
#else
            return null;
#endif
        }

        protected RectangleF occupyRect
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

        public float borderX
        {
            get { return drawRect.Width; }
        }

        public float borderY
        {
            get { return drawRect.Height; }
        }

        //public virtual RectangleF pickRect
        //{
        //    get
        //    {
        //        return drawRect;
        //    }
        //}
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

        string mDeubgName;
        public string debugName
        {
            get { return mDeubgName; }
            set { mDeubgName = value; }
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
        public PointF center
        {
            get
            {
                return transform(drawRect.center());
            }

            set
            {
                var pt = center;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF leftTop
        {
            get
            {
                return transform(drawRect.leftTop());
            }
            set
            {
                var pt = leftTop;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF leftMiddle
        {
            get
            {
                return transform(drawRect.leftMiddle());
            }
            set
            {
                var pt = leftMiddle;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF leftBottom
        {
            get
            {
                return transform(drawRect.leftBottom());
            }
            set
            {
                var pt = leftBottom;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF rightTop
        {
            get
            {
                return transform(drawRect.rightTop());
            }
            set
            {
                var pt = rightTop;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF rightMiddle
        {
            get
            {
                return transform(drawRect.rightMiddle());
            }
            set
            {
                var pt = rightMiddle;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF rightBottom
        {
            get
            {
                return transform(drawRect.rightBottom());
            }
            set
            {
                var pt = rightBottom;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF middleTop
        {
            get
            {
                return transform(drawRect.middleTop());
            }
            set
            {
                var pt = middleTop;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public PointF middleBottom
        {
            get
            {
                return transform(drawRect.middleBottom());
            }
            set
            {
                var pt = middleBottom;
                float dx = value.X - pt.X;
                float dy = value.Y - pt.Y;
                px += dx;
                py += dy;
            }
        }

        public enum EAlign
        {
            noAlign,
            center,
            leftTop,
            left,
            leftBottom,
            rightTop,
            right,
            rightBottom,
            top,
            bottom,
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

            PointF alignPos = new PointF();
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
                case EAlign.left:
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
                case EAlign.right:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightMiddle);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    break;
                case EAlign.rightBottom:
                    alignPos = paresent.invertTransform((paresent as UIWidget).rightBottom);
                    alignPos.X += offsetx - pui.paddingX - marginX;
                    alignPos.Y += offsety - pui.paddingY - marginY;
                    break;
                case EAlign.top:
                    alignPos = paresent.invertTransform((paresent as UIWidget).middleTop);
                    alignPos.Y += offsety + pui.paddingY - marginY;
                    break;
                case EAlign.bottom:
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
                case EAlign.left:
                    this.leftMiddle = alignPos;
                    break;
                case EAlign.leftBottom:
                    this.leftBottom = alignPos;
                    break;
                case EAlign.rightTop:
                    this.rightTop = alignPos;
                    break;
                case EAlign.right:
                    this.rightMiddle = alignPos;
                    break;
                case EAlign.rightBottom:
                    this.rightBottom = alignPos;
                    break;
                case EAlign.top:
                    this.middleTop = alignPos;
                    break;
                case EAlign.bottom:
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
                    px = p.paddingX + marginX;
                    width = p.width - (p.paddingX + marginX) * 2;
                }
            }

            if (this.expandAbleY)
            {
                if (p != null)
                {
                    py = p.paddingY + marginY;
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
        public ELayout layout = ELayout.none;
        public bool layoutInverse = false;

        public bool wrap = false;
        public bool layoutFilled = false;//最后一个layout的，填满
        //这个因与渲染的遍历次序不同,因此不能放到draw里
        public virtual void adjustLayout()
        {
            if (!visible) return;

            mOccupyRect = null;//重新layout, 当然要重计 mOccupyRect

            adjustExpand();
            bool bAjust = adjustAlign();
            for (int i = 0; i < mChildrent.Count; ++i)
            {
                var c = mChildrent[i] as UIWidget;
                c.adjustLayout();
            }
            
            List<UIWidget> noInLayout = new List<UIWidget>();//for if layout and child align coexist

            #region layout calc
            PointF rb = new PointF();
            RectangleF rc = new RectangleF(new PointF(paddingX, paddingY), new Size(0, 0));
            rb = rc.rightBottom();
            int idxCount = 0;

            UIWidget lastLayoutUi = null;
            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                var c = mChildrent[i] as UIWidget;
                if (!c.visible) continue;

                if (c.align != EAlign.noAlign)
                {
                    noInLayout.Add(c);
                    continue;
                }
                
                if (layout == ELayout.vertical)
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
                    //计算新的对齐位置
                    rc.Width = max(c.marginX*2+c.width, rc.Width);
                    rc.Height = rc.Height + c.marginY* 2 + c.height;
                    rb.X = max(rc.Right, rb.X);
                    rb.Y = max(rc.Bottom, rb.Y);
                    if (layoutInverse)
                    {
                        var ptInv = pt;
                        ptInv.Y = ( (drawRect.X*2+height)-pt.Y);
                        c.leftBottom = ptInv;
                        c.translate(c.offsetx, c.offsety);//note:考虑offset
                    }
                    else
                    {
                        c.leftTop = pt;
                        c.translate(c.offsetx, c.offsety);//note:考虑offset
                    }


                    idxCount++;
                    if ( this.wrap && rc.Bottom > this.drawRect.Height - paddingY && idxCount > 1)//只能容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new PointF(rc.Location.X + rc.Width, paddingY);
                        rc.Size = new Size();
                    }
                }
                else if (layout == ELayout.horizon)
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

                    if (layoutInverse)
                    {
                        var ptInv = pt;
                        ptInv.X = ((drawRect.X * 2 + width) - pt.X);
                        c.rightTop = ptInv;
                        c.translate(c.offsetx, c.offsety);//note:考虑offset
                    }
                    else
                    {
                        c.leftTop = pt;
                        c.translate(c.offsetx, c.offsety);//note:考虑offset
                    }

                    idxCount++;
                    if ( this.wrap && rc.Right > this.drawRect.Width - paddingX && idxCount > 1)//至少容一个的情况
                    {
                        i++;
                        idxCount = 0;
                        rc = rcOld;
                        rc.Location = new PointF(paddingX, rc.Location.Y + rc.Height);
                        rc.Size = new Size();
                    }
                }
            }
            if (!this.wrap && this.shrinkAble && mChildrent.Count != 0) //有wrap不可shrink， shrink覆盖expand, shrink后需要重新计算 expand和align
            {
                var childrentRect = getChildrenOccupy();

                for (int i = mChildrent.Count - 1; i >= 0; --i)
                {
                    var c = mChildrent[i] as UIWidget;
                    if (c.align != EAlign.noAlign) continue;
                    c.px = c.px - childrentRect.Value.Left;
                    c.py = c.py - childrentRect.Value.Top;
                }
                if (childrentRect.HasValue)
                {
                    width = childrentRect.Value.Right - childrentRect.Value.Left;
                    height = childrentRect.Value.Bottom - childrentRect.Value.Top;
                }
                adjustAlign();
            }
            else if(layoutFilled && lastLayoutUi != null) //layout的最后一个填满
            {
                if (layout == ELayout.horizon)
                {
                    lastLayoutUi.height = height - lastLayoutUi.py - paddingY - lastLayoutUi.marginY;

                    if (layoutInverse)
                    {
                        lastLayoutUi.width = lastLayoutUi.px + lastLayoutUi.width - paddingX - lastLayoutUi.marginX;
                        lastLayoutUi.px = paddingX + lastLayoutUi.marginX;
                    }
                    else
                    {
                        lastLayoutUi.width = width - lastLayoutUi.px - paddingX - lastLayoutUi.marginX;
                    }
                }
                else if (layout == ELayout.vertical)
                {
                    lastLayoutUi.width = width - lastLayoutUi.px - paddingX - lastLayoutUi.marginX;

                    if (layoutInverse)
                    {
                        lastLayoutUi.height = lastLayoutUi.py + lastLayoutUi.height - paddingY - lastLayoutUi.marginY;
                        lastLayoutUi.py = paddingY + lastLayoutUi.marginY;
                    }
                    else
                    {
                        lastLayoutUi.height = height - lastLayoutUi.py - paddingY - lastLayoutUi.marginY;
                    }
                }
                //size 调整后应重新adjustLayout
                lastLayoutUi.adjustLayout();
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

        internal RectangleF? getChildrenOccupy()
        {
            RectangleF? ret = null;
            for (int i = mChildrent.Count - 1; i >= 0; --i)
            {
                var c = mChildrent[i] as UIWidget;
                if (c.align != EAlign.noAlign) continue;
                var lrc = c.layoutRect.transform(c.getLocalMatrix());
                if (!ret.HasValue)
                {
                    var left = lrc.Left - paddingX;
                    var top = lrc.Top - paddingY;
                    var width = lrc.Width + 2*paddingX;
                    var height = lrc.Height + 2*paddingY;
                    ret = new RectangleF(left, top, width, height);
                }
                else
                {
                    var left = min((lrc.Left - paddingX), ret.Value.Left);;
                    var top = min((lrc.Top - paddingY), ret.Value.Top);
                    var right = max((lrc.Right + paddingX), ret.Value.Right);;
                    var bottom = max((lrc.Bottom + paddingY), ret.Value.Bottom);;
                    ret = new RectangleF(left, top, right-left, bottom-top);
                }
            }
            return ret;
        }

        #endregion

        #endregion

        #region transform
        public Matrix transformMatrix
        {
            get
            {
                Matrix m = new Matrix();
                m.Translate(px, py);
                m.Rotate(mDir);
                m.Scale(mScalex, mScaley);
                return m;
            }
        }
        
        public void translate(int dx, int dy)
        {
            px += dx;
            py += dy;
        }
        #endregion

        #region methods
        public UIWidget(){}

        public void attach(UIWidget ui)
        {
            if (ui == null) return;
            ui.paresent = this;
        }

        public void dettach(UIWidget ui)
        {
            if (ui == null) return;
            if (ui.paresent != this) return;
            ui.paresent = null;
        }
        #endregion

        #region events

        public Action<UIWidget, UIWidget> evtChangeParesent;

        
        //public delegate bool EvtMouse(UIWidget _this, int x, int y);
        //public delegate bool EvtKeyboard(UIWidget _this, int kc, bool isControl, bool isShift);
        //public delegate void EvtSizeChanged(UIWidget _this, int w, int h);


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
        public event Action evtOnEnter;
        public void evtOnEnterClear()
        {
            evtOnEnter = null;
        }
        internal void doEnter()
        {
            if (evtOnEnter != null)
                evtOnEnter();
        }

        public event Action evtOnExit;
        public void evtOnExitClear()
        {
            evtOnExit = null;
        }
        internal void doExit()
        {
            if (evtOnExit != null)
                evtOnExit();
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

        public event Func<UIWidget, int, bool> evtOnWheel;
        public void evtOnWheelClear()
        {
            evtOnWheel = null;
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

        internal bool doEvtOnChar(int kc, bool isControl, bool isShift)
        {
            if (evtOnChar == null)
            {
                return true;
            }
            return evtOnChar(this, kc, isControl, isShift);
        }

        internal bool doEvtOnWheel(int delta)
        {
            if (evtOnWheel == null)
            {
                return true;
            }
            return evtOnWheel(this, delta);
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
        /// 
        public event Action<int, int> evtOnDragMove;
        public void evtOnDragMoveClear()
        {
            evtOnDragMove = null;
        }
        public event Action<int, int> evtOnDragEnd;
        public void evtOnDragEndClear()
        {
            evtOnDragEnd = null;
        }

        bool mDragAble = false;
        public bool dragAble
        {
            set
            {
                if (mDragAble == value) return;
                mDragAble = value;
                if (mDragAble)
                {
                    onDragEnd(0, 0);
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
            updateFixPoint(x, y);
            if (evtOnDragMove != null) evtOnDragMove(x, y);
            setDirty(true);
            return;
        }

        bool onDragBegin(UIWidget _this, int x, int y)
        {
            beginFixPoint((float)x, (float)y);
            //这个改变先后关系
            this.setDepthHead();

            UIRoot.Instance.evtMove += onDragMove;
            UIRoot.Instance.evtLeftUp += onDragEnd;
            setDirty(true);
            return false;
        }

        void onDragEnd(int x, int y)
        {
            UIRoot.Instance.evtMove -= onDragMove;
            UIRoot.Instance.evtLeftUp -= onDragEnd;
            if(evtOnDragEnd != null) evtOnDragEnd(x, y);
            setDirty(true);
        }


        bool mRotateAble = false;
        public bool rotateAble
        {
            set
            {
                if (mRotateAble == value) return;
                mRotateAble = value;
                if (mRotateAble)
                {
                    onRotateEnd(0, 0);
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
            setDirty(true);
            return false;
        }

        void onRotateMove(int delta)
        {
            beginFixPoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            mDir += delta * 0.2f;
            updateFixPoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            setDirty(true);
            return;
        }

        void onRotateEnd(int x, int y)
        {
            UIRoot.Instance.mEvtWheel -= onRotateMove;
            UIRoot.Instance.mEvtRightUp -= onRotateEnd;
            setDirty(true);
        }

        /// <summary>
        /// dragable end
        /// </summary>


        /// <summary>
        /// scaleAble begin
        /// </summary>
        bool mScaleAble = false;
        
        /// <summary>
        /// scaleAble end
        /// </summary>
        public bool scaleAble
        {
            set
            {
                if (mScaleAble == value) return;
                mScaleAble = value;
                if (mScaleAble)
                {
                    onScaleEnd(0, 0);
                    evtOnLMDown += onScaleBegin;
                }
                else
                {
                    evtOnLMDown -= onScaleBegin;
                    onScaleEnd(0, 0);
                }
            }

            get
            {
                return mScaleAble;
            }
        }

        bool onScaleBegin(UIWidget ui, int x, int y)
        {
            UIRoot.Instance.mEvtWheel += onScaleWheel;
            UIRoot.Instance.evtLeftUp += onScaleEnd;
            setDirty(true);
            return false;
        }

        void onScaleWheel(int delta)
        {
            beginFixPoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            float sc = 1;
            if (delta > 0) sc = 1.1f;
            else sc = 0.9f;
            mScalex += sc - 1;
            mScaley += sc - 1;
            if (mScalex == 0) mScalex += sc - 1;//to avoid zero, 
            if (mScaley == 0) mScaley += sc - 1;
            updateFixPoint(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
            setDirty(true);
            //return false;
        }

        void onScaleEnd(int x, int y)
        {
            UIRoot.Instance.mEvtWheel -= onScaleWheel;
            UIRoot.Instance.evtLeftUp -= onScaleEnd;
            setDirty(true);
        }

        //utils
        protected static float min(float a, float b) { if (a < b)return a; else return b; }
        protected static float max(float a, float b) { if (a > b)return a; else return b; }
        protected static RectangleF expandRect(RectangleF r1, RectangleF r2)
        {
            float left = min(r1.Left, r2.Left);
            float top = min(r1.Top, r2.Top);
            float right = max(r1.Right, r2.Right);
            float buttom = max(r1.Bottom, r2.Bottom);

            return new RectangleF(left, top, right - left, buttom - top);
        }

        internal static string tryGetProp(string attName, XmlNode node)
        {
            string strRet = null;
            var ret = node.Attributes.GetNamedItem(attName);
            if (ret == null)
            {
                strRet = UIRoot.Instance.getPropertyInTable(attName);
            }
            else
            {
                strRet = ret.Value;
            }
            UIRoot.Instance.setPropertyInTable(attName, ref strRet);
            return strRet;
        }

        internal static T getProp<T>(XmlNode node, string attName, T defaultVal, out bool valid) where T : IConvertible
        {
            valid = true;
            string strRet = tryGetProp(attName, node);
            
            if (strRet != null)
            {
                if (typeof(T) == typeof(int))
                {
                    int di = (int)Convert.ChangeType(defaultVal, typeof(int)); //strRet.castInt();
                    return (T)Convert.ChangeType(strRet.castInt(di), typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(uint))
                {
                    uint dui = (uint)Convert.ChangeType(defaultVal, typeof(uint)); //strRet.castInt();
                    return (T)Convert.ChangeType(strRet.castHex(dui), typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool db = (bool)Convert.ChangeType(defaultVal, typeof(bool)); //strRet.castInt();
                    return (T)Convert.ChangeType(strRet.castBool(db), typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(float))
                {
                    float df = (float)Convert.ChangeType(defaultVal, typeof(float)); //strRet.castInt();
                    return (T)Convert.ChangeType(strRet.castFloat(df), typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(strRet, typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(EColorUtil))
                {
                    EColorUtil col;
                    bool br = Enum.TryParse<EColorUtil>(strRet, out col);
                    if(br)
                        return (T)Convert.ChangeType(col, typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(EAlign))
                {
                    EAlign align;
                    var br = Enum.TryParse(strRet, true, out align); //(typeof(EAlign), strRet);
                    if(br)
                        return (T)Convert.ChangeType(align, typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(ELayout))
                {
                    ELayout layout;
                    var br = Enum.TryParse(strRet, true, out layout); //(typeof(EAlign), strRet);
                    if(br)
                        return (T)Convert.ChangeType(layout, typeof(T)); //strRet.castInt();
                }
                else if(typeof(T) == typeof(EStyle))
                {
                    EStyle style;
                    var br = Enum.TryParse(strRet, true, out style); //(typeof(EAlign), strRet);
                    if (br)
                        return (T)Convert.ChangeType(style, typeof(T)); //strRet.castInt();
                }
                else if (typeof(T) == typeof(EForward))
                {
                    EForward forward;
                    var br = Enum.TryParse(strRet, true, out forward); //(typeof(EAlign), strRet);
                    if (br)
                        return (T)Convert.ChangeType(forward, typeof(T)); //strRet.castInt();
                }
                
            }
            valid = false;
            return defaultVal; 
        }

        void parseLayout(XmlNode node)
        {
            var ret = node.Attributes.GetNamedItem("layout");
            if (ret == null) return;
            string str = ret.Value;
            var propList = str.Split(',');
            var newProps = new List<string>();
            for (int i = 0; i < propList.Count(); ++i)
            {
                var item = propList[i];
                newProps.Add(item.Trim());
            }

            //vertical/horizon
            if (newProps.Contains("vertical") )
            {
                layout = ELayout.vertical;
            }
            else if (newProps.Contains("horizon") )
            {
                layout = ELayout.horizon;
            }
            else
            {
                layout = ELayout.none;
            }

            //inverse
            if (newProps.Contains("inverse") )
            {
                layoutInverse = true;
            }
            else
            {
                layoutInverse = false;
            }

            if (newProps.Contains("wrap"))
            {
                wrap = true;
            }
            else
            {
                wrap = false;
            }

            if (newProps.Contains("filled"))
            {
                layoutFilled = true;
            }
            else
            {
                wrap = false;
            }

            if (newProps.Contains("expand"))
            {
                expandAbleY = expandAbleX = true;
            }
            else
            {
                if (newProps.Contains("expandX"))
                {
                    expandAbleX = true;
                }
                else
                {
                    expandAbleX = false;
                }

                if(newProps.Contains("expandY"))
                {
                    expandAbleY = true;
                }
                else
                {
                    expandAbleY = false;
                }

                if (newProps.Contains("shrink"))
                {
                    shrinkAble = true;
                }
                else
                {
                    shrinkAble = false;
                }
            }

        }

        public XmlNodeList fromXML(XmlNode node)
        {
            //在UI*.fromXML之前调用?
            string strRet = null;
            bool br = true;
            UIRoot.Instance.setPropertyderived(true);
            var ret = node.Attributes.GetNamedItem("derived");
            if (ret != null)
            {
                bool v = ret.Value.castBool(false);
                UIRoot.Instance.setPropertyderived(v);
            }

            ret = node.Attributes.GetNamedItem("name");
            if (ret != null) name = ret.Value;
            ret = node.Attributes.GetNamedItem("debugName");
            if (ret != null) debugName = ret.Value;
            ret = node.Attributes.GetNamedItem("id");
            if (ret != null)
            {
                UIRoot.Instance.setIDWidget(ret.Value, this);
                StringID = ret.Value;
            }

            ret = node.Attributes.GetNamedItem("location");
            if (ret != null)
            {
                var strs = ret.Value.Split(',').ToList();
                var strs1 = new List<string>();
                strs.ForEach(str => strs1.Add(str.Trim()));
                int c = strs1.Count();
                if(c>0)
                {
                    py = px = strs1[0].castFloat();
                }
                if (c > 1)
                {
                    py = strs1[1].castFloat();
                }
            }

            ret = node.Attributes.GetNamedItem("size");
            if (ret != null)
            {
                var strs = ret.Value.Split(',').ToList();
                var strs1 = new List<string>();
                strs.ForEach(str => strs1.Add(str.Trim()));
                int c = strs1.Count();
                if (c > 0)
                {
                    height = width = strs1[0].castFloat();
                }
                if (c > 1)
                {
                    height = strs1[1].castFloat();
                }
            }
            width = getProp(node, "length", width, out br);
            if (br)
                height = width;
            else
            {
                width = getProp(node, "width", width, out br);
                height = getProp(node, "height", height, out br);
            }

            ret = node.Attributes.GetNamedItem("editMode");
            if (ret != null)
            {
                var strs = ret.Value.Split(',').ToList();
                var strs1 = new List<string>();
                strs.ForEach(str => strs1.Add(str.Trim()) );

                if (strs1.Contains("dragAble"))
                {
                    dragAble = true;
                }
                if (strs1.Contains("rotateAble"))
                {
                    rotateAble = true;
                }
                if (strs1.Contains("scaleAble"))
                {
                    scaleAble = true;
                }
            }

            px = getProp(node, "px", px, out br);
            py = getProp(node, "py", py, out br);
            mDir = getProp(node, "dir", mDir, out br);

            mScaley = mScalex = getProp(node, "scale", mScalex, out br);
            if (!br)
            {
                mScalex = getProp(node, "scaleX", mScalex, out br);
                mScaley = getProp(node, "scaleY", mScaley, out br);
            }

            alignParesent = mAlign = getProp(node, "align", mAlign, out br);
            var alignRet = getProp(node, "alignParesent", alignParesent, out br);
            if (br)
            {
                alignParesent = alignRet;
            }

            mOffsety = mOffsetx = getProp(node, "offset", mOffsetx, out br);
            if (!br)
            {
                mOffsetx = getProp(node, "offsetX", mOffsetx, out br);
                mOffsety = getProp(node, "offsetY", mOffsety, out br);
            }

            clip = getProp(node, "clip", clip, out br);
            enable = getProp(node, "enable", enable, out br);
            visible = getProp(node, "visible", visible, out br);

            dragAble = getProp(node, "dragAble", dragAble, out br);
            scaleAble = getProp(node, "scaleAble", scaleAble, out br);
            rotateAble = getProp(node, "rotateAble", rotateAble, out br);


            parseLayout(node);
            //layout = getAttr(node, "layout", layout, out br);
            layoutInverse = getProp(node, "layoutInverse", layoutInverse, out br);
            wrap = getProp(node, "wrap", wrap, out br);
            layoutFilled = getProp(node, "layoutFilled", layoutFilled, out br);
            shrinkAble = getProp(node, "shrink", shrinkAble, out br);
            expandAbleX = getProp(node, "expand", expandAbleX, out br);
            if (br)
                expandAbleY = expandAbleX;
            else
            {
                expandAbleX = getProp(node, "expandX", expandAbleX, out br);
                expandAbleY = getProp(node, "expandY", expandAbleY, out br);
            }

            marginX = getProp(node, "margin", marginX, out br);
            if (br)
                marginY = marginX;
            else
            {
                marginX = getProp(node, "marginX", marginX, out br);
                marginY = getProp(node, "marginY", marginY, out br);
            }

            paddingX = getProp(node, "padding", paddingX, out br);
            if (br)
                paddingY = paddingX;
            else
            {
                paddingX = getProp(node, "paddingX", paddingX, out br);
                paddingY = getProp(node, "paddingY", paddingY, out br);
            }

            return node.ChildNodes;
        }

        public UIWidget appendFromXML(string xml)
        {
            var ui = UIRoot.Instance.loadFromXML(xml);
            if (ui != null)
                ui.paresent = this;
            return ui;
        }

        #endregion

        #region others

        public virtual bool testSelfPick(PointF pos)
        {
            if (testRectPick)
                return true;
            else
                return false;//除非被改写
        }


        public virtual bool testRectPick
        {
            get
            {
                return true;
            }
        }

        public bool doTestPick(PointF pos, out UIWidget ui, bool testEnable = true)
        {
            if (testEnable && !enable)
            {
                ui = null;
                return false;
            }

            var t = transformMatrix.Clone();
            t.Invert();
            var ps = new PointF[] { pos };
            t.TransformPoints(ps);
            var newpos = ps[0];

            if (testRectPick)
            {
                var r = drawRect;
                if (!r.Contains(newpos))
                {
                    ui = null;
                    return false;
                }
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

            bool ret = testSelfPick(newpos);
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

            //Console.WriteLine("draw" + this.name + ":");
            //catch it?
            if (!onDraw(g))
            {
                g.Restore(gs);
                return;
            }

            if (evtOnPostDraw != null) evtOnPostDraw(g);

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

        public event Action<Graphics> evtOnPostDraw;
        public void evtOnPostDrawClear() 
        {
            evtOnPostDraw = null;
        }

        public virtual bool onDraw(Graphics g) { return true; }
        #endregion
    }
}
