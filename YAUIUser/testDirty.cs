﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAUIUser
{
    using ns_YAUI;

    class testDir : Singleton<testDir>
    {
        public testDir()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect name='r1' length='512' dragAble='true' rotateAble='true' scaleAble='true'>
        <round_rect name='r2' radius='5' length='256'>
            <round_rect  name='r3' length='128'>
                <rect name='r4' length = 'NA' width='22' height='64'>
                    <lable name='r5' text='LEELOO' color='0xffffff00'></lable>
                </rect>    
            </round_rect >
        </round_rect >
    </rect>
");
            var r3 = UIRoot.Instance.root.childOfPath("r1/r2/r3");
            r3.evtEnter += ()=>{Console.WriteLine("r3 enter!");};
            r3.evtExit += () => { Console.WriteLine("r3 exit!"); };

        }
    }
}
