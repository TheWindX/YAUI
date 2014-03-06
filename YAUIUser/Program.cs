using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        .initHandleLog((s) => CSRepl.Instance.print(s))
                        .initHandleInputShow(InputForm.Instance.show);
                        
                    p.evtPaint += UIRoot.Instance.handleDraw;
                    p.evtLeftDown += UIRoot.Instance.handleLeftDown;
                    p.evtLeftUp += UIRoot.Instance.handleEvtLeftUp;
                    p.evtMove += UIRoot.Instance.handleEvtMove;
                    p.evtOnWheel += UIRoot.Instance.handleEvtWheel;

                    UIRoot.Instance.root.appendFromXML(
@"
    <rect dragAble=""true"" scaleAble=""true"" width=""256"" height=""256"" layout=""horizen"" paddingX=""4"" paddingY=""4"">
        <innerTemplate name=""t1"">
            <rect width=""64"" height=""64"" marginX=""2"" marginY=""2"" layout=""horizen"">
                <rect name=""r1"" width=""20"" height=""20"" marginX=""2"" marginY=""2"">
                </rect>
            </rect>
        </innerTemplate>
        <apply innerTemplate=""t1"" height=""128"" r2.height=""55"">
               <rect name=""r2"" width=""30"" height=""20"" marginX=""2"" marginY=""2"">
               </rect>
        </apply>
        <apply innerTemplate=""t1"" height=""64""></apply>
        <apply innerTemplate=""t1"" height=""32""></apply>
    </rect>
");

                };

            p.evtUpdate += () =>
                {
                    ns_YAUI.CSRepl.Instance.runOnce();
                };

            p.Show();
            System.Windows.Forms.Application.Run();
        }
    }
}
