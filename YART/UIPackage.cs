using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YART
{
    using ns_YAUI;
    class UIPackage : UIRect
    {
        Packge mPkg = null;
        public UIPackage(Packge p):base(512, 512)
        {
            mPkg = p;
            dragAble = true;
            paddingY = paddingX = 5;
            layout = ELayout.vertical;
            //expandAbleY = expandAbleX = true;
            //shrinkAble = true;
            var items = p.getItems();
            foreach (var elem in items)
            {
                var ui = elem.funcGetUiWidget();
                ui.paresent = this;
            }
        }
        
    }
}
