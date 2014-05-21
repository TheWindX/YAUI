/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: export for UI user
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            UIRoot.Instance.pushProperty(null, false);
            var ui = UIRoot.Instance.loadFromXML(strXML);
            if (ui == null)
            {
                UIRoot.Instance.popProperty();
                return null;//? exception
            }
            if(attachRoot)
            {
                ui.paresent = UIRoot.Instance.root;
            }
            UIRoot.Instance.popProperty();
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
                mPainter.evtDClick += UIRoot.Instance.handleLDClick;
                mPainter.evtMove += UIRoot.Instance.handleEvtMove;
                mPainter.evtOnWheel += UIRoot.Instance.handleEvtWheel;
                mPainter.evtOnKey += UIRoot.Instance.handleEvtKey;
            };

            mPainter.evtUpdate += () =>
            {
                CSRepl.Instance.runOnce();
                TimerProvide.Instance.updateTimer();
                UIRoot.Instance.updateTimer();
                if (UIRoot.Instance.mLayoutUpdate)
                {
                    UIRoot.Instance.root.adjustLayout();
                    UIRoot.Instance.mRenderUpdate = true;
                    UIRoot.Instance.mLayoutUpdate = false;
                }
                if (UIRoot.Instance.mRenderUpdate)
                {
                    UIRoot.Instance.dirtyRedraw();
                    UIRoot.Instance.mRenderUpdate = false;
                }
            };

            mPainter.Show();
            return this;
        }

        public void clearID(string id)
        {
            UIRoot.Instance.clearIDWidget(id);
        }

        //TODO, setting...
        public UI setAntiAliasing(bool enable)
        {
            mPainter.antiAliasing = enable;
            //mPainter.textAntiAliasing = enable;
            return this;
        }

        public void setWindowSize(int w, int h)
        {
            mPainter.Size = new Size(w, h);
            root.setDirty(true);
        }

        public void setUICenter(UIWidget uiInRoot)
        {
            var sz = mPainter.Size;
            var pt = root.invertTransform(new PointF(sz.Width/2, sz.Height/2));
            uiInRoot.center = pt;
        }

        public void input(int x, int y, Action<string> continuous, int length=128)
        {
            UIInputForm.Instance.show(true, x, y, length, 32);
            Action<string> handler = null;
            handler = delegate(string str)
                {
                    Console.WriteLine("input");
                    continuous(str);
                    UIInputForm.Instance.evtInputExit -= handler;
                };
            UIInputForm.Instance.evtInputExit += handler;
        }
        #endregion

        #region tips
        void moveTipHandle(int x, int y)
        {
            mTips.px = x + 10;
            mTips.py = y;
            root.setDirty(true);
        }

        bool mOpen = false;
        public UITips setTips(string text = null)
        {
            if(text != null)
            {
                mTips.text = text;
                mTips.paresent = root;
                if (!mOpen)
                {
                    mOpen = true;
                    UIRoot.Instance.evtMove += moveTipHandle;
                }
            }
            else
            {
                mTips.paresent = null;
                if (mOpen)
                {
                    mOpen = false;
                    UIRoot.Instance.evtMove -= moveTipHandle;
                }
            }
            return mTips;
        }

        public UITips getTip()
        {
            return mTips;
        }
        #endregion

        #region debug
        static UILabel mTitle = null;
        public void setTitle(string str)
        {
            if (mTitle == null)
            {
                string content = @"<label text=''></label>";
                mTitle = root.appendFromXML(string.Format(content, str)) as UILabel;
            }

            mTitle.text = str;
            flush();
        }
        #endregion
    }
}
