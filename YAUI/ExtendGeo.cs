using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_YAUI
{
    public static class ExtendPoint
    {
        public static PointF transform(this PointF _this, Matrix m)
        {
            var pts = new PointF[] { _this };
            m.TransformPoints(pts);
            return pts[0];
        }

        public static PointF expand(this PointF _this, Matrix m)
        {
            var pts = new PointF[] { _this };
            m.TransformPoints(pts);
            return pts[0];
        }
    }

    public static class ExtendRectangle
    {
        public static PointF leftTop(this RectangleF rc)
        {
            return new PointF(rc.Left, rc.Top);
        }

        public static PointF leftMiddle(this RectangleF rc)
        {
            return new PointF(rc.Left, rc.Top + rc.Height / 2);
        }

        public static PointF leftBottom(this RectangleF rc)
        {
            return new PointF(rc.Left, rc.Bottom);
        }

        public static PointF rightTop(this RectangleF rc)
        {
            return new PointF(rc.Right, rc.Top);
        }

        public static PointF rightMiddle(this RectangleF rc)
        {
            return new PointF(rc.Right, rc.Top + rc.Height / 2);
        }

        public static PointF rightBottom(this RectangleF rc)
        {
            return new PointF(rc.Right, rc.Bottom);
        }

        public static PointF middleTop(this RectangleF rc)
        {
            return new PointF(rc.Right - rc.Width / 2, rc.Top);
        }

        public static PointF middleBottom(this RectangleF rc)
        {
            return new PointF(rc.Right - rc.Width / 2, rc.Bottom);
        }

        public static PointF center(this RectangleF rc)
        {
            return new PointF(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
        }

        static float min(float a, float b) { if (a < b)return a; else return b; }
        static float max(float a, float b) { if (a > b)return a; else return b; }

        public static RectangleF transform(this RectangleF rc, Matrix m)
        {
            PointF[] pts = new PointF[] { rc.leftTop(), rc.leftBottom(), rc.rightTop(), rc.rightBottom() };
            m.TransformPoints(pts);
            float left = min(min(min(pts[0].X, pts[1].X), pts[2].X), pts[3].X);
            float right = max(max(max(pts[0].X, pts[1].X), pts[2].X), pts[3].X);
            float top = min(min(min(pts[0].Y, pts[1].Y), pts[2].Y), pts[3].Y);
            float bottom = max(max(max(pts[0].Y, pts[1].Y), pts[2].Y), pts[3].Y);
            return new RectangleF(left, top, right - left, bottom - top);
        }

        public static RectangleF expand(this RectangleF rc, RectangleF rc1)
        {
            var left = min(rc.Left, rc1.Left);
            var right = max(rc.Right, rc1.Right);
            var top = min(rc.Top, rc1.Top);
            var bottom = max(rc.Bottom, rc1.Bottom);

            return new RectangleF(left, top, right - left, bottom - top);
        }

    }
}
