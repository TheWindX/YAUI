using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace ns_YAUIUser
{
    class testTips : Singleton<testTips>
    {
        public testTips()
        {
            UI.Instance.getTip().foreground = (uint)EColorUtil.red;
            UI.Instance.getTip().background = (uint)EColorUtil.black;
            UI.Instance.getTip().size = 20;
            var id = TimerManager.get().setInterval(t =>
                {
                    UI.Instance.setTips("leave time is:" + (15000 - t));
                    UI.Instance.flush();
                }, 500);
            TimerManager.get().setTimeout(t =>
                {
                    UI.Instance.setTips();
                    UI.Instance.flush();
                    TimerManager.get().clearTimer(id);
                }, 15000);

        }
    }
}
