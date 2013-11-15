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
        public UILable mLable;
        public MNamespace mModel;

        public List<UIWidget> mItems = new List<UIWidget>();

        UIWidget mSelectItem;
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
            : base(500, 500)
        {
            mModel = self;
            mLable = new UILable(mModel.name, 12, 0xffaaaaaa);
            mLable.enable = false;
            mLable.paresent = this;
            mLable.mScalex = 4;
            mLable.mScaley = 4;
            adjust();

            evtOnMMUp += onMUp;
            evtOnRMUp += onRUp;

            clip = true;
        }

        void adjust()
        {
            mLable.leftTop = invertTransform(leftTop);
            mLable.mPos.X += 2;
            mLable.mPos.Y += 2;

            for (int i = 0; i < mItems.Count; ++i)
            {
                mItems[i].paresent = null;
            }
            mItems.Clear();

            int dx = 12;
            int dy = 12;
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
                mLable.text = text;
                adjust();
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
