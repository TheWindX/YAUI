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

        public override Type[] depends()
        {
            return new Type[]{};
        }
    }

    class B : InheriteBase
    {
        public void fo() { Console.WriteLine("fb"); }

        public override Type[] depends()
        {
            return new Type[] {typeof(A) };
        }
    }

    class C : InheriteBase
    {
        public void fo() { Console.WriteLine("fc"); }

        public override Type[] depends()
        {
            return new Type[] { typeof(A) };
        }
    }

    class D : InheriteBase
    {
        public void fo() { Console.WriteLine("fd"); }

        public override Type[] depends()
        {
            return new Type[] { typeof(B), typeof(C) };
        }
    }
    
    class testAll : Singleton<testAll>
    {
        public testAll()
        {
            var ains = new D();
            ains.cast<A>().fo();
            ains.cast<B>().fo();
            ains.cast<C>().fo();
            ains.cast<D>().fo();

            Pacakge root = new Pacakge();
            root.cast<PackageItem>().name = "root";
            root.addPackage("itemA");
            root.addPackage("itemB");
            root.addPackage("itemC");
            root.removePacage("itemB");
            Console.WriteLine(root.stringForm(0));
            
        }
    }
}
