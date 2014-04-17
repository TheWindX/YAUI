/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: driver for paint and client IO
 * */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Threading;

using System.Runtime.InteropServices;


namespace ns_YAUI
{
    //public delegate void EvtPaint(Graphics g);
    using EvtMouse = Action<int, int>;
    using EvtPaint = Action<Graphics>;
    using EvtOnKey = Action<int, bool, bool>;
    using EvtOnWheel = Action<int>;
    //public EvtOnKey(int kc, bool isControl, bool isShift);
    public class UIPainterForm : Form
    {
        private System.Windows.Forms.Timer mTimer;
        private System.ComponentModel.IContainer components;

        public static UIPainterForm mIns = null;


        public UIPainterForm()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mTimer
            // 
            this.mTimer.Enabled = true;
            this.mTimer.Interval = 10;
            this.mTimer.Tick += new System.EventHandler(this.onUpdate);
            // 
            // UIPainterForm
            // 
            this.ClientSize = new System.Drawing.Size(578, 544);
            this.Name = "UIPainterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.onLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PaintDriver_KeyDown);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseWheel);
            this.ResumeLayout(false);

        }

        public void setSize(int w, int h)
        {
            this.ClientSize = new System.Drawing.Size(w, h);
        }

        public void setUpdateInterval(int time)
        {
            mTimer.Stop();
            mTimer.Interval = time;
            mTimer.Start();
        }

        public delegate void EvtInit();
        public EvtInit evtInit;
        void onLoad(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            Visible = false;
            //ShowInTaskbar = false;
            mIns = this;
            if (evtInit != null)
                evtInit();
        }

        public delegate void EvtUpdate();
        public EvtUpdate evtUpdate;


        private void onUpdate(object sender, EventArgs e)
        {
            if (evtUpdate != null)
                evtUpdate();
            //Invalidate();
        }

        private void updateDirty()
        {
            //mReflush = false;
            Invalidate();
        }


        //bool mReflush = true;
        public EvtPaint evtPaint;
        //private Bitmap m_bmpOffscreen;
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics gxOff; //Offscreen graphics
            //if (m_bmpOffscreen == null) //Bitmap for doublebuffering
            //{
            //    m_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);
            //}
            //else
            //{
            //    var sz = m_bmpOffscreen.Size;
            //    if (ClientSize.Width != sz.Width
            //        || ClientSize.Height != sz.Height)
            //    {
            //        m_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);
            //        UIRoot.Instance.root.setDirty();
            //    }
            //}
            //gxOff = Graphics.FromImage(m_bmpOffscreen);
            gxOff = e.Graphics;
            //gxOff.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (evtPaint != null)
                evtPaint(gxOff);
            //e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
            //mReflush = true;
        }



        //public delegate void EvtMouse(int x, int y);


        public EvtMouse evtLeftDown;
        public EvtMouse evtRightDown;
        public EvtMouse evtMidDown;
        private void PaintDriver_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (evtLeftDown != null)
                        evtLeftDown(e.X, e.Y);
                    break;
                case MouseButtons.Right:
                    if (evtRightDown != null)
                        evtRightDown(e.X, e.Y);
                    break;
                case MouseButtons.Middle:
                    if (evtMidDown != null)
                        evtMidDown(e.X, e.Y);
                    break;
                default:
                    break;
            }
        }

        public EvtMouse evtLeftUp;
        public EvtMouse evtRightUp;
        public EvtMouse evtMidUp;
        private void PaintDriver_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (evtLeftUp != null)
                        evtLeftUp(e.X, e.Y);
                    break;
                case MouseButtons.Right:
                    if (evtRightUp != null)
                        evtRightUp(e.X, e.Y);
                    break;
                case MouseButtons.Middle:
                    if (evtMidUp != null)
                        evtMidUp(e.X, e.Y);
                    break;
                default:
                    break;
            }
        }

        public EvtMouse evtMove;
        private void PaintDriver_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtMove != null)
                evtMove(e.X, e.Y);
        }


        public EvtOnKey evtOnKey;
        private void PaintDriver_KeyDown(object sender, KeyEventArgs e)
        {
            if (evtOnKey != null)
            {
                evtOnKey(e.KeyValue, e.Control, e.Shift);
            }
        }



        public EvtOnWheel evtOnWheel;
        public void PaintDriver_MouseWheel(object sender, MouseEventArgs e)
        {
            if (evtOnWheel != null)
            {
                evtOnWheel(e.Delta);
            }
        }

        public EvtMouse evtDClick;
        private void PaintDriver_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (evtDClick != null)
            {
                evtDClick(e.X, e.Y);
            }
        }

        public Point getMousePosition()
        {
            return PointToClient(Control.MousePosition);
        }

        public Keys getModify()
        {
            return Control.ModifierKeys;
        }
    }
}
