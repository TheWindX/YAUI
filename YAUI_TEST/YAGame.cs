using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUI;

namespace ns_YAUIUser
{
    namespace ns_game
    {
        class world
        {
            public UIWidget mUI = null;
            const string XMLFrame = @"
<rect clip='false' enable='false' size='480,360'>
    <label name='ready' text='Get Ready' color='yellow' align='center' size='24' style='bold'></label>
    <label name='failed' text='Failed' color='red' align='center' size='24' style='bold'></label>
    <label name='win' text='You Win!' color='green' align='center' size='24' style='bold'></label>
</rect>
";
            role mRole = null;
            public world()
            {
                mUI = UI.Instance.fromXML(XMLFrame, false);
                mGetReady = mUI.findByPath("ready") as UILabel;
                mFailed = mUI.findByPath("failed") as UILabel;
                mWin = mUI.findByPath("win") as UILabel;
                mRole = new role();
                
            }

            UILabel mGetReady = null;
            UILabel mFailed = null;
            UILabel mWin = null;
            public void init()
            {
                //get ready
                mGetReady.visible = true;
                mFailed.visible = false;
                mWin.visible = false;

                Action<int, int> clickHander = null;
                clickHander = (x, y) =>
                    {
                        start();
                        UIRoot.Instance.evtLeftUp -= clickHander;        
                    };

                UIRoot.Instance.evtLeftUp += clickHander;

                mRole.exit();
            }

            public void start()
            {
                mRole.enter(mUI); 
            }

            public void enter()
            {
                init();
            }

            public void exit()
            {
            }

            public void dead()
            {
                
            }
        }

        class move
        {
            public UIWidget mUI = null;
            public void setNum()
            {
            }

            public move(int size)
            {
                mUI = new UIRect(size, size);
            }

            public void destory()
            {
                mUI.paresent = null;
                mUI = null;
            }

            public void setPosition(float x, float y)
            {
                mUI.px = x;
                mUI.py = y;
            }

            UILabel mLb = null;
            public void setNum(int num)
            {
                if (mLb == null)
                {
                    mLb = new UILabel(num.ToString());
                    mLb.paresent = mUI;
                }
                mLb.text = num.ToString();
            }

            public void setColor(uint col)
            {
                (mUI as UIRect).fillColor = col;
            }
        }

        class role : move
        {
            public role():base(24)
            {
            }

            public void enter(UIWidget p)
            {
                mUI.paresent = p;
                startControl();
            }

            void onKey(int kc, bool isC, bool isS)
            {
                    {
                        Console.WriteLine(kc);
                    };
            }

            void onLMD(int x, int y)
            {
                {
                    Console.WriteLine(x+", "+y);
                };
                mTap = true;
            }


            void reset()
            {
                mTap = false;
                mVelY = 0;
                timeID = -1;
                
                mUI.px = mConstPx;
                mUI.py = mConstPy;
            }


            float mConstPx = 100;
            float mConstPy = 100;
            float mConstAccY = 100;
            float mConstUpV = -110;

            bool mStarted = false;
            bool mTap = false;
            float mVelY = 0;
            int timeID = -1;
            

            public void onTime(uint det, uint last)
            {
                if (mTap)
                {
                    mVelY = mConstUpV;
                    mTap = false;
                }
                float t = (float)det/1000;
                mUI.py = mUI.py +(mVelY * t + 0.5f * t * t*mConstAccY);
                mVelY = mVelY + mConstAccY * t;
                Console.WriteLine(mVelY);
                //Console.WriteLine(last);
                mUI.setDirty(true);
            }

            public void startControl()
            {
                if (mStarted) return;
                reset();
                mStarted = true;
                UIRoot.Instance.evtLeftDown += onLMD;

                timeID = TimerManager.get().setInterval(onTime, 20);
            }
            public void stopControl()
            {
                if (!mStarted) return;
                reset();
                mStarted = false;

                UIRoot.Instance.evtOnKey -= onKey;
                UIRoot.Instance.evtLeftDown -= onLMD;
                TimerManager.get().clearTimer(timeID);
            }

            public void exit()
            {
                stopControl();
            }
        }



        class YAGame : iTestInstance
        {
            world mWorld;

            public YAGame()
            {
            }

            public ECategory category()
            {
                return ECategory.demo;
            }

            public string title()
            {
                return "GameDemo";
            }

            public string desc()
            {
                return "Yet Another GAME";
            }




            public ns_YAUI.UIWidget getAttach()
            {
                mWorld = new world();
                mWorld.enter();
                return mWorld.mUI;
            }

            public void lostAttach()
            {
                return;
            }
        }
    }
}
