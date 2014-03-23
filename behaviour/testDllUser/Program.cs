using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testDllUser
{
    class Program
    {
        class testC : testDll.Class2.TestInterface
        {
            public int i1()
            {
                return 100;
            }
        };

        static void Main(string[] args)
        {
            Console.WriteLine((int)testDll.Class2.EA.a);
        }
    }
}
