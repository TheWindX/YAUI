using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ns_behaviour
{
    class UIEdit : UILable
    {
        UIRect mBackGround = new UIRect(128, 22, 0xffffffff, 0);
        int mMaxCharLength;
        public UIEdit(string t = "click to input", int maxCharLength = 16, uint color = 0xffffffff):base(t, 18, color)
        {
            mMaxCharLength = maxCharLength;
            this.evtOnLMDown += onClick;
            mBackGround.mPos = new Point(-2, -2);
            mBackGround.setParesent(this);
            mBackGround.depth = -1;
            mBackGround.width = (maxCharLength+1) * 18/2+2;
        }

        public override Rectangle rect
        {
            get
            {
                return mBackGround.rect;
            }
        }

        public bool onClick(UIWidget _this, int x, int y)
        {
            Globals.Instance.mPainter.textEdit.show(true, x, y);
            Globals.Instance.mPainter.textEdit.evtInputExit += (text) =>
                {
                    if (mMaxCharLength < text.Length)
                        this.text = text.Substring(0, mMaxCharLength);
                    else
                        this.text = text;
                };
            return true;
        }
    }
}
