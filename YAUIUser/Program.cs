using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace YAUIUser
{
    using ns_YAUI;
    class Program
    {
        static void Main(string[] args)
        {
            var p = new ns_YAUI.PaintDriver();
            

            p.evtInit += () =>
                {
                    CSRepl.Instance.start();

                    UIRoot.Instance.initXML()
                        .initEvt()
                        .initHandleDraw(p.Invalidate)
                        .initHandleLog((s) => CSRepl.Instance.print(s))
                        .initHandleInputShow(InputForm.Instance.show);


                    InputForm.Instance.evtInputExit += UIRoot.Instance.handleInputShow;
    
                    p.evtPaint += UIRoot.Instance.handleDraw;
                    p.evtLeftDown += UIRoot.Instance.handleLeftDown;
                    p.evtLeftUp += UIRoot.Instance.handleEvtLeftUp;
                    p.evtMove += UIRoot.Instance.handleEvtMove;
                    p.evtOnWheel += UIRoot.Instance.handleEvtWheel;


                    var uiTest = testAll.Instance;

                };

            p.evtUpdate += () =>
                {
                    CSRepl.Instance.runOnce();
                };

            p.Show();
            System.Windows.Forms.Application.Run();
        }
    }
}
