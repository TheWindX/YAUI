using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_YAUI
{
    public static class ExpandPoint
    {
        public static Point transform(this Point _this, Matrix m)
        {
            var pts = new Point[] { _this };
            m.TransformPoints(pts);
            return pts[0];
        }
    }
}
