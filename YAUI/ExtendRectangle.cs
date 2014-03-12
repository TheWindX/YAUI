using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_YAUI
{
    public static class ExtendRectangle
    {
        public static Point leftTop(this Rectangle rc)
        {
            return new Point(rc.Left, rc.Top);
        }

        public static Point leftMiddle(this Rectangle rc)
        {
            return new Point(rc.Left, rc.Top+rc.Height/2);
        }

        public static Point leftBottom(this Rectangle rc)
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

        public static Point rightBottom(this Rectangle rc)
        {
            return new Point(rc.Right, rc.Bottom);
        }

        public static Point middleTop(this Rectangle rc)
        {
            return new Point(rc.Right-rc.Width/2, rc.Top);
        }

        public static Point middleBottom(this Rectangle rc)
        {
            return new Point(rc.Right - rc.Width / 2, rc.Bottom);
        }

        public static Point center(this Rectangle rc)
        {
            return new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
        }

        static int min(int a, int b) { if (a < b)return a; else return b; }
        static int max(int a, int b) { if (a > b)return a; else return b; }

        public static Rectangle transform(this Rectangle rc, Matrix m)
        {
            Point[] pts = new Point[]{rc.leftTop(), rc.leftBottom(), rc.rightTop(), rc.rightBottom()};
            m.TransformPoints(pts);
            int left = min(min(min(pts[0].X, pts[1].X), pts[2].X), pts[3].X);
            int right = max(max(max(pts[0].X, pts[1].X), pts[2].X), pts[3].X);
            int top = min(min(min(pts[0].Y, pts[1].Y), pts[2].Y), pts[3].Y);
            int bottom = max(max(max(pts[0].Y, pts[1].Y), pts[2].Y), pts[3].Y);
            return new Rectangle(left, top, right - left, bottom - top);
        }

    }
}
