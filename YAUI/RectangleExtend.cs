using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace ns_behaviour
{
    public static class RectangleExtend
    {
        public static Point leftTop(this Rectangle rc)
        {
            return new Point(rc.Left, rc.Top);
        }

        public static Point leftMiddle(this Rectangle rc)
        {
            return new Point(rc.Left, rc.Top+rc.Height/2);
        }

        public static Point leftButtom(this Rectangle rc)
        {
            return new Point(rc.Left, rc.Bottom);
        }

        public static Point rightTop(this Rectangle rc)
        {
            return new Point(rc.Right, rc.Top);
        }

        public static Point rightMiddle(this Rectangle rc)
        {
            return new Point(rc.Right, rc.Top + rc.Height / 2);
        }

        public static Point rightButtom(this Rectangle rc)
        {
            return new Point(rc.Right, rc.Bottom);
        }

        public static Point middleTop(this Rectangle rc)
        {
            return new Point(rc.Right-rc.Width/2, rc.Top);
        }

        public static Point middleButtom(this Rectangle rc)
        {
            return new Point(rc.Right - rc.Width / 2, rc.Bottom);
        }

        public static Point center(this Rectangle rc)
        {
            return new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
        }
    }
}
