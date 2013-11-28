using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;


namespace ns_behaviour
{
    class NamespaceViewInstance : ViewItemTemplate, iViewerNameItem
    {
        #region instance
        public iModel getModel()
        {
            return mModel;
        }

        public UIWidget asWidget()
        {
            return this;
        }

        const uint mStanderdColor = 0xffdeb887;
        const uint mSelectColor = 0xffadff2f;
        public void select()
        {
            setColor(mSelectColor);
        }

        public void unselect()
        {
            setColor(mStanderdColor);
        }

        #endregion

        MNamespace mModel;
        public NamespaceViewInstance(MNamespace self)
        {
            this.dragAble = true;
            mModel = self;

            if (mModel == null) return;

            setType(mModel.type);
            setLable(mModel.name);
            setColor(mStanderdColor);

            evtOnDClick += (ui, x, y) =>
                {
                    var v = mModel.getViewerSelf().asWidget();
                    v.position = v.invertTransformParesentAbs(new Point(x, y));
                    v.paresent = UIRoot.Instance.root;
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

        


    }

}
