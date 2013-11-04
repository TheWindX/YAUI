using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class UIStub : UIWidget
    {

        public UIStub()
        {
               
        }

        public override Rectangle rect
        {
            get
            {
                return new Rectangle(-1280, -1280, 1280*2, 1280*2);
            }
        }

        public override string typeName
        {
            get { return "stub"; }
        }

        public override bool testPick(Point pos)
        {
            return true;
        }

        internal override void onDraw(Graphics g) 
        {
        }
    }
}
