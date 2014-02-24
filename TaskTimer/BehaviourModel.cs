using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{


    class BehaviourModel : iModelBeh, iModelName
    {
        #region interface
        string mName;
        public string name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        string mType = "B";
        public string type
        {
            get
            {
                return mType;
            }
        }


        public iViewerSelf getViewerSelf()
        {
            return new BehaviourViewSelf(this);
        }

        public iViewerBehItem getViewerBehItem()
        {
            var ret = new BehaviourViewInstance(this);
            ret.dragAble = true;
            return ret;
        }

        public iViewerNameItem getViewerNameItem()
        {
            var ret = new BehaviourViewInstance(this);
            return ret;
            //this should be in self
            
        }

        #endregion


        public int triggerInNumber;
        public int triggerOutNumber;

        public List<MetaType> mDataIn = new List<MetaType>();
        public List<MetaType> mDataOut = new List<MetaType>();
    }

    class BehaviourModelInstance : Singleton<BehaviourModelInstance>
    {
        public BehaviourModel mAdd;
        public BehaviourModel mLogS;
        public BehaviourModel mMain;

        public BehaviourModelInstance()
        {
            mAdd = new BehaviourModel();
            mAdd.name = "Add";
            mAdd.mDataIn.Add(MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_int));
            mAdd.mDataIn.Add(MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_int));
            mAdd.mDataOut.Add(MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_int));
            mAdd.triggerInNumber = 1;
            mAdd.triggerOutNumber = 1;

            mLogS = new BehaviourModel();
            mLogS.name = "LogS";
            mLogS.mDataIn.Add(MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_string));
            mLogS.triggerInNumber = 1;
            mLogS.triggerOutNumber = 1;

            mMain = new BehaviourModel();
            mMain.name = "Main";
            mMain.triggerInNumber = 1;
            mLogS.triggerOutNumber = 1;
        }

    };
}
