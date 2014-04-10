using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class UIPackageItem_package : UIBlank
    {
        UIRect mNameRect = null;
        UILable mName = null;

        UIRect mTypeRect = null;

        Packge mPkg = null;
        public UIPackageItem_package(Packge pkg)
        {
            mPkg = pkg;
            //shrinkAble = true;
            marginX = 5;
            marginY = 5;
            layout = ELayout.horizon;
            mNameRect = appendFromXML(@"
            <rect padding='2' clip='true' shrink='true'></rect>
            ") as UIRect;
            mName = mNameRect.appendFromXML(@"
            <lable size='12' color='black'></lable>") as UILable;

            mTypeRect = appendFromXML(@"
            <rect padding='2' clip='true' shrink='true'>
                <lable size='12' text='dir' color='yellow'></lable>
            </rect>") as UIRect;
            setName(pkg.cast<PackageItem>().name);
        }

        public void setName(string name)
        {
            mName.text = name;
            setDirty(true);
        }
    }
}
