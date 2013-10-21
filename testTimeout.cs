using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ctestTimeout
    {
        public static uint mTime = 0;
        public static uint mStart = 0;
        static bool init = false;
        static void updateTimer()
        {
            var tmp = (uint)(System.DateTime.Now.Ticks / 10000 << 4 >> 4);
            if (tmp > mTime)
                mTime = tmp;
            if (!init) { init = true; mStart = tmp; }
        }

        static uint nowTimer()
        {
            return mTime - mStart;
        }

        static void print(string info)
        {
            Console.WriteLine(info);
        }

        static void testTimeout(
            Action < Func < UInt32 > > init,
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout,
            Action tick,
            Action<string> writer,
            int timerNum,
            int rseed,
            int minTime,
            int maxTime)
            
        {
            //Action < Func < UInt32 > > init = timer2.TimerManager.Init;
            //Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer2.TimerManager.get().setTimeout;

            //Action tick = timer2.TimerManager.tickAll;
            //int minTime = 1000;
            //int maxTime = 5000;
            //int rseed = 123;
            int iterTime = timerNum;
            updateTimer();
            init(() => { return nowTimer(); });
            var settimeout = getsettimeout();

            Random r = new Random(rseed);

            int res = 0;
            bool resCal = true;
            for (int i = 0; i < iterTime; ++i)
            {
                int k = i;
                int id = 0;
                UInt32 rd = (UInt32)r.Next(minTime, maxTime);
                id = settimeout((t) => { if (resCal)res += id; else res -= id; resCal = !resCal; writer("timeout:" + id + ", res:" + res + ", rd:" + rd); }, rd);
            }

            bool exit = false;
            int count = 0;
            settimeout((t) => { exit = true; }, (uint)maxTime+1);
            for (; !exit; )
            {
                updateTimer();
                tick();
                //var n = nowTimer();
                //if (n >= 5000) break;
                count++;
            }

            Console.WriteLine("count:" + count);
            Console.WriteLine("res:" + res);
        }

        static void test1()
        {
            Action < Func < UInt32 > > init = timer1.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer1.TimerManager.get().setTimeout;
            Action tick = timer1.TimerManager.tickAll;
            Action<string> writer = (s) => { };//Console.WriteLine(s);

            int minTime = 1000;
            int maxTime = 5000;
            int rseed = 456;
            int timerNum = 111000;

            testTimeout(init, getsettimeout, tick, writer, timerNum, rseed, minTime, maxTime);
        }

        static void test2()
        {
            Action < Func < UInt32 > > init = timer2.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer2.TimerManager.get().setTimeout;
            Action tick = timer2.TimerManager.tickAll;
            Action<string> writer = (s) => { };//Console.WriteLine(s);

            int minTime = 1000;
            int maxTime = 5000;
            int rseed = 456;
            int timerNum = 111000;

            testTimeout(init, getsettimeout, tick, writer, timerNum, rseed, minTime, maxTime);
        }
        
        
        public static void test()
        {
            test2();
        }

    }
}
