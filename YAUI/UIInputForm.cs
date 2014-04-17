using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ns_YAUI
{
    public class UIInputForm : Form
    {
        private TextBox mTextBox;
        private System.ComponentModel.IContainer components;
        UIInputForm()
        {
            InitializeComponent();
        }

        public static UIInputForm Instance = new UIInputForm();

        void InitializeComponent()
        {
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mTextBox
            // 
            this.mTextBox.BackColor = System.Drawing.Color.White;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Font = new System.Drawing.Font("SimSun", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mTextBox.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.mTextBox.Location = new System.Drawing.Point(12, 15);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.Size = new System.Drawing.Size(366, 25);
            this.mTextBox.TabIndex = 1;
            this.mTextBox.Click += new System.EventHandler(this.mTextBox_Click);
            this.mTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyUp);
            //this.mTextBox.Leave += new System.EventHandler(this.mTextBox_Leave);
            this.Deactivate += new System.EventHandler(this.mTextBox_Leave);
            // 
            // InputForm
            // 
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(390, 52);
            this.ControlBox = false;
            this.Controls.Add(this.mTextBox);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InputForm";
            this.TransparencyKey = System.Drawing.Color.Maroon;
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SimpleCustomControl_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void SimpleCustomControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Pen blackPen = new Pen(Color.FromArgb(255, 0, 0, 0), 1);
            Pen greenPen = new Pen(Color.FromArgb(255, 200, 200, 200), 10);
            greenPen.Alignment = PenAlignment.Center;

            //g.DrawRectangle(greenPen, ClientRectangle);
        }

        private void mTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                show(false);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                show(false);
            }
        }

        string mTintText = "";
        public string tintText
        {
            set
            {
                mTintText = value;
            }
        }

        public void show(bool bShow, int x = 0, int y = 0)
        {
            if (bShow)
            {
                this.TopMost = true;
                mTextBox.Text = mTintText;
                uint c1 = 0x888888;
                mTextBox.ForeColor = Color.FromArgb((Int32)c1);
                this.Visible = true;
                this.Location = UIPainterForm.mIns.PointToScreen(new Point(x, y));
                if (evtInputEnter != null)
                    evtInputEnter();
            }
            else
            {
                this.TopMost = false;
                this.Visible = false;
                if(evtInputExit != null)
                    evtInputExit(this.mTextBox.Text);
            }
        }



        public void clear()
        {
            mTextBox.Text = "";
        }

        public Action evtInputEnter = null;
        public Action<string> evtInputExit = null;

        private void mTextBox_Click(object sender, EventArgs e)
        {
            uint c1 = 0xffffffff;
            mTextBox.Text = "";
            mTextBox.ForeColor = Color.FromArgb((Int32)c1);
        }

        private void InputForm_Load(object sender, EventArgs e)
        {

        }

        private void mTextBox_Leave(object sender, EventArgs e)
        {
            show(false);
            //this.Visible = false;
        }

    }
}
