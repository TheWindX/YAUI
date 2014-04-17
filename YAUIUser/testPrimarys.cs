using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;
namespace YAUIUser
{
    class testPrimarys : Singleton<testPrimarys> 
    {
        public testPrimarys()
        {
            UI.Instance.fromXML(@"
<stub dragAble='*true'>
    <rect color='red'></rect>
    <round color='green'></round>
    <round_rect color='blue'></round_rect>
    <triangle color='maroon'></triangle>
</stub>
            ");
        }
    }
}
