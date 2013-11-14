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

        public PaintDriver mIns = null;
        private InputForm mTextEdit;

        public InputForm textEdit
        {
            get
            {
                return mTextEdit;
            }
        }

        public PaintDriver()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.mTextEdit = new ns_behaviour.InputForm();
            
            this.SuspendLayout();
            // 
            // mTimer
            // 
            this.mTimer.Enabled = true;
            this.mTimer.Interval = 10;
            this.mTimer.Tick += new System.EventHandler(this.onUpdate);
            // 
            // mTextEdit
            // 
            this.mTextEdit.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.mTextEdit.ClientSize = new System.Drawing.Size(433, 72);
            this.mTextEdit.ControlBox = false;
            this.mTextEdit.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mTextEdit.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.mTextEdit.Location = new System.Drawing.Point(52, 52);
            this.mTextEdit.Name = "mTestForm";
            this.mTextEdit.TransparencyKey = System.Drawing.Color.BlanchedAlmond;
            this.mTextEdit.Visible = false;
            // 
            // PaintDriver
            // 
            this.ClientSize = new System.Drawing.Size(585, 523);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PaintDriver";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.onLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PaintDriver_KeyDown);
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

        public delegate void EvtMouse(int x, int y);
        
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
