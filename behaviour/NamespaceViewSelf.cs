using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class NamespaceViewSelf : ViewWindowTemplate
    {
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
                if (mSelectItem is iViewInstance)
                {
                    (mSelectItem as iViewInstance).unselect();
                }
                mSelectItem = value;
                if (mSelectItem is iViewInstance)
                {
                    (mSelectItem as iViewInstance).select();
                }
            }
        }


        public NamespaceViewSelf(MNamespace self)
        {
            mModel = self;
            if(mModel != null)
            {
                setType(mModel.type);
                setTitle(mModel.name);
                showContent();

                evtOnRMUp += onRUp;
            }
            evtClose += () =>
            {
                this.paresent = null;
            };
        }

        void showContent()
        {
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

                viewItem.evtOnLMDown += (ui, px, py) =>
                    {
                        selectItem = ui;
                        return false;
                    };

                mItems.Add(viewItem);
                addItem(viewItem, x, y);
                var pt = viewItem.rightButtom;
                if (pt.X > this.getSize().X)
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
                var lbTitleHead = (this.childOfPath("root/title_bar/title_head") as UILable);
                var lbTitleBG = (this.childOfPath("root/panel/title_bg") as UILable);
                lbTitleHead.text = text;
                lbTitleBG.text = text;
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
