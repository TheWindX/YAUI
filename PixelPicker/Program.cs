using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;
using System.Windows.Forms;

namespace ns_pixelPicker
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
        [STAThread]
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
            //System.Drawing.Color oldCl = new System.Drawing.Color();
            ns_YAUtils.TimerManager.get().setInterval(
                t =>
                {
                    if (ColorPicker.CursorPointManager.GetMouseLeftDown() )
                    {
                        var pt = ColorPicker.CursorPointManager.GetCursorPosition();
                        var cl = ColorPicker.ColorPickerManager.GetColor(pt.X, pt.Y);
                        //if (oldCl != cl)
                        {
                            ns_YAUtils.CSRepl.Instance.print(string.Format("{0}(0x{1:x}) , at ({2}, {3})", cl, cl.ToArgb(), pt.X, pt.Y));
                          //  oldCl = cl;
                        }
                    }
                    //if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    //{ 
                    //}
                }, 100);
        }
    }
}
