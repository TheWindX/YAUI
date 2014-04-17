﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YAUI
{
    public class UI : Singleton<UI>
    {
        UIPainterForm mPainter = null;
        public UI init()
        {
            mPainter = new UIPainterForm();

            mPainter.evtInit += () =>
            {
                CSRepl.Instance.start();

                UIRoot.Instance.initXML()
                    .initEvt()
                    .initHandleDraw(mPainter.Invalidate)
                    .initHandleLog((s) => CSRepl.Instance.print(s))
                    .initHandleInputShow(UIInputForm.Instance.show);


                UIInputForm.Instance.evtInputExit += UIRoot.Instance.handleInputShow;

                mPainter.evtPaint += UIRoot.Instance.handleDraw;
                mPainter.evtLeftDown += UIRoot.Instance.handleLeftDown;
                mPainter.evtLeftUp += UIRoot.Instance.handleEvtLeftUp;
                mPainter.evtRightDown += UIRoot.Instance.handleEvtRightDown;
                mPainter.evtRightUp += UIRoot.Instance.handleEvtRightUp;
                mPainter.evtMove += UIRoot.Instance.handleEvtMove;
                mPainter.evtOnWheel += UIRoot.Instance.handleEvtWheel;
            };

            mPainter.evtUpdate += () =>
            {
                CSRepl.Instance.runOnce();
                UIRoot.Instance.updateTimer();
            };

            mPainter.Show();
            return this;
        }

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

        public UIWidget root
        {
            get
            {
                return UIRoot.Instance.root;
            }
        }

        public void run()
        {
            System.Windows.Forms.Application.Run();
        }

        //TODO, setting...
    }
}
