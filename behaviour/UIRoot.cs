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
        public void init()
        {
            mRoot = new UIStub();
        }

        public void draw(Graphics g)
        {
            mRoot.doDraw(g);
        }

        public void testUIEvent(int x, int y, Func<UIWidget, Func<int, int, bool>> getAction)
        {
            UIWidget uiout;

            bool ret = mRoot.doTestPick(new Point(x, y), out uiout);
            if (ret)
            {
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

                    uiout = uiout.mParesent as UIWidget;
                }
            }
        }

        public void testLMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if(ui.evtOnLMDown != null)
                    return (x1, y1) => { return ui.evtOnLMDown(x1, y1); };
                return null;
            });
        }

        public void testLMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnLMUp != null)
                    return (x1, y1) => { return ui.evtOnLMUp(x1, y1); };
                return null;
            });
        }

        public void testRMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnRMDown != null)
                    return (x1, y1) => { return ui.evtOnRMDown(x1, y1); };
                return null;
            });
        }

        public void testRMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnRMUp != null)
                    return (x1, y1) => { return ui.evtOnRMUp(x1, y1); };
                return null;
            });
        }

        public void testMMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnMMDown != null)
                    return (x1, y1) => { return ui.evtOnMMDown(x1, y1); };
                return null;
            });
        }

        public void testMMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                if (ui.evtOnMMUp != null)
                    return (x1, y1) => { return ui.evtOnMMUp(x1, y1); };
                return null;
            });
        }
    }
}
