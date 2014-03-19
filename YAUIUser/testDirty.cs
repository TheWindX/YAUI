using System;
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
    <rect name=""r1"" width=""512"" height=""512"" >
        <round_rect clip=""true"" name=""r2"" radius=""5"" width=""256"" height=""256"" dragAble=""true"" >
            <round_rect  name=""r3"" radius=""5"" width=""128"" height=""128"" dragAble=""true"">
                <rect  name=""r4"" width=""22"" height=""64"" dragAble=""true"">
                    <lable name=""r5"" dragAble=""true"" text=""LEELOO"" color=""0xffffff00""></lable>
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
