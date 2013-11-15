using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class UIGSelfEnd : UIWidget
    {
        UIRect mRect;
        UIArrow mArrow;
        
        public override Rectangle rect
        {
            get
            {
                var rtl = new Point(mRect.rect.Top, mRect.rect.Left);
                rtl = mRect.transform(rtl);
                var rrb = new Point(mRect.rect.Right, mRect.rect.Bottom);
                rrb = mRect.transform(rrb);
                var atl = new Point(mArrow.rect.Top, mArrow.rect.Left);
                atl = mArrow.transform(atl);
                var arb = new Point(mArrow.rect.Right, mArrow.rect.Bottom);
                arb = mArrow.transform(arb);
                
                Rectangle rn = new Rectangle(rtl, new Size(rrb.X-rtl.X, rrb.Y-rtl.Y) );
                Rectangle an = new Rectangle(atl, new Size(arb.X-atl.X, arb.Y-atl.Y) );

                return expandRect(rn, an);
            }
        }

        public override string typeName
        {
            get { return "UIGSelfEnd"; }
        }

        public override bool testPick(Point pos)
        {   
            return mRect.testPick(mRect.transform(pos)) || mArrow.testPick(mArrow.transform(pos));
        }

        internal override void onDraw(Graphics g)
        {
        }

        protected UIGSelfEnd(EForward forward, int size = 20) 
        {
            int arrowLen = (int)(size * 1f);
            int rectLen = (int)(size * 1f);
            
            mRect = new UIRect(size, size);
            mArrow = new UIArrow(size, size, EForward.e_down);

            
            switch(forward)
            {
                case EForward.e_left:
                    {
                        mRect.width = rectLen;
                        mArrow.width = arrowLen;
                        mArrow.mPos.X -= arrowLen;
                        mArrow.forward = EForward.e_left;
                        break;
                    };
                case EForward.e_right:
                    {
                        mRect.width = rectLen;
                        mArrow.width = arrowLen;
                        mArrow.mPos.X += rectLen;
                        mArrow.forward = EForward.e_right;
                        break;
                    };
                case EForward.e_up:
                    {
                        mRect.height = rectLen;
                        mArrow.height = arrowLen;
                        mArrow.mPos.Y -= arrowLen;
                        mArrow.forward = EForward.e_up;
                        break;
                    };
                case EForward.e_down:
                    {
                        mRect.height = rectLen;
                        mArrow.height = arrowLen;
                        mArrow.mPos.Y += rectLen;
                        mArrow.forward = EForward.e_down;
                        break;
                    };
                default:
                    break;
            }

            
            mRect.paresent = this;
            mArrow.paresent = this;
            mRect.enable = false;
            mArrow.enable = false;
        }

        public static UIGSelfEnd create(EForward forward, int size = 20)
        {
            var ret = new UIGSelfEnd(forward, size);
            return ret;
        }

        //顶点位置
        public Point pivotPos
        {
            get
            {
                return transform(mArrow.pivotPos);
            }
        }

        //顶点位置
        public Point basePos
        {
            get
            {
                switch (mArrow.forward)
                {
                    case EForward.e_left:
                        {
                            var pt = new Point();
                            pt.Y += mRect.height / 2;
                            pt.X += mRect.width;
                            return transform(mRect.transform(pt));
                        };
                    case EForward.e_right:
                        {
                            var pt = new Point();
                            pt.Y += mRect.height / 2;
                            return transform(mRect.transform(pt));
                        };
                    case EForward.e_up:
                        {
                            var pt = new Point();
                            pt.Y = mRect.height;
                            pt.X = mRect.width / 2;
                            return transform(mRect.transform(pt));
                        };
                    case EForward.e_down:
                        {
                            var pt = new Point();
                            pt.X = mRect.width / 2;
                            return transform(mRect.transform(pt));
                        };
                    default:
                        break;
                }
                return new Point();
            }
            set
            {
                var pos = basePos;
                Point dpt = new Point(pos.X - value.X, pos.Y - value.Y);
                mPos.X -= dpt.X;
                mPos.Y -= dpt.Y;
            }
        }
    }

    class UIGSelfDataIn : UIGSelfEnd
    {
        public MetaType mType;
        public UIGSelfDataIn(MetaType type) : base(EForward.e_down)
        {
            mType = type;
        }
    }

    class UIGSelfDataOut : UIGSelfEnd
    {
        public MetaType mType;
        public UIGSelfDataOut(MetaType type) : base(EForward.e_up)
        {
            mType = type;
        }
    }

    class UIGSelfTriggerIn : UIGSelfEnd
    {
        public UIGSelfTriggerIn()
            : base(EForward.e_right)
        {   
        }
    }

    class UIGSelfTriggerOut : UIGSelfEnd
    {
        public UIGSelfTriggerOut()
            : base(EForward.e_left)
        {
        }
    }

    class UIGDataIn : UIGSelfEnd
    {
        public MetaType mType;
        public UIGDataIn(MetaType type)
            : base(EForward.e_down, 8)
        {
            mType = type;
        }
    }

    class UIGDataOut : UIGSelfEnd
    {
        public MetaType mType;
        public UIGDataOut(MetaType type)
            : base(EForward.e_down, 8)
        {
            mType = type;
        }
    }

    class UIGTriggerIn : UIGSelfEnd
    {
        public MetaType mType;
        public UIGTriggerIn()
            : base(EForward.e_right, 8)
        {
        }
    }

    class UIGTriggerOut : UIGSelfEnd
    {
        public MetaType mType;
        public UIGTriggerOut()
            : base(EForward.e_right, 8)
        {
        }
    }

    class UIGBehviour : UIRect
    {
        public UILable mName;
        public UIGBehviour():base(128, 64)
        {
            mName = new UILable("Template");
            mName.paresent = this;
        }
    }

    class UIEnviroment : UIWidget
    {
        
        public string name
        {
            get;
            set;
        }

        public void addDataIn(MetaType type)
        {
        }

        public void addDataOut(MetaType type)
        {
        }

        public void addTriggerIn()
        {
        }

        public void addTriggerOut()
        {
        }
    }
}
