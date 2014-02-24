using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;


namespace ns_behaviour
{
    class UIGSelfDataIn : UIGSelfEnd
    {
        public BehaviourViewSelf mContainer;
        int mIndex;

        public MetaType mType
        {
            get
            {
                return mContainer.mModel.mDataIn[mIndex];
            }
        }

        public UIGSelfDataIn(BehaviourViewSelf container, int idx)
            : base(EForward.e_down)
        {
            mContainer = container;
            mIndex = idx;
        }
    }

    class UIGSelfDataOut : UIGSelfEnd
    {
        public BehaviourViewSelf mContainer;
        int mIndex;

        public MetaType mType
        {
            get
            {
                return mContainer.mModel.mDataOut[mIndex];
            }
        }

        public UIGSelfDataOut(BehaviourViewSelf container, int idx)
            : base(EForward.e_down)
        {
            mContainer = container;
            mIndex = idx;
        }
    }

    class UIGSelfTriggerIn : UIGSelfEnd
    {
        int mIndex;
        public UIGSelfTriggerIn(BehaviourViewSelf container, int idx)
            : base(EForward.e_right)
        {
            mIndex = idx;
        }
    }

    class UIGSelfTriggerOut : UIGSelfEnd
    {
        BehaviourViewSelf mContainer;
        int mIndex;
        public UIGSelfTriggerOut(BehaviourViewSelf container, int idx)
            : base(EForward.e_right)
        {
            mIndex = idx;
        }
    }

    class BehaviourViewSelf : ViewWindowTemplate, iViewerSelf
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

        Dictionary<string, object> mAttrs = new Dictionary<string, object>();
        public Dictionary<string, object> attr
        {
            get
            {
                return mAttrs;
            }
        }
        #endregion

        public BehaviourModel mModel;
        List<UIGSelfDataOut> mDataOut = new List<UIGSelfDataOut>();
        List<UIGSelfDataIn> mDataIn = new List<UIGSelfDataIn>();
        List<UIGSelfTriggerOut> mTriggerOut = new List<UIGSelfTriggerOut>();
        List<UIGSelfTriggerIn> mTriggerIn = new List<UIGSelfTriggerIn>();

        public BehaviourViewSelf(BehaviourModel model)
        {
            mModel = model;
            this.setTitle(mModel.name);
            this.setType(mModel.type);
            showContent();
            //evtClose += () =>
            //    {
            //        paresent = null;
            //    };
            evtOnChar += (ui, kc, isC, isS) =>
            {
                if (kc == (int)System.Windows.Forms.Keys.V && isC)
                {
                    var m = GlobalModel.mModelCopy;
                    if (m != null)
                    {
                        if (m is iModelBeh)
                        {
                            var mn = (m as iModelBeh);
                            var vitem = mn.getViewerBehItem();
                            var uiitem = vitem.asWidget();

                            var pt = Globals.Instance.mPainter.getMousePosition();
                            pt = getClient().invertTransformAbs(pt);

                            this.addItem(uiitem, pt.X, pt.Y);//todo
                        }
                    }
                }
                return false;
            };
            this.rotateAble = true;
        }

        public void showContent()
        {
            var w = this.getClient().drawRect.Width;
            var left = this.getClient().position.X;
            var right = left + w;
            var dataInWidth = w / (mModel.mDataIn.Count() + 1);
            var dataOutWidth = w / (mModel.mDataOut.Count() + 1);

            var h = this.getClient().drawRect.Height;
            var up = this.getClient().position.Y;
            var down = up + h;
            var triggerInHeight = h / (mModel.triggerInNumber + 1);
            var triggerOutHeight = h / (mModel.triggerOutNumber + 1);

            for (int i = 0; i < mModel.mDataIn.Count(); ++i)
            {
                var ui = new UIGSelfDataIn(this, i);
                ui.dragAble = true;
                //ui.setFillColor(mColorInOut);
                ui.paresent = getClient();
                ui.attrs["type"] = mModel.mDataIn[i];
                ui.middleTop = new Point(dataInWidth + dataInWidth * i, up);
                mDataIn.Add(ui);
            }

            for (int i = 0; i < mModel.mDataOut.Count(); ++i)
            {
                var ui = new UIGSelfDataOut(this, i);
                ui.dragAble = true;
                //ui.setFillColor(mColorInOut);
                ui.paresent = getClient();
                ui.attrs["type"] = mModel.mDataOut[i];
                ui.attrs["index"] = i;
                ui.middleButtom = new Point(dataOutWidth + dataOutWidth * i, down);
                mDataOut.Add(ui);
            }

            for (int i = 0; i < mModel.triggerInNumber; ++i)
            {
                var ui = new UIGSelfTriggerIn(this, i);
                ui.dragAble = true;
                //ui.setFillColor(mColorInOut);
                ui.paresent = getClient();
                ui.attrs["index"] = i;
                ui.leftMiddle = new Point(left, triggerInHeight + triggerInHeight * i);
                mTriggerIn.Add(ui);
            }

            for (int i = 0; i < mModel.triggerOutNumber; ++i)
            {
                var ui = new UIGSelfTriggerOut(this, i);
                ui.dragAble = true;
                //ui.setFillColor(mColorInOut);
                ui.paresent = getClient();
                ui.attrs["index"] = i;
                ui.rightMiddle = new Point(right, triggerInHeight + triggerInHeight * i);
                mTriggerOut.Add(ui);
            }
        }
    }
}
