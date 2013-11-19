using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    interface iViewInstance
    {
        void select();
        void unselect();
    }

    class NamespaceViewInstance : ViewItemTemplate, iViewInstance
    {
        MNamespace mModel;
        public NamespaceViewInstance(MNamespace self)
        {
            mModel = self;

            if (mModel == null) return;

            setType(mModel.type);
            setLable(mModel.name);
            setColor(mStanderdColor);

            evtOnDClick += (ui, x, y) =>
                {
                    var v = mModel.viewSelf;
                    v.paresent = UIRoot.Instance.root;
                    return false;
                };
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


    }

}
