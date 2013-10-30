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
    };

    class BehData : BehAccessory
    {
        public BehData mFrom;
        public List<BehData> mTo = new List<BehData>();
        public Data mCurrentData;

        public void linkTo(BehData to)
        {
            to.mFrom = this;
            mTo.Add(to);//may be duplicate
        }

        public void linkFrom(BehData from)
        {
            mFrom = from;
            from.mTo.Add(this);//may be duplicate
        }
        
    }

    class BehDataIn : BehData
    {
        public BehDataIn(BehaviourData d)
        {
            mData = d;
        }
        public void update()
        {
            if (mFrom == null)
            {

            }
            else
            {
                mCurrentData = mFrom.mCurrentData;
            }
        }
    }

    class BehDataOut : BehData
    {
        LinkedList<Data> mDataStack = new LinkedList<Data>();

        public BehDataOut( BehaviourData d)
        {
            mData = d;
        }

        public void push()
        {
            mDataStack.AddLast(mCurrentData);
            mCurrentData = mCurrentData.clone();
        }
        public void pop()
        {
            mCurrentData = mDataStack.Last();
            mDataStack.RemoveLast();
        }
    };

    class BehTrigger : BehAccessory
    {
        public bool mState = false;
        public BehTrigger mFrom;
        public List<BehTrigger> mTo = new List<BehTrigger>();

        public void linkTo(BehTrigger to)
        {
            to.mFrom = this;
            mTo.Add(to);//may be duplicate
        }

        public void linkFrom(BehTrigger from)
        {
            mFrom = from;
            from.mTo.Add(this);//may be duplicate
        }
    }

    class BehTriggerIn : BehTrigger
    {
        public BehTriggerIn(BehaviourData d)
        {
            mData = d;
        }

        public void update()
        {
            if (mFrom == null)
            {
                mState = false;
            }
            else
            {
                mState = mFrom.mState;
            }
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
                    elem.mCurrentData = di.mCurrentData;
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
        public enum EDATATYPE
        {
            e_script,
            e_ghaphic,
        };

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
            mDataIn[idx]  = di;
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
        }

        public void setInterfaceTriggerIn(int sz)
        {
            ListExtra.Resize(mTriggerIn, sz);
            for(int i = 0; i<mTriggerIn.Count; ++i)
            {
                var t = new BehTriggerIn(this);
                mTriggerIn[i] = t;
            }
        }

        public void setInterfaceTriggerOut(int sz)
        {
            ListExtra.Resize(mTriggerOut, sz);
            for(int i = 0; i<mTriggerOut.Count; ++i)
            {
                var t = new BehTriggerOut(this);
                mTriggerOut[i] = t;
            }
        }

        BehaviourData mUp = null;
        LinkedList<BehaviourData> mUpList = new LinkedList<BehaviourData>();

        public void push(BehaviourData beh)
        {
            mUpList.AddLast(mUp);
            mUp = beh;
            foreach (var d in mDataOut)
            {
                d.push();
            }

            foreach (var t in mTriggerOut)
            {
                t.push();
            }
        }

        public void pop()
        {
            mUp = mUpList.Last();
            mUpList.RemoveLast();
            foreach (var d in mDataOut)
            {
                d.pop();
            }
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

        public override Data clone() 
        {
            var ret = new BehaviourData();
            ret.mDataType = this.mDataType;
            ret.mAction = this.mAction;
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
                    Console.WriteLine(ret);
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

            

            test1.mDataType = BehaviourData.EDATATYPE.e_ghaphic;
            test1.mGraph = new BehGraphcs(test1);
            test1.mGraph.addBehaviourData(badd);
            test1.mGraph.addBehaviourData(blog);

            test1.addInterfaceDataInInt(0);
            test1.addInterfaceDataInInt(1);
            test1.setInterfaceTriggerIn(1);
            test1.setTriggerIn(0, true);
            test1.setDataIntIn(0, 3);
            test1.setDataIntIn(0, 5);
            
            test1.mTriggerIn[0].linkTo(badd.mTriggerIn[0]);
            test1.mDataIn[0].linkTo(badd.mDataIn[0]);
            test1.mDataIn[1].linkTo(badd.mDataIn[1]);
            
            badd.mTriggerOut[0].linkTo(blog.mTriggerIn[0]);
            badd.mDataOut[0].linkTo(blog.mDataIn[0]);
            
            

        }

        public void t1()
        {
            prelude();
            test1.run();
        }
    }

}
