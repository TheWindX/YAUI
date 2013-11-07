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

namespace ns_behaviour
{

    public class PaintDriver : Form
    {
        private System.Windows.Forms.Timer mTimer;
        private System.ComponentModel.IContainer components;

        
        public PaintDriver mIns = null;
        public PaintDriver()
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
            // PaintDriver
            // 
            this.ClientSize = new System.Drawing.Size(585, 523);
            this.Name = "PaintDriver";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.onLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PaintDriver_KeyDown);
            this.MouseDown  += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseMove);

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
            this.Invalidate();
        }

        public delegate void EvtPaint(Graphics g);
        public EvtPaint evtPaint;
        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (evtPaint != null)
                evtPaint(e.Graphics);
        }

        public delegate void EvtLeftDown(int x, int y);
        public delegate void EvtRightDown(int x, int y);
        public delegate void EvtMidDown(int x, int y);
        public EvtLeftDown evtLeftDown;
        public EvtRightDown evtRightDown;
        public EvtMidDown evtMidDown;
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
        public delegate void EvtLeftUp(int x, int y);
        public delegate void EvtRightUp(int x, int y);
        public delegate void EvtMidUp(int x, int y);
        public EvtLeftUp evtLeftUp;
        public EvtRightUp evtRightUp;
        public EvtMidUp evtMidUp;
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

        public delegate void EvtMove(int x, int y);
        public EvtMove evtMove;
        private void PaintDriver_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtMove != null)
                evtMove(e.X, e.Y);
        }

        public delegate void EvtOnKey(int kc);
        public EvtOnKey evtOnKey;
        private void PaintDriver_KeyDown(object sender, KeyEventArgs e)
        {
            if (evtOnKey != null)
            {
                evtOnKey(e.KeyValue);
            }
        }


        public delegate void EvtOnWheel(int delta);
        public EvtOnWheel evtOnWheel;
        public void PaintDriver_MouseWheel(object sender, MouseEventArgs e)
        {
            if (evtOnWheel != null)
            {
                evtOnWheel(e.Delta);
            }
        }
    }
}
