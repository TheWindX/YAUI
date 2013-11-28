using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class BehaviourViewInstance : UIStub, iViewerBehItem, iViewerNameItem
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

        public void select()
        {
            mMainIns.setColor(mColorMainSelect);
            //  setColor(mSelectColor);
        }

        public void unselect()
        {
            mMainIns.setColor(mColorMainUnSelect);
        }
        #endregion

        ViewItemTemplate mMainIns;

        List<UIArrow> mDataIn = new List<UIArrow>();
        List<UIArrow> mDataOut = new List<UIArrow>();
        List<UIArrow> mTriggerIn = new List<UIArrow>();
        List<UIArrow> mTriggerOut = new List<UIArrow>();

        BehaviourModel mModel;
        public BehaviourViewInstance(BehaviourModel self)
        {
            mModel = self;
            reflush();
            this.dragAble = true;

            evtOnDClick += (ui, x, y) =>
            {
                var v = mModel.getViewerSelf();
                v.asWidget().position = v.asWidget().invertTransformParesentAbs(new Point(x, y));
                v.asWidget().paresent = UIRoot.Instance.root;
                return false;
            };

            evtOnChar += (ui, kc, isC, isS) =>
            {
                if (kc == (int)System.Windows.Forms.Keys.C && isC)
                {
                    GlobalModel.mModelCopy = getModel();
                }
                return false;
            };
        }

        const uint mColorMainUnSelect = 0xFFAAAA90;
        const uint mColorMainSelect = 0xffadff2f;

        const uint mColorInOut = 0xFFAAAA90;

        public override bool postTestPick(Point pos)
        {
            return false;
            //var posIn = mMainIns.invertTransform(pos);
            //if(mMainIns.pickRect.transform(mMainIns.transformMatrix).Contains(posIn) )
            //    return true;
            //return false;
        }



        public void reflush()
        {
            mMainIns = new ViewItemTemplate();
            mMainIns.setSize(80, 54);
            mMainIns.setColor(mColorMainUnSelect);
            mMainIns.paresent = this;
            mMainIns.setLable(mModel.name);
            mMainIns.setType("be");
            

            var w = mMainIns.drawRect.Width;
            var left = mMainIns.position.X;
            var right = left + w;
            var dataInWidth = w/(mModel.mDataIn.Count()+1);
            var dataOutWidth = w/(mModel.mDataOut.Count()+1);

            var h = mMainIns.drawRect.Height;
            var up = mMainIns.position.Y;
            var down = up+h;
            var triggerInHeight = h/(mModel.triggerInNumber+1);
            var triggerOutHeight = h/(mModel.triggerOutNumber+1);

            for (int i = 0; i<mModel.mDataIn.Count(); ++i)
            {
                var ui = new UIArrow(8, 8, EForward.e_down);
                ui.setFillColor(mColorInOut);
                ui.paresent = this;
                ui.attrs["type"] = mModel.mDataIn[i];
                ui.middleButtom = new Point(dataInWidth + dataInWidth * i, up);
                mDataIn.Add(ui);
            }

            for (int i = 0; i < mModel.mDataOut.Count(); ++i)
            {
                var ui = new UIArrow(8, 8, EForward.e_down);
                ui.setFillColor(mColorInOut);
                ui.paresent = this;
                ui.attrs["type"] = mModel.mDataOut[i];
                ui.attrs["index"] = i;
                ui.middleTop = new Point(dataOutWidth+dataOutWidth * i, down);
                mDataIn.Add(ui);
            }

            for (int i = 0; i < mModel.triggerInNumber; ++i)
            {
                var ui = new UIArrow(8, 8, EForward.e_right);
                ui.setFillColor(mColorInOut);
                ui.paresent = this;
                ui.attrs["index"] = i;
                ui.rightMiddle = new Point(left, triggerInHeight + triggerInHeight * i);
                mTriggerIn.Add(ui);
            }

            for (int i = 0; i < mModel.triggerOutNumber; ++i)
            {
                var ui = new UIArrow(8, 8, EForward.e_right);
                ui.setFillColor(mColorInOut);
                ui.paresent = this;
                ui.attrs["index"] = i;
                ui.leftMiddle = new Point(right, triggerInHeight + triggerInHeight * i);
                mTriggerOut.Add(ui);
            }
        }

    }
}
