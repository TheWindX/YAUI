﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YAUIUser
{
    using ns_YAUI;
    class Program
    {
        static void Main(string[] args)
        {
            UI.Instance.init().setAntiAliasing(true);
            var tall = testFrame.Instance;
            UI.Instance.run();
        }
    }
}
