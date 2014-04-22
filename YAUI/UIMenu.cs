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

            evtOnChar += (ui, kc, iC, iS) =>
            {
                //Console.WriteLine((int)kc);
                if (kc == 38)
                {
                    selectIndex(mSelectItem - 1);
                }
                else if (kc == 40)
                {
                    selectIndex(mSelectItem + 1);
                }
                return false;
            };
        }


        const string XMLItem =
@"
<rect strokeColor='0x00000000' clip='true'>
    <lable enable='false' align='center'></lable>
</rect>
";
        Dictionary<string, UIRect> mNameItems = new Dictionary<string, UIRect>();
        List<UIRect> mItems = new List<UIRect>();
        int mSelectItem = -1;
        public void addItem(string cmdName, Action act, Action selecAct = null)
        {
            var rc = UIRoot.Instance.loadFromXML(XMLItem) as UIRect;
            rc.attrs["name"] = cmdName;
            rc.attrs["action"] = act;
            rc.attrs["selectAction"] = selecAct;
            mItems.Add(rc);
            mNameItems.Add(cmdName, rc);
            rc.paresent = mRR;

            var lb = rc.findByTag("lable") as UILable;
            rc.fillColor = mBColor;
            rc.width = itemWidth;
            rc.height = itemHeight;
            lb.textColor = mFColor;
            lb.text = cmdName;
            rc.evtOnLMUp += (ui, x, y) =>
            {
                enterCurrent();
                return false;
            };

            rc.evtOnEnter += () =>
                {
                    int c = -1;
                    var item = mItems.First(elem =>
                        {
                            c++;
                            return elem == rc;
                        });
                    if (item != null)
                    {
                        selectIndex(c);
                    }
                };

            rc.evtOnExit += () =>
            {
                int c = -1;
                var item = mItems.First(elem =>
                {
                    c++;
                    return elem == rc;
                });
                if (item != null)
                {
                    selectIndex(-1);
                }
            };

            
            this.setDirty(true);
        }

        public void removeItem(string cmdName)
        {
            UIRect ret;
            if (mNameItems.TryGetValue(cmdName, out ret))
            {
                ret.evtOnLMUpClear();
                ret.paresent = null;
                mNameItems.Remove(cmdName);
                ret.attrs.Remove("name");
                ret.attrs.Remove("action");
                ret.attrs.Remove("selectAction");
                mItems.Remove(ret);
                this.setDirty(true);
            }
        }

        public string current()
        {
            if (mSelectItem >= 0 && mSelectItem < mItems.Count())
            {
                UIRect rc = mItems[mSelectItem];
                object oname = "";
                rc.attrs.TryGetValue("name", out oname);
                return (string)name;
            }
            return "";
        }

        public UIRect currentRect()
        {
            if (mSelectItem >= 0 && mSelectItem < mItems.Count())
            {
                UIRect rc = mItems[mSelectItem];
                return rc;
            }
            return null;
        }

        public void enterCurrent()
        {
            if (mSelectItem >= 0 && mSelectItem < mItems.Count())
            {
                UIRect rc = mItems[mSelectItem];
                object oaction = null;
                if (rc.attrs.TryGetValue("action", out oaction))
                {
                    ((Action)oaction)();
                }
            }
        }

        public bool setItem(int idx, bool select, out UIRect orc)
        {
            if (idx >= 0 && idx < mItems.Count())
            {
                UIRect rc = mItems[idx];
                if(select)
                    rc.fillColor = mSelectColor;
                else
                    rc.fillColor = mBColor;
                orc = rc;
                return true;
            }
            orc = null;
            return false;
        }

        public bool selectIndex(int idx)
        {
            UIRect rc;
            setItem(mSelectItem, false, out rc);
            mSelectItem = idx;
            var ret = setItem(mSelectItem, true, out rc);
            if (!ret) mSelectItem = -1;
            setDirty(true);
            return ret;
        }

        public bool selectName(string name)
        {
            int c = -1;
            mItems.First(elem=>
                {
                    c++;
                    object oname = null;
                    if(elem.attrs.TryGetValue("name", out oname) )
                    {
                        return (string)oname == name;
                    }
                    return false;
                });
            return selectIndex(c);
        }

        public bool selectFirst()
        {
            return selectIndex(0);
        }

        public bool selectNext()
        {
            return selectIndex(mSelectItem+1);
        }

        uint mFColor = (uint)EColorUtil.white;
        uint mBColor = (uint)EColorUtil.black;
        uint mSelectColor = (uint)EColorUtil.brown;
        public uint foreground
        {
            set
            {
                mFColor = value;
                foreach (UIRect item in mNameItems.Values)
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
                foreach (UIRect item in mNameItems.Values)
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
                foreach (UIRect item in mNameItems.Values)
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
                foreach (UIRect item in mNameItems.Values)
                {
                    item.height = mItemHeight;
                }
                this.setDirty(true);
            }
        }
    }
}
