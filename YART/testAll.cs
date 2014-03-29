using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YART
{
    class A : InheriteBase
    {
        public void fo() { Console.WriteLine("fa"); }

        public override Type[] inheritFrom()
        {
            return new Type[]{};
        }
    }

    class B : InheriteBase
    {
        public void fo() { Console.WriteLine("fb"); }

        public override Type[] inheritFrom()
        {
            return new Type[] {typeof(A) };
        }
    }

    class C : InheriteBase
    {
        public void fo() { Console.WriteLine("fc"); }

        public override Type[] inheritFrom()
        {
            return new Type[] { typeof(A) };
        }
    }

    class D : InheriteBase
    {
        public void fo() { Console.WriteLine("fd"); }

        public override Type[] inheritFrom()
        {
            return new Type[] { typeof(B), typeof(C) };
        }
    }
    
    class testAll : Singleton<testAll>
    {
        public testAll()
        {
            var _1 = testWindow.Instance;
        }
    }
}
