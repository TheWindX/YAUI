using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notrify
{
    public class Singleton<T> where T:class, new()
    {
        static object iLObj = new object();
        static T mIns = null;

        public static T Instance
        {
            get
            {
                if (mIns == null)
                {
                    lock (iLObj)
                    {
                        if (mIns == null)
                            mIns = new T();
                    }
                }
                return mIns;
            }
        }

    }

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
            var repl = new ns_utils.CSRepl();
            repl.start();

            TimerProvide.Instance.updateTimer();
            ns_timerUtil.TimerManager.Init(TimerProvide.Instance.nowTimer);
            TLoop.Instance.evtUpdate += TimerProvide.Instance.updateTimer;
            TLoop.Instance.evtUpdate += ns_timerUtil.TimerManager.tickAll;
            TLoop.Instance.evtUpdate += repl.runOnce;
        }

        static void test1()
        {
            ns_timerUtil.TimerManager.get().setTimeout((t)=>{
                Console.WriteLine("time end!");
            }, 1000);
        }
    }
}
