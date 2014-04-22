using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using ns_YAUtils;

namespace ns_YAUI
{
    #region trival
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
    #endregion

    public class UI : Singleton<UI>
    {
        #region trival
        UIPainterForm mPainter = null;
        UITips mTips;
        #endregion

        #region methods
        public UIWidget fromXML(string strXML, bool attachRoot = true)
        {
            var ui = UIRoot.Instance.loadFromXML(strXML);
            if(ui == null)return null;//? exception
            if(attachRoot)
            {
                ui.paresent = UIRoot.Instance.root;
            }
            return ui;
        }

        public void flush()
        {
            if (mTips.paresent == root)
            {
                mTips.setDepthHead();
            }
            root.setDirty(true);
        }

        public UIWidget root
        {
            get
            {
                return UIRoot.Instance.root;
            }
        }

        public PointF getCursorPosition()
        {
            return new PointF(UIRoot.Instance.cursorX, UIRoot.Instance.cursorY);
        }

        public void run()
        {
            System.Windows.Forms.Application.Run();
        }
        #endregion

        #region init & setting
        public UI init()
        {
            mPainter = new UIPainterForm();

            mPainter.evtInit += () =>
            {
                //
                CSRepl.Instance.start();
                TimerManager.Init(TimerProvide.Instance.nowTimer);
                TimerProvide.Instance.updateTimer();

                //xml init
                UIRoot.Instance.initXML()
                    .initEvt()
                    .initHandleDraw(mPainter.Invalidate)
                    .initHandleLog((s) => CSRepl.Instance.print(s))
                    .initHandleInputShow(UIInputForm.Instance.show);

                //tip init
                mTips = new UITips();

                //event init
                UIInputForm.Instance.evtInputExit += UIRoot.Instance.handleInputShow;

                mPainter.evtPaint += UIRoot.Instance.handleDraw;
                mPainter.evtLeftDown += UIRoot.Instance.handleLeftDown;
                mPainter.evtLeftUp += UIRoot.Instance.handleEvtLeftUp;
                mPainter.evtRightDown += UIRoot.Instance.handleEvtRightDown;
                mPainter.evtRightUp += UIRoot.Instance.handleEvtRightUp;
                mPainter.evtMove += UIRoot.Instance.handleEvtMove;
                mPainter.evtOnWheel += UIRoot.Instance.handleEvtWheel;
                mPainter.evtOnKey += UIRoot.Instance.handleEvtKey;
            };

            mPainter.evtUpdate += () =>
            {
                CSRepl.Instance.runOnce();
                TimerProvide.Instance.updateTimer();
                TimerManager.tickAll();
                UIRoot.Instance.updateTimer();
            };

            mPainter.Show();
            return this;
        }
        //TODO, setting...
        public UI setAntiAliasing(bool enable)
        {
            mPainter.antiAliasing = enable;
            return this;
        }
        #endregion

        #region tips
        void moveTipHandle(int x, int y)
        {
            mTips.position = new PointF(x+10, y);
            root.setDirty(true);
        }
        public UITips setTips(string text = null)
        {
            if(text != null)
            {
                mTips.text = text;
                mTips.paresent = root;
                UIRoot.Instance.evtMove += moveTipHandle;
            }
            else
            {
                mTips.paresent = null;
                UIRoot.Instance.evtMove -= moveTipHandle;
            }
            return mTips;
        }

        public UITips getTip()
        {
            return mTips;
        }
        #endregion
    }
}
