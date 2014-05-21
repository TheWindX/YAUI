using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ns_YAUI
{
    public class UITips : UIStub
    {
        const string XMLLayout = @"
<round_rect enable='false' shrink='true' paddingX='5' paddingY='2' >
    <label text='asdf' style='normal'></label>
</round_rect>
";
        UIRoundRect mRR = null;
        UILabel mText = null;
        internal UITips()
        {
            appendFromXML(XMLLayout);
            mRR = this.findByTag("round_rect") as UIRoundRect;
            mRR.fillColor = (uint)schemes.backgroundColor;
            mText = this.findByTag("round_rect/label") as UILabel;
        }

        public int size
        {
            get
            {
                return mText.textSize;
            }
            set
            {
                mText.textSize = value;
            }
        }

        public uint foreground
        {
            set
            {
                mText.textColor = value;
            }
        }

        public uint background
        {
            set
            {
                mRR.fillColor = value;
            }
        }

        public string text
        {
            set
            {
                mText.text = value;
            }
        }
    }
}
