
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testHierarchy : iTestInstance
    {   
        public ECategory category()
        {
            return ECategory.example;
        }

        public string title()
        {
            return "hierarchy";
        }

        public string desc()
        {
            return
@"
控件的层级关系;
";
        }

        int timeID = -1;
        public UIWidget getAttach()
        {
            var sun = UI.Instance.fromXML(@"
    <round location='128' dragAble='*true' id='sun' radius='64'  color='yellow'>
        <round id='earth' radius='12'  px='128' color='green'>
            <round id='moon' radius='6' px='32' color='silver'>
            </round>
        </round>
    </round>
", false);
            var earth = sun.findByID("earth");
            var moon = sun.findByID("moon");

            timeID = TimerManager.get().setInterval(
                (i, t) =>
                {
                    earth.rotate(0.3f);
                    moon.rotate(0.2f);
                    UI.Instance.flush();
                }, 5);

            return sun;
        }

        public void lostAttach()
        {
            //throw new NotImplementedException();
            TimerManager.get().clearTimer(timeID);

            //重复ID， 清理
            UI.Instance.clearID("sun");
            UI.Instance.clearID("earth");
            UI.Instance.clearID("moon");
        }
    }
}

