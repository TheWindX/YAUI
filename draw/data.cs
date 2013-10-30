using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draw1
{
    public static class ListExtra
    {
        public static void Resize<T>(this List<T> list, int sz, T c = default(T))
        {
            int cur = list.Count;
            if(sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if(sz > cur)
                list.AddRange(Enumerable.Repeat(c, sz - cur));
        }
    }

    class MetaType
    {
        public virtual bool equal(MetaType other){return true;}
        public virtual int hash(){return 0;}
        //public virtual Data defaultData();

        Dictionary<int, List<MetaType>> mTemplates = new Dictionary<int, List<MetaType>>();
        public MetaType getTemplate()
        {
            List<MetaType> tlist;
            if (mTemplates.TryGetValue(this.hash(), out tlist))
            {
                foreach (var t in tlist)
                {
                    if (t.equal(this))
                        return t;
                }
                tlist.Add(this);
                return this;
            }
            else
            {
                var ins = new List<MetaType>();
                ins.Add(this);
                mTemplates.Add(this.hash(), ins);
                return this;
            }
        }
    }

    class MetaTypeType : MetaType
    {
        public override bool equal(MetaType other) 
        { 
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;
            return true;
        }
        public override int hash() { return 123; }

        //public override Data defaultData()
        //{
        //    return TypeData.defaultData();
        //}

        public static MetaTypeType instance()
        {
            return new MetaTypeType().getTemplate() as MetaTypeType;
        }
    }

    class MetaTypeRef : MetaType
    {
        public MetaType mRef;

        public override bool equal(MetaType other)
        {
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;
            if (mRef.equal((other as MetaTypeRef).mRef))
            {
                return true;
            }

            return false;
        }


        public override int hash()
        {
            return mRef.hash()+124;
        }

        public static MetaTypePair instance(MetaType t1, MetaType t2)
        {
            var t = new MetaTypePair();
            t.first = t1;
            t.sec = t2;
            return t.getTemplate() as MetaTypePair;
        }
    }

    class MetaTypePair : MetaType
    {
        public MetaType first;
        public MetaType sec;

        public override bool equal(MetaType other)
        {
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;
            if(first.equal( (other as MetaTypePair).first ) )
            {
                if(sec.equal( (other as MetaTypePair).sec ) )
                {
                    return true;
                }
            }
            
            return false;
        }

        public override int hash()
        {
            return first.hash() + sec.hash()+125;
        }

        public static MetaTypePair instance(MetaType t1, MetaType t2)
        {
            var t = new MetaTypePair();
            t.first = t1;
            t.sec = t2;
            return t.getTemplate() as MetaTypePair;
        }
    }

    class MetaTypePrimary : MetaType
    {
        public enum ETYPE
        {
            e_invalid,
            e_int,
            e_bool,
            e_string,
        }
        public ETYPE mType = ETYPE.e_invalid;

        public override bool equal(MetaType other)
        {
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;
            if (this.mType == (other as MetaTypePrimary).mType) 
            {
                return true;
            }
            return false;
        }

        public override int hash()
        {
            return mType.GetHashCode() + 126;
        }

        public static MetaTypePrimary instance(ETYPE t)
        {
            var ret = new MetaTypePrimary();
            ret.mType = t;
            return ret.getTemplate() as MetaTypePrimary;
        }

    }

    class MetaTypeList : MetaType
    {
        public string mTag = "";
        
        public List<MetaType> mTypes = new List<MetaType>();
        public override bool equal(MetaType other)
        {
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;
            
            var otherand = other as MetaTypeList;

            if (mTag != otherand.mTag) return false;

            for(int i = 0; i<mTypes.Count(); ++i)
            {
                if (!mTypes[i].equal(otherand.mTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int hash()
        {
            int ret = mTag.GetHashCode();
            foreach (var t in mTypes)
            {
                ret += t.hash();
            }
            ret += 127;
            return ret;
        }

        public static MetaTypeList create(string t)
        {
            MetaTypeList ret = new MetaTypeList();
            ret.mTag = t;
            return ret;
        }

        public void addType(MetaType t)
        {
            mTypes.Add(t);
        }

    }

    class MetaTypeBehaviour : MetaType
    {
        MetaTypeList mDataIn;
        MetaTypeList mDataOut;
        public int mTriggerIn;
        public int mTriggerOut;

        public override bool equal(MetaType other)
        {
            if(object.ReferenceEquals(this, other) ) return true;
            if (this.GetType() != other.GetType()) return false;

            var other1 = other as MetaTypeBehaviour;

            if (!other1.mDataIn.equal(this.mDataIn)) return false;
            if (!other1.mDataOut.equal(this.mDataOut)) return false;
            if (other1.mTriggerIn != mTriggerIn) return false;
            if (other1.mTriggerOut != mTriggerOut) return false;
            
            return true;
        }

        public override int hash()
        {
            int ret = mDataIn.GetHashCode();
            ret += mDataOut.GetHashCode();
            ret += mTriggerIn.GetHashCode();
            ret += mTriggerOut.GetHashCode();
            ret += 128;
            return ret;
        }
    }

    class Data
    {
        static int idc = 0;
        public int id = 0;
        public Data() { id = idc ++; }
        public MetaType mType;

        public object extra;

        public virtual Data clone(){return null;}
    }

    class InvalidData : Data
    {
        InvalidData()
        {
            mType = MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_invalid);
        }
        static InvalidData ins = new InvalidData();
        public static InvalidData getData()
        {
            return ins;
        }

        public InvalidData defaultData()
        {
            return getData();
        }

        public override Data clone() { return this; }
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

    class TypeData : Data
    {
        public MetaType mVal;

        public static TypeData getData(MetaType val)
        {
            var ret = new TypeData();
            ret.mVal = val;
            ret.mType = MetaTypeType.instance();
            return ret;
        }

        public static TypeData defaultData()
        {
            return getData(MetaTypePrimary.instance(MetaTypePrimary.ETYPE.e_invalid) );
        }

        public override Data clone() { return getData(mVal); }
    }

    class PairData : Data
    {
        public Data first;
        public Data second;

        public static PairData getData(Data f, Data s)
        {
            var ret = new PairData();
            ret.first = f;
            ret.second = s;

            ret.mType = MetaTypePair.instance(ret.first.mType, ret.second.mType);
            return ret;
        }

        public override Data clone() {
            Data d1 = first.clone();
            Data d2 = second.clone();
            return getData(d1, d2); 
        }

    }

    class ListData : Data
    {
        public List<Data> mDatas = new List<Data>();

        public static ListData create(string tag)
        {
            var ret = new ListData();
            ret.mType = MetaTypeList.create(tag);
            return ret;
        }

        public void add(Data d)
        {
            mDatas.Add(d);
            (mType as MetaTypeList).addType(d.mType);
        }

        public void done()
        {
            mType = (mType as MetaTypeList).getTemplate();
        }

        public override Data clone()
        {
            //TODO,
            return null;
        }
    }

    class BehAccessory
    {
        public BehaviourData mData;
        public int idx = 0;
    };

    class BehData : BehAccessory
    {
        public List<BehData> mTo = new List<BehData>();
        public Data mCurrentData;

        public virtual BehData clone(BehaviourData d)
        {
            BehData ret = new BehData();
            ret.mData = d;
            ret.idx = idx;
            ret.mCurrentData = mCurrentData.clone();
            return ret;
        }

        public void linkTo(BehData to)
        {
            mTo.Add(to);//may be duplicate
        }

        public void linkFrom(BehData from)
        {
            from.mTo.Add(this);//may be duplicate
        }
        
    }

    class BehDataIn : BehData
    {
        public BehDataIn(BehaviourData d)
        {
            mData = d;
        }

        public override BehData clone(BehaviourData d)
        {
            BehDataIn ret = new BehDataIn(d);
            ret.mData = d;
            ret.idx = idx;
            ret.mCurrentData = mCurrentData.clone();
            return ret;
        }
    }

    class BehDataOut : BehData
    {
        LinkedList<Data> mDataStack = new LinkedList<Data>();

        public BehDataOut( BehaviourData d)
        {
            mData = d;
        }
        public override BehData clone(BehaviourData d)
        {
            BehDataOut ret = new BehDataOut(d);
            ret.mData = d;
            ret.idx = idx;
            ret.mCurrentData = mCurrentData.clone();
            return ret;
        }
        //public void push()
        //{
        //    mDataStack.AddLast(mCurrentData);
        //    mCurrentData = mCurrentData.clone();
        //}
        //public void pop()
        //{
        //    mCurrentData = mDataStack.Last();
        //    mDataStack.RemoveLast();
        //}
    };

    class BehTrigger : BehAccessory
    {
        public bool mState = false;
        public List<BehTrigger> mTo = new List<BehTrigger>();

        public void linkTo(BehTrigger to)
        {
            mTo.Add(to);//may be duplicate
        }

        public void linkFrom(BehTrigger from)
        {
            from.mTo.Add(this);//may be duplicate
        }

        public virtual BehTrigger clone(BehaviourData d)
        {
            BehTrigger ret = new BehTrigger();
            ret.mData = d;
            ret.idx = idx;
            ret.mState = mState;
            return ret;
        }
    }

    class BehTriggerIn : BehTrigger
    {
        public BehTriggerIn(BehaviourData d)
        {
            mData = d;
        }

        public override BehTrigger clone(BehaviourData d)
        {
            BehTriggerIn ret = new BehTriggerIn(d);
            ret.mData = d;
            ret.idx = idx;
            ret.mState = mState;
            return ret;
        }
    }

    class BehTriggerOut : BehTrigger
    {
        public LinkedList<bool> mStateStack = new LinkedList<bool>();

        public BehTriggerOut(BehaviourData d)
        {
            mData = d;
        }

        public void push()
        {
            mStateStack.AddLast(mState);
            mState = false;
        }
        public void pop()
        {
            mState = mStateStack.Last();
            mStateStack.RemoveLast();
        }

        public override BehTrigger clone(BehaviourData d)
        {
            BehTriggerOut ret = new BehTriggerOut(d);
            ret.mData = d;
            ret.idx = idx;
            ret.mState = mState;
            return ret;
        }
    }

    class BehGraphcs : BehAccessory
    {
        public List<BehaviourData> mBehavious = new List<BehaviourData>();
        LinkedList<LinkedList<BehaviourData>> mPcStack = new LinkedList<LinkedList<BehaviourData>>();
        LinkedList<BehaviourData> mBehStack = new LinkedList<BehaviourData>();
        

        public BehGraphcs(BehaviourData d)
        {
            mData = d;
        }

        public void pushBehProcess(BehaviourData bd)
        {
            mBehStack.AddLast(bd);
        }

        public void addBehaviourData(BehaviourData bd)
        {
            bd.idx = mBehavious.Count;
            mBehavious.Add(bd);
        }

        public BehGraphcs clone(BehaviourData d)
        {
            BehGraphcs ret = new BehGraphcs(d);
            ret.mBehavious = mBehavious;
            ret.mPcStack = new LinkedList<LinkedList<BehaviourData>>();
            ret.mBehStack = new LinkedList<BehaviourData>();



            return ret;
        }

        public void run()
        {
            mPcStack.AddLast(mBehStack);
            mBehStack = new LinkedList<BehaviourData>();
            foreach (var b in mBehavious)
            {
                b.push(this.mData);
            }

            foreach (var di in mData.mDataIn)
            {
                foreach (var elem in di.mTo)
                {
                    elem.mCurrentData = di.mCurrentData.clone();
                }
            }

            foreach (var ti in mData.mTriggerIn)
            {
                if (ti.mState)
                {
                    foreach (var t in ti.mTo)
                    {
                        mBehStack.AddLast(t.mData);
                    }
                }
            }

            for (; mBehStack.Count != 0; )
            {
                var beh = mBehStack.Last();
                mBehStack.RemoveLast();
                beh.run();
            }

            mBehStack.Clear();
            foreach (var b in mBehavious)
            {
                b.pop();
            }

            mBehStack = mPcStack.Last();
            mPcStack.RemoveLast();
        }
    };

    class BehaviourData : Data
    {
        public int idx = 0;
        public enum EDATATYPE
        {
            e_script,
            e_ghaphic,
        };

        public void DataInLinkTo(BehaviourData other, int idx1, int idx2)
        {
            mDataIn[idx1].linkTo(other.mDataIn[idx2]);
        }

        public void DataOutLinkTo(BehaviourData other, int idx1, int idx2)
        {
            mDataOut[idx1].linkTo(other.mDataIn[idx2]);
        }

        public void TILinkTo(BehaviourData other, int idx1, int idx2)
        {
            mTriggerIn[idx1].linkTo(other.mTriggerIn[idx2]);
        }

        public void TOLinkTo(BehaviourData other, int idx1, int idx2)
        {
            mTriggerOut[idx1].linkTo(other.mTriggerIn[idx2]);
        }

        public EDATATYPE mDataType = EDATATYPE.e_script;
        public Action<BehaviourData> mAction = null;

        public BehGraphcs mGraph;

        public List<BehDataOut> mDataOut = new List<BehDataOut>();
        public List<BehDataIn> mDataIn = new List<BehDataIn>();

        public List<BehTriggerOut> mTriggerOut = new List<BehTriggerOut>();
        public List<BehTriggerIn> mTriggerIn = new List<BehTriggerIn>();

        public void addInterfaceDataInInt(int idx)
        {
            if(mDataIn.Count < idx+1)
            {
                ListExtra.Resize(mDataIn, idx+1);
            }
            var di = new BehDataIn(this);
            
            di.mCurrentData = IntData.getData(0);
            mDataIn[idx] = di;
            di.idx = idx;
        }

        public void addInterfaceDataOutInt(int idx)
        {
            if(mDataOut.Count < idx+1)
            {
                ListExtra.Resize(mDataOut, idx+1);
            }
            var dout = new BehDataOut(this);
            dout.mCurrentData = IntData.getData(0);
            mDataOut[idx]  = dout;
            dout.idx = idx;
        }

        public void setInterfaceTriggerIn(int sz)
        {
            ListExtra.Resize(mTriggerIn, sz);
            for(int i = 0; i<mTriggerIn.Count; ++i)
            {
                var t = new BehTriggerIn(this);
                mTriggerIn[i] = t;
                t.idx = i;
            }
        }

        public void setInterfaceTriggerOut(int sz)
        {
            ListExtra.Resize(mTriggerOut, sz);
            for(int i = 0; i<mTriggerOut.Count; ++i)
            {
                var t = new BehTriggerOut(this);
                mTriggerOut[i] = t;
                t.idx = i;
            }
        }

        BehaviourData mUp = null;
        LinkedList<BehaviourData> mUpList = new LinkedList<BehaviourData>();

        public void push(BehaviourData beh)
        {
            mUpList.AddLast(mUp);
            mUp = beh;

            foreach (var t in mTriggerOut)
            {
                t.push();
            }
        }

        public void pop()
        {
            mUp = mUpList.Last();
            mUpList.RemoveLast();

            foreach (var t in mTriggerOut)
            {
                t.pop();
            }
        }

        public void run()
        {
            //foreach (var ti in mTriggerIn)
            //{
            //    ti.update();
            //}

            if (mDataType == EDATATYPE.e_script)
            {
                mAction(this);
            }
            else if (mDataType == EDATATYPE.e_ghaphic)
            {
                mGraph.run();
            }
        }

        void copyTriggerIn(BehaviourData from, BehaviourData to)
        {
            ListExtra.Resize(to.mTriggerIn, from.mTriggerIn.Count);
            for (int i = 0; i < from.mTriggerIn.Count; ++i)
            {
                to.mTriggerIn[i] = from.mTriggerIn[i].clone(to) as BehTriggerIn;
            }
        }

        void copyTriggerOut(BehaviourData from, BehaviourData to)
        {
            ListExtra.Resize(to.mTriggerOut, from.mTriggerOut.Count);
            for (int i = 0; i < from.mTriggerOut.Count; ++i)
            {
                to.mTriggerOut[i] = from.mTriggerOut[i].clone(to) as BehTriggerOut;
            }
        }

        void copyDataIn(BehaviourData from, BehaviourData to)
        {
            ListExtra.Resize(to.mDataIn, from.mDataIn.Count);
            for (int i = 0; i < from.mDataIn.Count; ++i)
            {
                to.mDataIn[i] = from.mDataIn[i].clone(to) as BehDataIn;
            }
        }

        void copyDataOut(BehaviourData from, BehaviourData to)
        {
            ListExtra.Resize(to.mDataOut, from.mDataOut.Count);
            for (int i = 0; i < from.mDataOut.Count; ++i)
            {
                to.mDataOut[i] = from.mDataOut[i].clone(to) as BehDataOut;
            }
        }

        public override Data clone() 
        {
            var ret = new BehaviourData();
            ret.mDataType = this.mDataType;
            ret.mAction = this.mAction;
            ListExtra.Resize(ret.mTriggerIn, mTriggerIn.Count);

            copyDataIn(this, ret);
            copyDataOut(this, ret);
            copyTriggerIn(this, ret);
            copyTriggerOut(this, ret);

            if(this.mGraph != null)
                ret.mGraph = this.mGraph.clone(ret);
            return ret;
        }
        
        public int getIntIn(int idx)
        {
            BehDataIn din = mDataIn[idx];
            return (din.mCurrentData as IntData).mVal;
        }

        public bool getTriggerIn(int idx)
        {
            BehTriggerIn ti = mTriggerIn[idx];
            return ti.mState;
        }

        public void setIntOut(int idx, int val)
        {
            BehDataOut to = mDataOut[idx];
            ((to.mCurrentData) as IntData).mVal = val;
            foreach (var d in to.mTo)
            {
                
                for (int i = 0; i < d.mData.mDataIn.Count; ++i)
                {
                    var t = d.mData.mDataIn[i];
                }
                ((d.mCurrentData) as IntData).mVal = val;
            }
        }

        public void setTriggerOut(int idx, bool st)
        {
            var to = mTriggerOut[idx];
            to.mState = st;
            foreach (var o in to.mTo)
            {
                o.mState = true;

                if (st)
                {
                    if (mUp != null)
                    {
                        mUp.mGraph.pushBehProcess(o.mData);
                    }
                }
            }

        }

        //for test use
        public void setTriggerIn(int idx, bool st)
        {
            mTriggerIn[idx].mState = st;
        }

        public void setDataIntIn (int idx, int val)
        {
            (mDataIn[idx].mCurrentData as IntData).mVal = val;  
        }
    }

    public class testMain
    {
        BehaviourData badd = new BehaviourData();
        BehaviourData blog = new BehaviourData();
        BehaviourData bset = new BehaviourData();
        BehaviourData bgreat = new BehaviourData();

        BehaviourData test1 = new BehaviourData();
        

        void prelude()
        {
            bset.mDataType = BehaviourData.EDATATYPE.e_script;
            bset.addInterfaceDataInInt(0);
            bset.addInterfaceDataOutInt(0);
            bset.setInterfaceTriggerIn(1);
            bset.setInterfaceTriggerOut(1);
            bset.mAction = (beh) =>
                {
                    int a = beh.getIntIn(0);
                    beh.setIntOut(0, a);
                    beh.setTriggerOut(0, true);
                };

            bgreat.mDataType = BehaviourData.EDATATYPE.e_script;
            bgreat.addInterfaceDataInInt(0);
            bgreat.addInterfaceDataInInt(1);
            bgreat.setInterfaceTriggerIn(1);
            bgreat.setInterfaceTriggerOut(2);
            bgreat.mAction = (beh) =>
            {
                int a = beh.getIntIn(0);
                int b = beh.getIntIn(1);
                if (a > b) beh.setTriggerOut(0, true);
                else beh.setTriggerOut(1, true);
            };

            badd.mDataType = BehaviourData.EDATATYPE.e_script;
            badd.setInterfaceTriggerIn(1);
            badd.setInterfaceTriggerOut(1);
            badd.addInterfaceDataInInt(0);
            badd.addInterfaceDataInInt(1);
            badd.addInterfaceDataOutInt(0);

            badd.mAction = (beh)=>
                {
                    int a = beh.getIntIn(0);
                    int b = beh.getIntIn(1);
                    int ret = a+b;
                    beh.setIntOut(0, ret);
                    beh.setTriggerOut(0, true);
                };

            blog.mDataType = BehaviourData.EDATATYPE.e_script;
            blog.setInterfaceTriggerIn(1);
            blog.setInterfaceTriggerOut(1);
            blog.addInterfaceDataInInt(0);
            
            blog.mAction = (beh)=>
                {
                    int ret = beh.getIntIn(0);
                    Console.WriteLine(ret);
                    beh.setTriggerOut(0, true);
                };



            var add1 = badd.clone() as BehaviourData;
            var add2 = badd.clone() as BehaviourData;
            var log0 = blog.clone() as BehaviourData;
            var gt3 = bgreat.clone() as BehaviourData;


            test1.mDataType = BehaviourData.EDATATYPE.e_ghaphic;
            test1.mGraph = new BehGraphcs(test1);
            test1.addInterfaceDataInInt(0);
            test1.addInterfaceDataInInt(1);
            test1.setInterfaceTriggerIn(1);
            test1.setTriggerIn(0, true);
            test1.setDataIntIn(0, 0);
            test1.setDataIntIn(1, 100);

            test1.mGraph.addBehaviourData(log0);
            test1.mGraph.addBehaviourData(add1);
            test1.mGraph.addBehaviourData(add2);
            test1.mGraph.addBehaviourData(gt3);

            test1.TILinkTo(add1, 0, 0);
            add1.TOLinkTo(add2, 0, 0);
            add2.TOLinkTo(gt3, 0, 0);
            gt3.TOLinkTo(log0, 0, 0);
            gt3.TOLinkTo(add1, 1, 0);

            test1.DataInLinkTo(add1, 0, 0);
            test1.DataInLinkTo(add1, 0, 1);
            test1.DataInLinkTo(add2, 0, 0);
            add1.DataOutLinkTo(add1, 0, 1);
            add2.DataOutLinkTo(add1, 0, 0);
            add2.DataOutLinkTo(add2, 0, 0);
            add2.DataOutLinkTo(gt3, 0, 0);
            add2.setDataIntIn(1, 1);
            gt3.setDataIntIn(1, 100);
            add1.DataOutLinkTo(log0, 0, 0);
            
        }

        public void t1()
        {
            prelude();
            test1.run();
        }
    }

}
