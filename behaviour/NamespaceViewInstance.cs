using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    interface iVieweInstance
    {
        void select();
        void unselect();
    }

    class NamespaceViewInstance : UIRect, iVieweInstance
    {
        public UIRect mLayer1;
        public UIRect mLayer2;

        public UIRect mAccesory1;
        //public UILine mAccesory2;
        public UILable mType;
        public UILable mLable;
        public MNamespace mModel;

        const string mTag = "NS";

        const int mWidth = 96;
        const int mHeight = 64;

        const uint mStanderdColor = 0xffdeb887;
        const uint mSelectColor = 0xffadff2f;

        public NamespaceViewInstance(MNamespace self)
            : base(mWidth, mHeight, 0xffffffff, mStanderdColor)
        {
            mModel = self;

            mLayer1 = new UIRect(mWidth, mHeight, 0xffffffff, mStanderdColor);
            mLayer1.enable = false;
            mLayer1.paresent = this;
            mLayer1.mPos.Offset(-2, -2);

            mLayer2 = new UIRect(mWidth, mHeight, 0xffffffff, mStanderdColor);
            mLayer2.enable = false;
            mLayer2.paresent = mLayer1;
            mLayer2.mPos.Offset(-2, -2);

            mAccesory1 = new UIRect(18, 18);
            mAccesory1.enable = false;
            mAccesory1.paresent = this;

            //mAccesory2 = new UILine((int)(24 * 1.414f), 1);
            //mAccesory2.enable = false;
            //mAccesory2.paresent = this;
            //mAccesory2.mDir = -45;
            //mAccesory2.leftTop = mAccesory1.leftButtom;

            mType = new UILable(mTag, 8, 0xff00ff00);
            mType.enable = false;
            mType.paresent = this;
            mType.center = mAccesory1.center;

            mLable = new UILable(mModel.name, 12, 0xffffffff);
            mLable.enable = false;
            mLable.paresent = this;

            adjust();

            dragAble = true;

            evtOnLMDown += (ui, x, y) =>
                {
                    if (mParesent != null && mParesent is NamespaceViewSelf)
                    {
                        var p = mParesent as NamespaceViewSelf;
                        p.selectItem = this;
                    }
                    return false;
                };
            evtOnDClick += (ui, x, y) =>
                {
                    var v = mModel.viewSelf;
                    v.paresent = UIRoot.Instance.root;
                    return false;
                };
        }

        public void select()
        {
            this.fillColor = mSelectColor;
            mLayer1.fillColor = mSelectColor;
            mLayer2.fillColor = mSelectColor;
        }

        public void unselect()
        {
            this.fillColor = mStanderdColor;
            mLayer1.fillColor = mStanderdColor;
            mLayer2.fillColor = mStanderdColor;
        }

        void adjust()
        {
            mLable.center = invertTransform(this.center);
        }
    }

}
