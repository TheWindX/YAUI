﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class testResizer : Singleton<testResizer>
    {
        public testResizer()
        {
            UIRoot.Instance.root.rotateAble = true;
            UIRoot.Instance.root.dragAble = true;
            UIRoot.Instance.root.scaleAble = true;
            UIRoot.Instance.root.appendFromXML(@"
<rect shrink='true' clip='*true' padding='5' fillColor='0xff3e4649' layout='vertical'>
    <label align='leftTop'></label>
    <label align='rightTop' text='x'></label>
    <div size='30'></div>
    <resizer size='256'> </resizer>
</rect>
");
        }
    }
}
