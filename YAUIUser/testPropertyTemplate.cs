using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testPropertyTemplate : Singleton<testPropertyTemplate>
    {
        public testPropertyTemplate()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble='true' margin='4' paddingX='4' paddingY='4' shrink='true' layout='horizon'>
        <rect width='128' height='32' shrink='NA'></rect>
        <rect></rect>
    </rect>
");
        }
    }
}
