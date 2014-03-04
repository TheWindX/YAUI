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




namespace ns_behaviour
{

    public class PaintDriver : Form
    {
        private System.Windows.Forms.Timer mTimer;
        private System.ComponentModel.IContainer components;
        public NotifyIcon shut;

        static public PaintDriver mIns = null;

        
        public PaintDriver()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
        }

        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaintDriver));
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.shut = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // mTimer
            // 
            this.mTimer.Enabled = true;
            this.mTimer.Tick += new System.EventHandler(this.onUpdate);
            // 
            // shut
            // 
            this.shut.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.shut.Icon = ((System.Drawing.Icon)(resources.GetObject("shut.Icon")));
            this.shut.Tag = "123";
            this.shut.Text = "shut";
            this.shut.Visible = true;
            this.shut.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // PaintDriver
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(330, 441);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PaintDriver";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TransparencyKey = System.Drawing.Color.Black;
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
        public event EvtInit evtInit;
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
        public event EvtUpdate evtUpdate;

        private void onUpdate(object sender, EventArgs e)
        {
            if (evtUpdate != null)
                evtUpdate();
            this.Invalidate();
        }

        public delegate void EvtPaint(Graphics g);
        public event EvtPaint evtPaint;
        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (evtPaint != null)
                evtPaint(e.Graphics);
        }

        public delegate void EvtMouse(int x, int y);
        
        public event EvtMouse evtLeftDown;
        public event EvtMouse evtRightDown;
        public event EvtMouse evtMidDown;
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

        public event EvtMouse evtLeftUp;
        public event EvtMouse evtRightUp;
        public event EvtMouse evtMidUp;
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

        public event EvtMouse evtMove;
        private void PaintDriver_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtMove != null)
                evtMove(e.X, e.Y);
        }

        public delegate void EvtOnKey(int kc, bool isControl, bool isShift);
        public event EvtOnKey evtOnKey;
        private void PaintDriver_KeyDown(object sender, KeyEventArgs e)
        {
            if (evtOnKey != null)
            {
                evtOnKey(e.KeyValue, e.Control, e.Shift);
            }
        }


        public delegate void EvtOnWheel(int delta);
        public event EvtOnWheel evtOnWheel;
        public void PaintDriver_MouseWheel(object sender, MouseEventArgs e)
        {
            if (evtOnWheel != null)
            {
                evtOnWheel(e.Delta);
            }
        }

        public event EvtMouse evtDClick;
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
