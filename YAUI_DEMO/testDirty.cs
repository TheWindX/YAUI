using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUIUser
{
    using ns_YAUI;
    using ns_YAUtils;
    class testDir : Singleton<testDir>
    {
        public testDir()
        {
            UIRoot.Instance.root.appendFromXML(
@"
    <rect name='r1' size='512' dragAble='true' rotateAble='true' scaleAble='true'>
        <round_rect name='r2' radius='5' size='256'>
            <round_rect  name='r3' size='128'>
                <rect name='r4' length = 'NA' width='22' height='64'>
                    <label name='r5' text='LEELOO' color='0xffffff00'></label>
                </rect>    
            </round_rect >
        </round_rect >
    </rect>
");
            var r3 = UIRoot.Instance.root.findByPath("r1/r2/r3");
            r3.evtOnEnter += ()=>{Console.WriteLine("r3 enter!");};
            r3.evtOnExit += () => { Console.WriteLine("r3 exit!"); };

        }
    }
}
