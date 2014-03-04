﻿/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class testTemplate : Singleton<testTemplate>
    {
        public testTemplate()
        {
            Globals.Instance.evtOnInit += main;
        }

        public void main()
        {
            UIRoot.Instance.root.appendFromXML(
                @"
    <rect width=""256"" height=""256"" layout=""horizen"" paddingX=""4"" paddingY=""4"">
        <innerTemplate name=""t1"">
            <rect width=""64"" height=""64"" marginX=""2"" marginY=""2"" layout=""horizen"">
                <rect name=""r1"" width=""20"" height=""20"" marginX=""2"" marginY=""2"">
                </rect>
            </rect>
        </innerTemplate>
        <apply innerTemplate=""t1"" height=""128"" r2.height=""55"">
               <rect name=""r2"" width=""30"" height=""20"" marginX=""2"" marginY=""2"">
               </rect>
        </apply>
        <apply innerTemplate=""t1"" height=""64""></apply>
        <apply innerTemplate=""t1"" height=""32""></apply>
    </rect>
");
        }
    }
}
