using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_behaviour
{
    using ns_utils;
    class MetaType
    {
        public virtual bool equal(MetaType other) { return true; }
        public virtual int hash() { return 0; }
        //public virtual Data defaultData();

        Dictionary<int, array<MetaType>> mTemplates = new Dictionary<int, array<MetaType>>();
        public MetaType getTemplate()
        {
            array<MetaType> tlist;
            if (mTemplates.TryGetValue(this.hash(), out tlist))
            {
                foreach (var t in tlist)
                {
                    if (t.equal(this))
                        return t;
                }
                tlist.push(this);
                return this;
            }
            else
            {
                var ins = new array<MetaType>();
                ins.push(this);
                mTemplates.Add(this.hash(), ins);
                return this;
            }
        }
    }

    class MetaTypeType : MetaType
    {
        public override bool equal(MetaType other)
        {
            if (object.ReferenceEquals(this, other)) return true;
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
            if (object.ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            if (mRef.equal((other as MetaTypeRef).mRef))
            {
                return true;
            }

            return false;
        }


        public override int hash()
        {
            return mRef.hash() + 124;
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
            if (object.ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            if (first.equal((other as MetaTypePair).first))
            {
                if (sec.equal((other as MetaTypePair).sec))
                {
                    return true;
                }
            }

            return false;
        }

        public override int hash()
        {
            return first.hash() + sec.hash() + 125;
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
            if (object.ReferenceEquals(this, other)) return true;
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

        public array<MetaType> mTypes = new array<MetaType>();
        public override bool equal(MetaType other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;

            var otherand = other as MetaTypeList;

            if (mTag != otherand.mTag) return false;

            for (int i = 0; i < mTypes.size(); ++i)
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
            mTypes.push(t);
        }

    }

    class MetaTypeBehaviour : MetaType
    {
        array<MetaTypeType> mDataIn;
        array<MetaTypeType> mDataOut;
        public int mTriggerIn;
        public int mTriggerOut;

        public override bool equal(MetaType other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;

            var other1 = other as MetaTypeBehaviour;

            if (mDataIn.size() != other1.mDataIn.size()) return false;
            if (mDataOut.size() != other1.mDataOut.size()) return false;

            if (other1.mTriggerIn != mTriggerIn) return false;
            if (other1.mTriggerOut != mTriggerOut) return false;

            for (int i = 0; i < mDataIn.size(); ++i)
            {
                if (!mDataIn[i].equal(other1.mDataIn[i]) ) return false;
            }

            for (int i = 0; i < mDataIn.size(); ++i)
            {
                if (!mDataOut[i].equal(other1.mDataOut[i])) return false;
            }
            return true;
        }

        public override int hash()
        {
            int ret = 0;
            for (int i = 0; i < mDataIn.size(); ++i)
            {
                ret += mDataIn[i].hash();
            }

            for (int i = 0; i < mDataOut.size(); ++i)
            {
                ret += mDataOut[i].hash();
            }
            
            ret += mTriggerIn.GetHashCode();
            ret += mTriggerOut.GetHashCode();
            ret += 128;
            return ret;
        }
    }

    
}
