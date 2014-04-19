using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace ns_YAUIUser
{
    using ns_YAUI;

    class testUse : Singleton<testUse>
    {
        public testUse()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(@"
<a></b>
");
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.LineNumber);
                Console.WriteLine(e.LinePosition);
            }
        }
    }
}
