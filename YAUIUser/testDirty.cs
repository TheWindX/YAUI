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
        <round_rect clip=""true"" name=""rr1"" radius=""5"" width=""256"" height=""256"" dragAble=""true"" >
            <round_rect  name=""rrr1"" radius=""5"" width=""128"" height=""128"" dragAble=""true"">
                <rect  name=""rrrr1"" width=""22"" height=""64"" dragAble=""true"">
                    <lable dragAble=""true"" text=""aldsfjasdf"" color=""0xffff0000""></lable>
                </rect>    
            </round_rect >
        </round_rect >
    </rect>
");

        }
    }
}
