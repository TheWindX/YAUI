﻿/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace ns_behaviour
{
    class Globals : Singleton<Globals>
    {
        public PaintDriver mPainter;
        public CSRepl mRepl;

        public delegate void EvtOnInit();
        public EvtOnInit evtOnInit;

        static uint mTime = 0;
        static void updateTimer()
        {
            var tmp = (uint)(System.DateTime.Now.Ticks / 10000 << 4 >> 4);
            if (tmp > mTime)
                mTime = tmp;
        }
        static uint nowTimer()
        {
            //Console.WriteLine(mTime);
            return mTime;
        }

        public void init()
        {
            mPainter = new PaintDriver();
            mRepl = CSRepl.Instance();
            
            //repl
            //mPainter.evtInit += mRepl.start;
            mPainter.evtUpdate += mRepl.runOnce;

            updateTimer();
            TimerManager.Init(() => nowTimer());
            //timer
            mPainter.evtInit += () =>
                {
                    //updateTimer();
                    //TimerManager.Init(() => nowTimer());
                };
            mPainter.evtUpdate += () =>
                {
                    updateTimer();
                    TimerManager.tickAll();
                };

            if(evtOnInit != null)
                evtOnInit();

            Application.Run(Globals.Instance.mPainter);
        }
    }
}