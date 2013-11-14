/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ns_behaviour
{
    class UIRoot : Singleton<UIRoot>
    {
        public UIWidget mRoot;

        public UIRoot()
        {
            Globals.Instance.evtOnInit += init;
        }

        void init()
        {
            mRoot = new UIStub();

            Globals.Instance.mPainter.evtPaint += (g) =>
            {
                UIRoot.Instance.draw(g);
            };
            Globals.Instance.mPainter.evtLeftDown += (x, y) =>
            {
                UIRoot.Instance.testLMD(x, y);
            };
            Globals.Instance.mPainter.evtLeftUp += (x, y) =>
            {
                UIRoot.Instance.testLMU(x, y);
            };
            Globals.Instance.mPainter.evtRightDown += (x, y) =>
            {
                UIRoot.Instance.testRMD(x, y);
            };
            Globals.Instance.mPainter.evtRightUp += (x, y) =>
            {
                UIRoot.Instance.testRMU(x, y);
            };
            Globals.Instance.mPainter.evtMidDown += (x, y) =>
            {
                UIRoot.Instance.testMMD(x, y);
            };
            Globals.Instance.mPainter.evtMidUp += (x, y) =>
            {
                UIRoot.Instance.testMMU(x, y);
            };
        }

        public void draw(Graphics g)
        {
            mRoot.doDraw(g);
        }

        void onKeyLock(int kc)
        {
            if (kc == (int)System.Windows.Forms.Keys.Space)
            {
                if (lockWidget)
                {
                    lockWidget = false;
                }
                else
                {
                    lockWidget = true;
                }
            }
        }
        bool mLockable = false;
        void setLock()
        {
            if (mLockable == true) return;
            mLockable = true;
            Globals.Instance.mPainter.evtOnKey += onKeyLock;
        }

        void unsetLock()
        {
            if (mLockable == false) return;
            mLockable = false;
            Globals.Instance.mPainter.evtOnKey -= onKeyLock;
        }

        public bool lockable
        {
            get
            {
                return mLockable;
            }
            set
            {
                if (value)
                    setLock();
                else
                    unsetLock();
            }
        }

        public UIWidget focusWidget
        {
            get
            {
                object o;
                bool ret = mRoot.attrs.TryGetValue("focus", out o);
                if (!ret) return null;
                return o as UIWidget;
            }
            set
            {
                mRoot.attrs["focus"] = value;
            }
        }

        public UIWidget currentWidget
        {
            get
            {
                object o;
                bool ret = mRoot.attrs.TryGetValue("current", out o);
                if (!ret) return null;
                return o as UIWidget;
            }
            set
            {
                mRoot.attrs["current"] = value;
            }
        }

        public bool lockWidget
        {
            get
            {
                return (focusWidget != null);
            }
            set
            {
                if (value)
                    focusWidget = currentWidget;
                else
                    focusWidget = null;
            }
            
        }

        public void testUIEvent(int x, int y, Func<UIWidget, Func<int, int, bool>> getAction)
        {
            UIWidget uiout;
            bool ret = true;
            uiout = focusWidget;
            if(uiout == null)
                ret  = mRoot.doTestPick(new Point(x, y), out uiout);
            if (ret)
            {
                currentWidget = uiout;
                while (uiout != null)
                {
                    var act = getAction(uiout);
                    if (act != null)
                    {
                        if (!act(x, y) )
                        {
                            break;//consumed
                        }
                    }
                    if (!uiout.propagate)
                    {
                        break;
                    }

                    uiout = uiout.paresent as UIWidget;
                }
            }
        }

        public void testLMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if(ui.evtOnLMDown != null)
                    return (x1, y1) => { return ui.evtOnLMDown(ui, x1, y1); };
                return null;
            });
        }

        public void testLMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnLMUp != null)
                    return (x1, y1) => { return ui.evtOnLMUp(ui, x1, y1); };
                return null;
            });
        }

        public void testRMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnRMDown != null)
                    return (x1, y1) => { return ui.evtOnRMDown(ui, x1, y1); };
                return null;
            });
        }

        public void testRMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnRMUp != null)
                    return (x1, y1) => { return ui.evtOnRMUp(ui, x1, y1); };
                return null;
            });
        }

        public void testMMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnMMDown != null)
                    return (x1, y1) => { return ui.evtOnMMDown(ui, x1, y1); };
                return null;
            });
        }

        public void testMMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnMMUp != null)
                    return (x1, y1) => { return ui.evtOnMMUp(ui, x1, y1); };
                return null;
            });
        }
    }
}
