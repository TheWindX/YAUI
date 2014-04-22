
//from http://blog.csdn.net/leftxden/article/details/7789076 //C#获取鼠标光标处屏幕颜色

using System;
using System.Drawing;
using System.Runtime.InteropServices;



namespace ColorPicker
{
    /// <summary>
    /// win8下wpf程序测试成功
    /// </summary>
    public class CursorPointManager
    {
        #region 得到光标在屏幕上的位置
        [DllImport("user32")]
        private static extern bool GetCursorPos(out PointF lpPoint);

        [DllImport("user32")]
        public static extern short GetKeyState(int nVirtKey);
        /// <summary>
        /// 获取光标相对于显示器的位置
        /// </summary>
        /// <returns></returns>
        public static PointF GetCursorPosition()
        {
            PointF showPoint = new PointF();
            GetCursorPos(out showPoint);
            return showPoint;
        }

        public const int VK_LBUTTON = 0x01;
        public const int VK_MBUTTON = 0x04;
        public static bool GetMouseLeftDown()
        {
            int ks = GetKeyState(VK_MBUTTON) & 0x8000;
            if (ks > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

    }

    /// <summary>
    /// 获取当前光标处颜色，win8下wpf测试成功
    /// </summary>
    public class ColorPickerManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">鼠标相对于显示器的坐标X</param>
        /// <param name="y">鼠标相对于显示器的坐标Y</param>
        /// <returns></returns>
        public static Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel &

0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);

            return color;
        }

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
        IntPtr hdcDest, // 目标设备的句柄 
        int nXDest, // 目标对象的左上角的X坐标 
        int nYDest, // 目标对象的左上角的X坐标 
        int nWidth, // 目标对象的矩形的宽度 
        int nHeight, // 目标对象的矩形的长度 
        IntPtr hdcSrc, // 源设备的句柄 
        int nXSrc, // 源对象的左上角的X坐标 
        int nYSrc, // 源对象的左上角的X坐标 
        int dwRop // 光栅的操作值 
        );

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(
        string lpszDriver, // 驱动名称 
        string lpszDevice, // 设备名称 
        string lpszOutput, // 无用，可以设定位"NULL" 
        IntPtr lpInitData // 任意的打印机数据 
        );

        /// <summary>
        /// 获取指定窗口的设备场景
        /// </summary>
        /// <param name="hwnd">将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的DC</param>
        /// <returns>指定窗口的设备场景句柄，出错则为0</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// 释放由调用GetDC函数获取的指定设备场景
        /// </summary>
        /// <param name="hwnd">要释放的设备场景相关的窗口句柄</param>
        /// <param name="hdc">要释放的设备场景句柄</param>
        /// <returns>执行成功为1，否则为0</returns>
        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>
        /// 在指定的设备场景中取得一个像素的RGB值
        /// </summary>
        /// <param name="hdc">一个设备场景的句柄</param>
        /// <param name="nXPos">逻辑坐标中要检查的横坐标</param>
        /// <param name="nYPos">逻辑坐标中要检查的纵坐标</param>
        /// <returns>指定点的颜色</returns>
        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
    }
}