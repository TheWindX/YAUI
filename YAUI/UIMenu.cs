using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_YAUI
{
    public class UIMenu : UIStub
    {
        const string XMLLayout =
@"
<rect paddingX='5' paddingY='2' layout='vertical' shrink='true'>
</rect>
";
        UIRect mRR = null;
        public UIMenu()
        {
            mRR = appendFromXML(XMLLayout) as UIRect;
            mRR.fillColor = mBColor;
        }


        const string XMLItem =
@"
<rect strokeColor='0x00000000' clip='true'>
    <lable enable='false' align='center'></lable>
</rect>
";
        Dictionary<string, UIRect> mItems = new Dictionary<string, UIRect>();
        UIRect mSelect = null;
        public void addItem(string cmdName, Action act)
        {
            var rc = UIRoot.Instance.loadFromXML(XMLItem) as UIRect;
            mItems.Add(cmdName, rc);
            rc.paresent = mRR;

            var lb = rc.findByTag("lable") as UILable;
            rc.fillColor = mBColor;
            rc.width = itemWidth;
            rc.height = itemHeight;
            lb.textColor = mFColor;
            lb.text = cmdName;
            rc.evtOnLMUp += (ui, x, y) =>
            {
                act();
                return false;
            };

            rc.evtOnEnter += () =>
                {
                    if (mSelect != null)
                    {
                        mSelect.fillColor = mBColor;
                    }

                    mSelect = rc;
                    rc.fillColor = mSelectColor;
                    setDirty(true);
                };

            rc.evtOnExit += () =>
            {
                rc.fillColor = mBColor;
                setDirty(true);
            };

            this.setDirty(true);
        }

        public void removeItem(string cmdName)
        {
            UIRect ret;
            if (mItems.TryGetValue(cmdName, out ret))
            {
                ret.evtOnLMUpClear();
                ret.paresent = null;
                mItems.Remove(cmdName);
                this.setDirty(true);
            }
        }

        uint mFColor = (uint)EColorUtil.white;
        uint mBColor = (uint)EColorUtil.black;
        uint mSelectColor = (uint)EColorUtil.brown;
        public uint foreground
        {
            set
            {
                mFColor = value;
                foreach (UIRect item in mItems.Values)
                {
                    var lb = item.findByTag("rect/lable") as UILable;
                    lb.textColor = mFColor;
                }
                this.setDirty(true);
            }
        }

        public uint background
        {
            set
            {
                mBColor = value;
                mRR.fillColor = mBColor;
                foreach (UIRect item in mItems.Values)
                {
                    item.fillColor = mBColor;
                }
                this.setDirty(true);
            }
        }

        public uint selectColor
        {
            set
            {
                mSelectColor = value;
            }
        }

        int mItemWidth = 128;
        public int itemWidth
        {
            get
            {
                return mItemWidth;
            }
            set
            {
                mItemWidth = value;
                foreach (UIRect item in mItems.Values)
                {
                    item.width = mItemWidth;
                }
                this.setDirty(true);
            }
        }

        int mItemHeight = 24;
        public int itemHeight
        {
            get
            {
                return mItemHeight;
            }
            set
            {
                mItemHeight = value;
                foreach (UIRect item in mItems.Values)
                {
                    item.height = mItemHeight;
                }
                this.setDirty(true);
            }
        }


    }
}
