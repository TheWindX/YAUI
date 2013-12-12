using System;
using System.Drawing;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Threading;

namespace ns_GameViewer
{

    public class PaintDriver : Form
    {
        public static PaintDriver ins = null;
        private System.Windows.Forms.Timer mTimer;
        private System.ComponentModel.IContainer components;

        CSRepl mRepl = new CSRepl();

        public PaintDriver()
        {
            InitializeComponent();
            ins = this;
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
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PaintDriver_MouseClick);
            this.ResumeLayout(false);

        }

        internal void ResetTimerLoop(int time)
        {
            mTimer.Stop();
            mTimer.Interval = time;
            mTimer.Start();
        }

        void onLoad(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            mRepl.start();
            Visible = false;
            //ShowInTaskbar = false;
            ViewWorld.enter();
        }

        private void onUpdate(object sender, EventArgs e)
        {
            mRepl.runOnce();
            ViewWorld.update();
            if (ViewWorld.mActive)
                this.Refresh();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            ViewMap.render(e.Graphics);
        }

        private void PaintDriver_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ViewMap.PlayerGo(e.Location.X, e.Location.Y);
                    break;
                case MouseButtons.Right:
                    ViewMap.PlayerSkill(0, e.Location.X, e.Location.Y);
                    break;
                default:
                    break;
            }
        }

        private void PaintDriver_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode > Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int skillId = e.KeyCode - Keys.D0;
                Point p = PointToClient(MousePosition);
                ViewMap.PlayerSkill(skillId, p.X, p.Y);
            }
        }
    }
}
