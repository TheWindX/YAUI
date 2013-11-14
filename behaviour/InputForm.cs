using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ns_behaviour
{
    public class InputForm : Form
    {
        private TextBox mTextBox;
        private System.ComponentModel.IContainer components;
        public InputForm()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mTextBox
            // 
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Font = new System.Drawing.Font("SimSun", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mTextBox.Location = new System.Drawing.Point(12, 12);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mTextBox.Size = new System.Drawing.Size(409, 28);
            this.mTextBox.TabIndex = 0;
            this.mTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyUp);
            // 
            // InputForm
            // 
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(435, 55);
            this.ControlBox = false;
            this.Controls.Add(this.mTextBox);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InputForm";
            this.TransparencyKey = this.BackColor;
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



            g.DrawRectangle(greenPen, ClientRectangle);
            //g.FillRectangle(Brushes.Yellow, ClientRectangle);
            
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

        public void show(bool bShow, int x = 0, int y = 0)
        {
            if (bShow)
            {
                this.TopMost = true;
                mTextBox.Text = "";
                this.Visible = true;
                this.Location = Globals.Instance.mPainter.PointToScreen(new Point(x, y));
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

    }
}
