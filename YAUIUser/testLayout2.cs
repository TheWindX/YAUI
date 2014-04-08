using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testLayoutInverse : Singleton<testLayoutInverse>
    {
        public testLayoutInverse()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble='true' scaleAble='true' width='512' height='512' layout='horizon' layoutInverse='true' layoutFilled='true' paddingX='4' paddingY='4'>
        <rect width='64' height='64' margin='8' expandY='true'></rect>
        <rect width='64' height='64' margin='8' ></rect>
    </rect>
");
        }
    }
}
