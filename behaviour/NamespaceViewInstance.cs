using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    interface iVieweInstance
    {
        void select();
        void unselect();
    }

    class NamespaceViewInstance : UIStub, iVieweInstance
    {
        const string xmllayout = @"
<stub name=""root"">
    <rect width=""96"" height=""64"" 
        offsetx=""4"" offsety=""4""
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887""> </rect>
    <rect width=""96"" height=""64"" 
        offsetx=""2"" offsety=""2""
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887""> </rect>
    <rect name=""main"" width=""96"" height=""64"" 
        stokeColor=""0xffffffff"" fillColor=""0xffdeb887"">
        <rect width=""18"" height=""18"" offsetx=""4"" offsety=""4"">
            <lable text=""NS""
                size=""8""
                color=""0xff00ff00""
                align=""center"" alignParesent=""center"">
            </lable>
        </rect>
        <lable name=""text""> </lable>
    </rect>
</stub>";

        MNamespace mModel;

        public NamespaceViewInstance(MNamespace self)
        {
            mModel = self;
            var root = UIRoot.Instance.loadFromXML(xmllayout);
            if (mModel != null)
            {
                var lb = (root.childOfPath("main/text") as UILable);
                lb.text = mModel.name;
                lb.alignParesent(EAlign.center, EAlign.center);
            }

            root.paresent = this;
            this.dragAble = true;
            this.clip = true;
            
            evtOnLMDown += (ui, x, y) =>
                {
                    if (mParesent != null && mParesent is NamespaceViewSelf)
                    {
                        var p = mParesent as NamespaceViewSelf;
                        p.selectItem = this;
                    }
                    return false;
                };
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
            (this.childOfPath("root/main") as UIRect).fillColor = mSelectColor;
        }

        public void unselect()
        {
            (this.childOfPath("root/main") as UIRect).fillColor = mStanderdColor;
        }


    }

}
