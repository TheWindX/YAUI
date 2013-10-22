using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timer1
{
    class TimerManager
    {
        class TimeContext : IComparable<TimeContext>
        {
            TimerManager mManager;
            public int mID = 0;
            Action<float> mIntevalHandler = null;
            UInt32 mStartTime;
            UInt32 mDuration;
            float mStartValue;
            float mEndValue;
            UInt32 mInterval;

            UInt32 mTimerCount;
            UInt32 mIntervalCount;
            UInt32 mIntervalStation;

            public bool mValid = true;

            UInt32 nextTime
            {
                get
                {
                    if (mIntervalCount < mTimerCount)
                    {
                        return mIntervalStation + mInterval;
                    }
                    else return mStartTime + mDuration;
                }
            }

            public int CompareTo(TimeContext other)
            {
                int i = (int)nextTime;
                if (nextTime < other.nextTime)
                {
                    return -1;
                }
                else if (nextTime == other.nextTime)
                {
                    if (mID < other.mID)
                    {
                        return -1;
                    }
                    else if (mID == other.mID)
                    {
                        return 0;
                    }
                    return 1;
                }
                else
                    return 1;
            }

            internal TimeContext(TimerManager manager, int id, Action<float> handler, float a, float b, UInt32 duration, UInt32 interval)
            {
                mManager = manager;
                mID = id;
                mIntevalHandler = handler;
                mStartTime = manager.mTimer();
                mDuration = duration;
                mStartValue = a;
                mEndValue = b;
                mInterval = interval;
                mTimerCount = 0;
                mIntervalCount = 0;
                mIntervalStation = mStartTime;

                mValid = true;
            }

            internal bool tick(UInt32 delta)
            {
                if (!mValid) return false;
                //Debug.Log("id:" + mID +", "+mDuration);
                mTimerCount = mManager.mTimer() - mStartTime;
                mIntervalCount = mManager.mTimer() - mIntervalStation;
                if (mIntervalCount >= mInterval || mTimerCount >= mDuration)
                {
                    mManager.mTimeOuts.Add(this);
                    float interp = (float)((double)mTimerCount / (double)mDuration);
                    ////Debug.Log("tick:" + mTimerCount+", "+mDuration);
                    if (interp >= 1) { interp = 1; mValid = false; }
                    //float interpValue = (mEndValue - mStartValue) * interp + mStartValue;

                    //if (mIntevalHandler != null)
                    //{
                    //    mIntevalHandler(interpValue);
                    //}
                    //mIntervalCount -= mInterval;
                }

                return mValid;
            }

            internal void trigger()
            {
                float interp = (float)((double)mTimerCount / (double)mDuration);
                ////Debug.Log("tick:" + mTimerCount+", "+mDuration);
                if (interp >= 1) { interp = 1; mValid = false; }
                float interpValue = (mEndValue - mStartValue) * interp + mStartValue;

                if (mIntevalHandler != null)
                {
                    mIntevalHandler(interpValue);
                }
                mIntervalCount -= mInterval;
                mIntervalStation += mInterval;
            }


        }

        Func<UInt32> mTimer;
        UInt32 mLast = 0;
        public TimerManager(Func<UInt32> t, int id)
        {
            mTimer = t;
            mLast = t();
            mID = id;
        }
        public int mID = int.MaxValue;
        bool mChanged = false;
        Dictionary<int, TimeContext> mID2Interpolation = new Dictionary<int, TimeContext>();
        List<TimeContext> mInterpolates = new List<TimeContext>();
        List<TimeContext> mTimeOuts = new List<TimeContext>();
        List<TimeContext> mPreInterpolates = new List<TimeContext>();
        void swap()
        {
            var tmp = mInterpolates;
            mInterpolates = mPreInterpolates;
            mPreInterpolates = tmp;
        }

        int mIDCount = 0;


        public int timerCount()
        {
            return mInterpolates.Count+mPreInterpolates.Count;
        }

        public int setInterpolates(Action<float> handler, float a, float b, UInt32 duration, UInt32 interval = 0)
        {

            var elem = new TimeContext(this, ++mIDCount, handler, a, b, duration, interval);
            mPreInterpolates.Add(elem);
            mID2Interpolation.Add(mIDCount, elem);
            //Console.WriteLine("add timer:" + mIDCount);
            //Debug.Log("add timer:" + mIDCount);
            mChanged = true;
            return mIDCount;
        }


        //timer as it in html5
        public int setTimeout(Action<UInt32> handler, UInt32 duration)
        {
            UInt32 start = mTimer();
            return setInterpolates((n) => { handler(mTimer() - start); }, 0, 1, duration, UInt32.MaxValue);
        }

        public int setInterval(Action<UInt32> intervalHandler, UInt32 interval, Action<UInt32> endHandler = null, UInt32 duration = UInt32.MaxValue)
        {
            UInt32 start = mTimer();
            Action<float> f = (n) =>
            {
                if (n == 1)
                {
                    if (endHandler != null)
                        endHandler(mTimer() - start);
                }
                else
                {
                    if (intervalHandler != null)
                        intervalHandler(mTimer() - start);
                }
            };
            return setInterpolates(f, 0, 1, duration, interval);
        }

        public bool clearTimer(int id)
        {
            TimeContext elem = null;
            if (mID2Interpolation.TryGetValue(id, out elem))
            {
                mID2Interpolation.Remove(id);
                elem.mValid = false;
                return true;
            }
            return false;
        }

        public void clearAll()
        {
            foreach (var elem in mInterpolates)
            {
                clearTimer(elem.mID);
            }

            foreach (var elem in mPreInterpolates)
            {
                clearTimer(elem.mID);
            }
        }


        public void tick()
        {
            UInt32 now = mTimer();
            UInt32 deltaTime = now - mLast;
            mLast = now;
            int count = mInterpolates.Count;
            foreach (var elem in mInterpolates)
            {
                bool valid = elem.tick(deltaTime);
                if (!valid)
                {
                    mChanged = true;
                    mID2Interpolation.Remove(elem.mID);
                }
            }

            mTimeOuts.Sort();
            foreach (var elem in mTimeOuts)
            {
                elem.trigger();
            }
            mTimeOuts.Clear();

            if (mChanged)
            {
                foreach (var elem in mInterpolates)
                {
                    if (elem.mValid)
                    {
                        mPreInterpolates.Add(elem);
                    }
                }

                foreach (var elem in mPreInterpolates)
                {
                    //Debug.Log("timer in list:" + elem.mID);
                }

                swap();
                mPreInterpolates.Clear();
            }
            mChanged = false;
        }

        static SortedDictionary<int, TimerManager> mID2Timer = new SortedDictionary<int, TimerManager>();
        static SortedDictionary<int, TimerManager> mPreID2Timer = new SortedDictionary<int, TimerManager>();
        static bool mIDTimerChanged = false;
        static Func<UInt32> mGlobalTimer;
        public static void Init(Func<UInt32> t)
        {
            mGlobalTimer = t;
        }

        public static TimerManager get(int level = 0)//DEFAULT
        {
            TimerManager mout;
            if (mID2Timer.TryGetValue(level, out mout))
            {
                return mout;
            }
            else
            {
                if (mPreID2Timer.TryGetValue(level, out mout))
                {
                    return mout;
                }

                var t = new TimerManager(mGlobalTimer, level);
                if (!mPreID2Timer.ContainsKey(level))
                    mPreID2Timer.Add(level, t);
                mIDTimerChanged = true;
                return t;
            }
        }

        public static void tickAll()
        {
            foreach (var elem in mID2Timer)
            {
                elem.Value.tick();
            }
            if (mIDTimerChanged)
            {
                foreach (var elem in mPreID2Timer)
                {
                    mID2Timer.Add(elem.Key, elem.Value);
                }
                mPreID2Timer.Clear();
                mIDTimerChanged = false;
            }
        }
    }
}