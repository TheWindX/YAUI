using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class NamespaceViewSelf : ViewWindowTemplate, iViewerSelf
    {

        #region interface
        public iModel getModel()
        {
            return mModel;
        }

        public UIWidget asWidget()
        {
            return this;
        }
        #endregion
        public MNamespace mModel;
        public List<iViewerNameItem> mItems = new List<iViewerNameItem>();

        iViewerNameItem mSelectItem;

        public iViewerNameItem selectItem
        {
            get
            {
                return mSelectItem;
            }

            set
            {
                if(mSelectItem != null)
                    mSelectItem.unselect();
                mSelectItem = value;
                if (mSelectItem != null)
                    mSelectItem.select();
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


            evtOnChar += (ui, kc, isC, isS) =>
            {
                if (kc == (int)System.Windows.Forms.Keys.V && isC)
                {
                    var m = GlobalModel.mModelCopy;
                    if (m != null)
                    {
                        if (m is iModelName)
                        {
                            var mn = (m as iModelName);
                            var vitem = mn.getViewerNameItem();
                            var uiitem = vitem.asWidget();
                            this.addItem(uiitem, 20, 20);//todo

                        }
                    }
                }
                return false;
            };
        }

        public void reflush()
        {
            showContent();
        }

        void showContent()
        {
            for (int i = 0; i < mItems.Count; ++i)
            {
                mItems[i].asWidget().paresent = null;
            }
            mItems.Clear();

            int dx = 12;
            int dy = 12;
            int x = dx;
            int y = dy;
            int yNext = y;

            foreach (var elem in mModel.enumChildrent())
            {
                var viewItem = elem.getViewerNameItem();

                viewItem.asWidget().evtOnLMDown += (ui, px, py) =>
                    {
                        var item = ui as iViewerNameItem;
                        if (item != null)
                            selectItem = item;
                        return false;
                    };

                mItems.Add(viewItem);
                addItem(viewItem.asWidget(), x, y);
                var pt = viewItem.asWidget().rightButtom;
                if (pt.X > this.getSize().X)
                {
                    x = dx;
                    y = yNext;
                    viewItem.asWidget().leftTop = new Point(x, y);
                }

                var ptrb = viewItem.asWidget().rightButtom;
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
                reflush();
            };

            return false;
        }
    }
}
