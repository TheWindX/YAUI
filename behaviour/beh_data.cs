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

namespace ns_behaviour
{
    using ns_utils;    
    class Data
    {
        //static int idc = 0;
        //public int id = 0;
        //public Data() { id = idc++; }
        public MetaType mType;
        virtual public MetaType getType()
        {
            return mType;
        }
        public virtual Data clone() { return null; }
    }

    class IntData : Data
    {
        public int mVal;
        IntData(int v)
        {
            mType = MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_int);
            mVal = v;
        }

        public static IntData getData(int v)
        {
            return new IntData(v);
        }

        public static IntData defaultData()
        {
            return getData(0);
        }

        public override Data clone() { return getData(mVal); }
    }


    class StringData : Data
    {
        public string mVal;
        StringData(string v)
        {
            mType = MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_string);
            mVal = v;
        }

        public static StringData getData(string v)
        {
            return new StringData(v);
        }

        public static StringData defaultData()
        {
            return getData("");
        }

        public override Data clone() { return getData(mVal); }
    }

    class BehData
    {
        public MetaType mType;//type
        public BehaviourData mData;//belongs
        public int mIdx;//位置
        public bool inOrOut;
    }

    class BehTrigger
    {
        public BehaviourData mData;//belongs
        public int mIdx;//位置
        public bool inOrOut;
    }

    class BehaviourData : Data
    {
        public string mName = "";
        public array<BehData> mDIn = new array<BehData>();
        public array<BehData> mDOut = new array<BehData>();
        public array<BehTrigger> mTIn = new array<BehTrigger>();
        public array<BehTrigger> mTOut = new array<BehTrigger>();
    }

    class BehaviourScriptData : BehaviourData
    {
        public Action<BehaviourData> mAction;
    }

    class BehaviourGraphicData : BehaviourData
    {
        array<GBehavioursInstance> mBehGraphic = new array<GBehavioursInstance>();
        array<GDataLink> mDataLinks = new array<GDataLink>();
    }

    interface IGraphicDraw
    {
        void drawRect(float x, float y, float wide, float height, UInt32 color);
        void fillRect(float x, float y, float wide, float height, UInt32 color);

        void drawLine(float x1, float y1, float x2, float y2, UInt32 color);
        void drawText(string str, float x, float y, int sz, UInt32 color = 0xffffffff);

        void pushClip(float x, float y, float wide, float height);
        void popClip();
    };

    class GraphicData
    {
        public BehaviourGraphicData mData;
    }

    class GVector2D
    {
        public float x;
        public float y;
        public GVector2D(float px, float py)
        {
            x = px;
            y = py;
        }

        public GVector2D() : this(0, 0)
        {   
        }

        public static GVector2D operator +(GVector2D p1, GVector2D p2)
        {
            return new GVector2D(p1.x + p2.x, p1.y+p2.y);
        }

    }

    class GBehavioursInstance : GraphicData
    {
        public BehaviourData mSource;

        public GVector2D calculateSize()
        {
            int szH = mSource.mDIn.size();
            szH = szH > mSource.mDOut.size() ? szH : mSource.mDOut.size();
            if(szH == 0) szH = 64;

            int szV = mSource.mTIn.size();
            szV = szV > mSource.mTOut.size() ? szV : mSource.mTOut.size();
            if(szV == 0) szV = 48;

            int nameLength = mSource.mName.Count()*24;
            if (nameLength > szH) szH = nameLength;
            return new GVector2D(szH, szV);
        }
    }

    class GDataLinkPos : GraphicData
    {   
        public GBehavioursInstance mIns;//if null, it is self link
        bool mInOrOut;
        int mIndex;
    }

    class GDataLink : GraphicData
    {
        public GDataLinkPos from;
        public GDataLinkPos to;
        //position

        array<GVector2D> mShape = new array<GVector2D>();
    }

    class GTriggerLinkPos : GraphicData
    {
        public GBehavioursInstance mIns;//if null, it is mData linker
        int mIndex;
        bool mInOrOut;
    }

    class GTriggerLink : GraphicData
    {   
        public GTriggerLinkPos from;
        public GTriggerLinkPos to;
        //
        array<GVector2D> mShape = new array<GVector2D>();
    }
}
