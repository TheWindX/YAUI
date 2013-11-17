using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class NamespaceViewSelf : UIRect
    {
        public UILable mLableMidIn;
        public UILable mLableTitle;

        public UIRect mTitleBar;

        public UIRect mAccesory1;
        public UILable mType;
        const string mTag = "NS";

        public MNamespace mModel;

        public List<UIWidget> mItems = new List<UIWidget>();

        UIWidget mSelectItem;

        const int mWidth = 512;
        const int mHeight = 512;
        const int mTitleHeight = 32;


        public UIWidget selectItem
        {
            get
            {
                return mSelectItem;
            }

            set
            {
                if (mSelectItem is iVieweInstance)
                {
                    (mSelectItem as iVieweInstance).unselect();
                }
                mSelectItem = value;
                if (mSelectItem is iVieweInstance)
                {
                    (mSelectItem as iVieweInstance).select();
                }
            }
        }


        public NamespaceViewSelf(MNamespace self)
            : base(mWidth, mHeight, 0xffffffff, 0xffaaaaaa)
        {
            mModel = self;

            mTitleBar = new UIRect(mWidth, mTitleHeight, 0x0, 0xFF0072E3);
            mTitleBar.paresent = this;

            mLableTitle = new UILable(mModel.name, 12, 0xffffffff);
            mLableTitle.paresent = mTitleBar;
            mLableTitle.leftMiddle = invertTransform(mTitleBar.leftMiddle);
            mLableTitle.translate(32, 0);

            mAccesory1 = new UIRect(18, 18);
            mAccesory1.enable = false;
            mAccesory1.paresent = mTitleBar;
            mAccesory1.leftMiddle = invertTransform(mTitleBar.leftMiddle);
            mAccesory1.translate(4, 0);

            mType = new UILable(mTag, 8, 0xff00ff00);
            mType.enable = false;
            mType.paresent = mTitleBar;
            mType.leftMiddle = mAccesory1.leftMiddle;

            mLableMidIn = new UILable(mModel.name, 12, 0xff999999);
            mLableMidIn.enable = false;
            mLableMidIn.paresent = this;
            mLableMidIn.mScalex = 8;
            mLableMidIn.mScaley = 8;
            mLableMidIn.center = invertTransform(this.center);

            showContent();

            mTitleBar.setDepthHead();

            evtOnMMUp += onMUp;
            evtOnRMUp += onRUp;

            dragAble = true;
            clip = true;
        }

        void showContent()
        {
            for (int i = 0; i < mItems.Count; ++i)
            {
                mItems[i].paresent = null;
            }
            mItems.Clear();

            int dx = 12;
            int dy = mTitleHeight+12;
            int x = dx;
            int y = dy;
            int yNext = y;


            foreach (var elem in mModel.enumChildrent())
            {
                var viewItem = elem.viewInstance;
                viewItem.paresent = this;
                viewItem.leftTop = new Point(x, y);
                var pt = viewItem.rightButtom;
                if (pt.X > rect.Width)
                {
                    x = dx;
                    y = yNext;
                    viewItem.leftTop = new Point(x, y);
                }

                var ptrb = viewItem.rightButtom;
                x = ptrb.X + dx;
                yNext = ptrb.Y + dy;
            }
        }

        Action<string> mMUpAction = null;
        public bool onMUp(UIWidget _this, int x, int y)
        {
            var edit = InputForm.Instance;
            edit.show(true, x, y);
            edit.evtInputExit = (text) =>
            {
                mModel.name = text;
                mLableMidIn.text = text;
                showContent();
            };
            return false;
        }

        public bool onRUp(UIWidget _this, int x, int y)
        {
            var edit = InputForm.Instance;
            edit.tintText = "set name for new namespace";
            edit.show(true, x, y);
            edit.evtInputExit = (text) =>
            {
                mModel.addNameSpace(text);
                mModel.reflushViewSelf();
            };

            return false;
        }
    }
}
