/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    using ns_utils;

    class BehviourAccessory
    {
        public Behaviour mBehaviour;//belonging
        public int mIdx;//in list
    };

    class DataIn : BehviourAccessory
    {
        public MetaType mType;
    }

    class DataOut : BehviourAccessory
    {
        public MetaType mType;
    }

    class TriggerIn : BehviourAccessory
    {
    }

    class TriggerOut : BehviourAccessory
    {
    }



    class Behaviour
    {
        public array<DataIn> mDataIn;
        public array<DataIn> mDataOut;
        public array<TriggerIn> mTriggerIn;
        public array<TriggerOut> mTriggerOut;

        public string name
        {
            get;
            set;
        }
    }


    class BehaviourScript : Behaviour
    {
        public Action<Behaviour> mAction;
    }

    class BehaviourGraphic : Behaviour
    {
        
    }

    class BehaviourInGraphic
    {
        
    }



    class GBehaviour : BehaviourInGraphic
    {
        public Point mPosition;
        public Behaviour mBeh;
    }

    class GSelfDataIn : BehaviourInGraphic, iGDataOut
    {
        public DataIn mDataIn;
        public MetaType getType()
        {
            return mDataIn.mType;
        }
    }

    class GSelfDataOut : BehaviourInGraphic, iGDataIn
    {
        public DataOut mDataOut;
        public MetaType getType()
        {
            return mDataOut.mType;
        }
    }

    class GSelfTriggerIn : BehaviourInGraphic, iGTriggerOut
    {
        public TriggerIn mTriggerIn;
    }

    class GSelfTriggerOut : BehaviourInGraphic, iGTriggerIn
    {
        public TriggerOut mTriggerOut;
    }

    interface iGDataIn
    {
        MetaType getType();
    }

    interface iGDataOut
    {
        MetaType getType();
    }

    interface iGTriggerIn
    {
        
    }

    interface iGTriggerOut
    {

    }

    class GDataIn : BehaviourInGraphic, iGDataIn
    {
        public GBehaviour mGBeh;
        public DataIn mDataIn;

        public MetaType getType()
        {
            return mDataIn.mType;
        }
    }

    class GDataOut : BehaviourInGraphic, iGDataOut
    {
        public GBehaviour mGBeh;
        public DataOut mDataOut;

        public MetaType getType()
        {
            return mDataOut.mType;
        }
    }


    class GTriggerIn : BehaviourInGraphic, iGTriggerIn
    {
        public GBehaviour mGBeh;
        public TriggerIn mDataIn;
    }

    class GTriggerOut : BehaviourInGraphic, iGTriggerOut
    {
        public GBehaviour mGBeh;
        public TriggerOut mDataOut;
    }

    class GDataLinker : BehaviourInGraphic
    {
        public iGDataIn mDi;
        public iGDataOut mDo;

        public List<Point> mPoints = new List<Point>();
    }

    class GTriggerLinker : BehaviourInGraphic
    {
        public iGTriggerIn mTi;
        public iGTriggerOut mTo;

        public List<Point> mPoints = new List<Point>();
    }

}
