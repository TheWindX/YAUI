using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class testPropertyTemplate : Singleton<testPropertyTemplate>
    {
        public testPropertyTemplate()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble='true' margin='4' paddingX='4' paddingY='4' shrink='true' layout='horizon' strokeColor='red'>
        <rect width='32' height='*132' strokeColor='blue'></rect>
        <rect ></rect>
    </rect>
");
        }
    }
}
