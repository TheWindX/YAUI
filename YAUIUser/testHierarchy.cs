using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_YAUIUser
{
    using ns_YAUI;
    class testHierarchy : Singleton<testHierarchy>
    {
        public testHierarchy()
        {
            UI.Instance.setTitle(@"test of hierarchy");

            var sun = UI.Instance.fromXML(@"
    <round location='256' dragAble='*true' id='sun' radius='64'  color='yellow'>
        <round id='earth' radius='12'  px='128' color='green'>
            <round id='moon' radius='6' px='32' color='silver'>
            </round>
        </round>
    </round>
");
            var earth = sun.findByID("earth");
            var moon = sun.findByID("moon");

            TimerManager.get().setInterval(
                t =>
                {
                    earth.rotate(0.3f);
                    moon.rotate(0.2f);
                    UI.Instance.flush();
                }, 5);
        }
    }
}
