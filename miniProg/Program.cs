using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YAMini
{   
    class TimerProvide : Singleton<TimerProvide>
    {
        uint mTime;
        public void updateTimer()
        {
            var tmp = (uint)(System.DateTime.Now.Ticks / 10000 << 4 >> 4);
            if (tmp > mTime)
                mTime = tmp;
        }

        public uint nowTimer()
        {
            return mTime;
        }
    };

    class TLoop : Singleton<TLoop>
    {
        public void update()
        {
            if (evtUpdate != null)
                evtUpdate();
        }

        public bool exit = false;

        public Action evtUpdate;
    };

    class Program
    {
        static void Main(string[] args)
        {
            initAll();
            while (TLoop.Instance.exit == false)
            {
                TLoop.Instance.update();
            }
        }

        static void initAll()
        {
            init();
            test1();
        }

        static void init()
        {
            var repl = ns_YAUtils.CSRepl.Instance;
            repl.start();

            TimerProvide.Instance.updateTimer();
            ns_YAUtils.TimerManager.Init(TimerProvide.Instance.nowTimer);
            TLoop.Instance.evtUpdate += TimerProvide.Instance.updateTimer;
            TLoop.Instance.evtUpdate += ns_YAUtils.TimerManager.tickAll;
            TLoop.Instance.evtUpdate += repl.runOnce;
        }

        static void test1()
        {
            ns_YAUtils.TimerManager.get().setTimeout((t) =>
            {
                CSRepl.Instance.print("time1 is end");
            }, 1000);

            ns_YAUtils.TimerManager.get().setInterval( (i,t)=>CSRepl.Instance.print("time2 in intervals at "+t), 500, 
                t=>CSRepl.Instance.print("time2 is end at "+t), 3001);
        }
    }
}
