using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ctestInterval
    {
        public static uint mTime = 0;
        static void updateTimer()
        {
            var tmp = (uint)(System.DateTime.Now.Ticks / 10000 << 4 >> 4);
            if (tmp > mTime)
                mTime = tmp;
        }
        static uint nowTimer()
        {
            return mTime;
        }

        static void print(string info)
        {
            Console.WriteLine(info);
        }

        static void testInterval (
            Action < Func < UInt32 > > init,
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int> > getsetinterval,
            Action tick,
            Action<string> writer,
            int timerNum,
            int rseed,
            float tickTimeMin,
            float tickTimeMax,
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
            var setinterval = getsetinterval();

            Random r = new Random(rseed);

            int res = 0;
            bool resCal = true;
            for (int i = 0; i < iterTime; ++i)
            {
                int k = i;
                int id = 0;
                UInt32 rd = (UInt32)r.Next(minTime, maxTime);
                UInt32 ri = (UInt32)( (tickTimeMin+ tickTimeMax*(1-r.NextDouble()) ) *(double)rd);
                id = setinterval((t) => { if (resCal)res += id; else res -= id; resCal = !resCal; writer("interval:" + id + ", res:" + res + ", rd:" + rd); }, ri, (t) => { if (resCal)res += id; else res -= id; resCal = !resCal; writer("timeout:" + id + ", res:" + res + ", rd:" + rd); }, rd);
            }

            bool exit = false;
            int count = 0;
            setinterval(null, UInt32.MaxValue, (t) => { exit = true; }, (uint)maxTime + 1);
            for (; !exit; )
            {
                updateTimer();
                tick();
                count++;
            }

            Console.WriteLine("count:" + count);
            Console.WriteLine("res:" + res);
        }
        
        static void testSimple1()
        {
            Action<Func<UInt32>> init = timer1.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval = () => timer1.TimerManager.get().setInterval;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer1.TimerManager.get().setTimeout;
            Action tickAll = timer1.TimerManager.tickAll;
            Func<int, bool> clearTimer = timer1.TimerManager.get().clearTimer;
            updateTimer();
            
            init(nowTimer);
            var setInterval = getsetinterval();
            var setTimeout = getsettimeout();


            int id1 = setInterval((n) => { Console.WriteLine("id 1 interval:" + n); }, 100, null, uint.MaxValue);

            int id2 = setTimeout((n) => { Console.WriteLine("id 2 timeout:" + n); clearTimer(id1); }, 2000);

            bool exit = false;
            int id3 = setInterval((n) => { Console.WriteLine("id3 interval:" + n); }, 200, (n) => { exit = true; }, 3000);

            while (!exit)
            {
                updateTimer();
                tickAll();
            }
        }

        static void testSimple2()
        {
            Action<Func<UInt32>> init = timer2.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval = () => timer2.TimerManager.get().setInterval;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer2.TimerManager.get().setTimeout;
            Action tickAll = timer2.TimerManager.tickAll;
            Func< Func<int, bool>> getclearTimer = ()=>timer2.TimerManager.get().clearTimer;
            updateTimer();

            init(nowTimer);
            var setInterval = getsetinterval();
            var setTimeout = getsettimeout();
            var clearTimer = getclearTimer();

            int id1 = setInterval((n) => { Console.WriteLine("id 1 interval:" + n); }, 100, null, uint.MaxValue);

            int id2 = setTimeout((n) => { Console.WriteLine("id 2 timeout:" + n); clearTimer(id1); }, 2000);

            bool exit = false;
            int id3 = setInterval((n) => { Console.WriteLine("id3 interval:" + n); }, 200, (n) => { exit = true; }, 3000);

            while (!exit)
            {
                updateTimer();
                tickAll();
            }
        }

        static void testCompex(
            Action<Func<UInt32>> init,
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval,
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout,
            Action tickAll,
            Func<Func<int, bool>> getclearTimer,
            Func< Func<int> > getTimerCount
            )
        {
            updateTimer();

            init(nowTimer);
            var setInterval = getsetinterval();
            var setTimeout = getsettimeout();
            var clearTimer = getclearTimer();


            int id0 = 0;
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;
            id0 = setInterval((n) =>
                {
                    //every frame
                }, 0, null, UInt32.MaxValue);

                id1 = setInterval((n) => 
                {
                    Console.WriteLine("id 1 interval:" + n);
                    int idx = 0;
                    idx = setInterval((x) =>
                    {
                        Console.WriteLine("id "+idx+", interval:" + x);
                        if (idx - 1 != id3)
                            clearTimer(idx - 1);
                    }, 100, null, 300);


                }, 100, null, uint.MaxValue);

            id2 = setTimeout((n) => { Console.WriteLine("id 2 timeout:" + n); clearTimer(id1); }, 2000);

            bool exit = false;
            id3 = setInterval((n) => { Console.WriteLine("id3 interval:" + n); }, 200, (n) => { exit = true; }, 3000);

            Console.WriteLine("timerCount:" + getTimerCount()());
            while (!exit && getTimerCount()() != 0 )
            {
                updateTimer();
                tickAll();
            }
            Console.WriteLine("timerCount:" + getTimerCount()());
        }

        static void testCompex1()
        {
            Action<Func<UInt32>> init = timer1.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval = () => timer1.TimerManager.get().setInterval;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer1.TimerManager.get().setTimeout;
            Action tickAll = timer1.TimerManager.tickAll;
            Func<Func<int, bool>> getclearTimer = () => timer1.TimerManager.get().clearTimer;
            Func<Func<int>> getTimerCount = ()=>timer1.TimerManager.get().timerCount;

            testCompex(init, getsetinterval, getsettimeout, tickAll, getclearTimer, getTimerCount);
        }

        static void testCompex2()
        {
            Action<Func<UInt32>> init = timer2.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval = () => timer2.TimerManager.get().setInterval;
            Func<Func<Action<UInt32>, UInt32, int>> getsettimeout = () => timer2.TimerManager.get().setTimeout;
            Action tickAll = timer2.TimerManager.tickAll;
            Func<Func<int, bool>> getclearTimer = () => timer2.TimerManager.get().clearTimer;
            Func<Func<int>> getTimerCount = () => timer2.TimerManager.get().timerCount;

            testCompex(init, getsetinterval, getsettimeout, tickAll, getclearTimer, getTimerCount);
        }

        static void test1()
        {
            Action < Func < UInt32 > > init = timer1.TimerManager.Init;
            Func< Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int> > getsetinterval = ()=>timer1.TimerManager.get().setInterval;
            Action tick = timer1.TimerManager.tickAll;
            Action<string> writer = (s) => { };//Console.WriteLine(s);
            //Action<string> writer = (s) => Console.WriteLine(s);

            int minTime = 1000;
            int maxTime = 5000;
            float minInterval = 0.05f;
            float maxInterval = 0.4f;
            int rseed = 456;
            int timerNum = 2220;

            testInterval(init, getsetinterval, tick, writer, timerNum, rseed, minInterval, maxInterval, minTime, maxTime);
        }

        static void test2()
        {
            Action<Func<UInt32>> init = timer2.TimerManager.Init;
            Func<Func<Action<UInt32>, UInt32, Action<UInt32>, UInt32, int>> getsetinterval = () => timer2.TimerManager.get().setInterval;
            Action tick = timer2.TimerManager.tickAll;
            Action<string> writer = (s) => { };//Console.WriteLine(s);
            //Action<string> writer = (s) => Console.WriteLine(s);

            int minTime = 1000;
            int maxTime = 5000;
            float minInterval = 0.05f;
            float maxInterval = 0.4f;
            int rseed = 456;
            int timerNum = 2220;

            testInterval(init, getsetinterval, tick, writer, timerNum, rseed, minInterval, maxInterval, minTime, maxTime);
        }
        
        
        public static void test()
        {
            testCompex2();
        }

    }
}
