using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testWindow : Singleton<testWindow>
    {
        public testWindow()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect resizeAble=""true"" layout=""vertical"" padding=""5"" dragAble=""true"" fillColor=""ff1ba1e2"">
        <lable text=""x"" align=""rightTop"" alignParesent=""rightTop"" padding=""4"" ></lable>
        <rect width=""40"" height=""40"" align=""rightBottom"" alignParesent=""rightBottom""></rect>
        <stub layout=""horizen"">
            <innerTemplate name=""tab"">
                <round_rect width=""120"" height=""30"" fillColor=""ff7f8899"" radius=""8"" marginX=""0"" clip=""true"" leftBottomCorner=""false"" rightBottomCorner=""false"">
                    <lable text=""tab"" alignParesent=""center"" align=""center""></lable>
                </round_rect>
            </innerTemplate>
            <apply innerTemplate=""tab""></apply>
            <apply innerTemplate=""tab"" fillColor=""0xff3e4649""></apply>
        </stub>
        <rect width=""512"" height=""512"" fillColor=""0xff3e4649"">
        </rect>
        <rect width=""30"" height=""30"" align=""rightBottom"" alignParesent=""rightBottom""></rect>
    </rect>
");


        }
    }
}
