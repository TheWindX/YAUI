﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YAUIUser
{
    class testAll : Singleton<testAll>
    {
        public testAll()
        {
            //var test = testUse.Instance;
            //var test = testPrimarys.Instance; 
            //var test = testTransform.Instance;
            //var test = testHierarchy.Instance;
            //var test = testTips.Instance;
            //var test = testMenu.Instance; 
            //var test = testAlign.Instance;
            var test = testScrolledMap.Instance;
            //var test = testLayout.Instance;
            //var _0 =  testDir.Instance;
            //var _1 = testRoundRect.Instance;
            
            //var _2_3 = testLayout3.Instance;
            //var _21 = testLayoutShrinkExpand.Instance;
            //var _3 = testWindow.Instance;
            //var _4 = testLayoutInverse.Instance;
            //var _5 = testPropertyTemplate.Instance;
            //var _6 = testNewControl.Instance;
            //var _7 = testResizer.Instance;
        }
    }
}