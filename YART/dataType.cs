using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    //类型
    interface iDataType : iPackageItem
    {
        string tagName();
    }

    public class CInt_t : iDataType
    {
        public CInt_t()
        {
            mImpl.name = "int_t";
        }

        #region iPackageItem impl
        public implPackageItem mImpl = new implPackageItem();
        string iPackageItem.name
        {
            get { return mImpl.name; }
            set { mImpl.name = value; }
        }

        List<CPacakge> iPackageItem.locations()
        {
            return mImpl.locations();
        }
        #endregion

        string iDataType.tagName()
        {
            return "Int_t";
        }

        #region singleton
        public static CInt_t instance = new CInt_t();
        #endregion
    }


    //可放于map空间
    interface iMapSpace
    {
    }

    interface iData_d : iMapSpace
    {
        iDataType getTypeof();
        string tagName();
    }

    class CArg_d : iData_d
    {
        #region iData_d impl
        public iDataType mType;
        iDataType iData_d.getTypeof()
        {
            return mType;
        }

        string iData_d.tagName()
        {
            return "arg";
        }
        #endregion
    }

    interface iNoArgData_d : iData_d, iPackageItem
    {   
    }

    class CInt_d : iNoArgData_d
    {
        #region iPackageItem impl
        public implPackageItem mImpl = new implPackageItem();
        string iPackageItem.name
        {
            get { return mImpl.name; }
            set { mImpl.name = value; }
        }

        List<CPacakge> iPackageItem.locations()
        {
            return mImpl.locations();
        }
        #endregion

        iDataType iData_d.getTypeof()
        {
            return CInt_t.instance;
        }

        string iData_d.tagName()
        {
            return "Int_d";
        }
    }
}
