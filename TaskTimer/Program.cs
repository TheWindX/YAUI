/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: MAIN
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ns_behaviour
{
    class Program
    {
        public static int Main()
        {
            string str = Application.ExecutablePath;
            var reg_Autorun_Key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            reg_Autorun_Key.SetValue("shut", '"' + str + '"');
            InitAll.main();
            testAll.main();
            Globals.Instance.init();
            return 0;
        }
    }
}
