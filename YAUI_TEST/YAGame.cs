using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ns_YAUI;

namespace ns_YAUIUser
{
    namespace ns_game
    {
        class bonusBlock
        {
            public List<bonus> mBonus = new List<bonus>();


            internal bonus addNewBonus()
            {
                bonus b = new bonus();
                mBonus.Add(b);
                return b;
            }
        }

        class world
        {
            System.Random mRandGen = new System.Random();
            int randRange(int max, int min = 0)
            {
                int r = mRandGen.Next() % (max - min + 1);
                return min + r;
            }


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
                mRole = new role(this);
            }

            UILabel mGetReady = null;
            UILabel mFailed = null;
            UILabel mWin = null;

            enum EState
            {
                invalid,
                ready,
                gaming,
                fail,
                youwin,
            }

            EState mState = EState.invalid;
            public void init()
            {
                mState = EState.ready;
                showInit();
                
                Action<int, int> clickHander = null;
                clickHander = (x, y) =>
                    {
                        start();
                        UIRoot.Instance.evtLeftUp -= clickHander;        
                    };

                UIRoot.Instance.evtLeftUp += clickHander;

                mState = EState.ready;
            }

            private void showInit()
            {
                //get ready
                mGetReady.visible = true;
                mFailed.visible = false;
                mWin.visible = false;
                mRole.hide();
            }

            int timeIDGenBonus = -1;
            public void start()
            {
                Console.WriteLine("world.start");
                mState = EState.gaming;
                mRole.show();
                mRole.starFly();

                timeIDGenBonus = TimerManager.get().setInterval(onTimerGenBonus, 300);
            }

            int mConstBonusBlockMax = 5;
            void onTimerGenBonus(uint det, uint last)
            {
                if (mBonus.Count == 0)
                {
                    for (int i = 0; i < mConstBonusBlockMax; ++i)
                    {
                        addBonus();
                    }
                }
                else
                {
                    while (getLastBonusX() < 600)
                    {
                        addBonus();
                    }
                    while (getFirstBonusX() < -100)
                    {
                        removeBonus();
                    }
                }
            }

            public void exit()
            {
            }

            public void dead()
            {
                
            }

            Queue<bonusBlock> mBonus = new Queue<bonusBlock>();

            float mConstBonusMaxY= 300;
            float mConstBonusMinY = 50;

            float mConstBonusStartPosX = -100;
            float mConstBonusIntervalX = 200;

            float getLastBonusX()
            {
                if (mBonus.Count == 0) return mConstBonusStartPosX;
                else
                {
                    var last = mBonus.Last();
                    return last.mBonus[0].mUI.px;
                }
            }

            float getFirstBonusX()
            {
                if (mBonus.Count == 0) return mConstBonusStartPosX;
                else
                {
                    var first = mBonus.First();
                    return first.mBonus[0].mUI.px;
                }
            }


            void addBonus()
            {
                Console.WriteLine("add bonus");
                var bb = new bonusBlock();
                var num = randRange(2, 1);

                float xpos = (int)getLastBonusX();
                Console.WriteLine("xpos:"+xpos);
                for (int i = 0; i < num; ++i)
                {
                    var b = bb.addNewBonus();
                    b.mUI.paresent = mUI;
                    b.mUI.px = xpos+mConstBonusIntervalX;
                    Console.WriteLine(b.mUI.px);
                    b.mUI.py = randRange((int)mConstBonusMaxY, (int)mConstBonusMinY);
                }

                mBonus.Enqueue(bb);
            }

            void removeBonus()
            {
                Console.WriteLine("removeBonus");
                var bb = mBonus.Dequeue();

                for (int i = 0; i < bb.mBonus.Count; ++i)
                {
                    var b = bb.mBonus[i];
                    b.mUI.paresent = null;
                }
            }

            internal IEnumerable<move> getAllCollids()
            {
                List<move> ret = new List<move>();
                foreach(var elem in mBonus)
                {
                    var bs = elem.mBonus;
                    foreach (var belem in bs)
                    {
                        yield return belem;
                    }
                }
            }

            //向前飞，所有物体后行
            float mConstVelX = 50;
            float mVelX = 50;
            internal void moveForward(uint det, uint last)
            {
                var moves = getAllCollids();
                foreach (var elem in moves)
                {
                    elem.mUI.px = elem.mUI.px - mVelX * det/1000f;
                }
            }

            //结束画面
            internal static void showEnd()
            {
                throw new NotImplementedException();
            }
        }

        class move
        {
            public UIWidget mUI = null;
            
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

            public int getNum(int num = 2)
            {
                if (mLb == null)
                {
                    mLb = new UILabel(num.ToString());
                    mLb.paresent = mUI;
                }
                return int.Parse(mLb.text);
            }



            public void setColor(uint col)
            {
                (mUI as UIRect).fillColor = col;
            }

            internal bool hasCollid(move elem)
            {
                RectangleF r1 = elem.mUI.drawRect.transform(elem.mUI.getLocalMatrix());
                RectangleF r2 = mUI.drawRect.transform(mUI.getLocalMatrix());

                if (r2.Contains(r1.leftTop() )) return true;
                else if (r2.Contains(r1.rightBottom() )) return true;
                else if (r1.Contains(r2.leftTop() ) ) return true;
                return false;
            }
        }

        class bonus : move
        {
            public bonus()
                : base(32)
            {
            }
        }

        class bonusNumber : bonus
        {
            public bonusNumber(int num)
            {
                setNum(num);
            }
        }

        

        class role : move
        {
            public role(world w):base(24)
            {
                mWorld = w;
                mUI.paresent = w.mUI;
            }
            world mWorld = null;

            void onKey(int kc, bool isC, bool isS)
            {
                    {
                        Console.WriteLine(kc);
                    };
            }

            void onLMD(int x, int y)
            {
                flyUp();
            }


            void resetFly()
            {
                mTap = false;
                mVelY = 0;
                flyingTimeID = -1;
                
                mUI.px = mConstPx;
                mUI.py = mConstPy;
            }


            float mConstPx = 100;
            float mConstPy = 100;
            float mConstPyGround = 360;
            float mConstAccY = 100;
            float mConstUpV = -110;

            bool mStarted = false;
            bool mTap = false;
            bool mStart = false;
            float mVelY = 0;
            float mAccY = 100;
            int flyingTimeID = -1;


            public void flyUp()
            {
                mTap = true;
            }

            public void calcFly(uint det, uint last)
            {
                if (mTap)
                {
                    mVelY = mConstUpV;
                    mTap = false;
                }
                float t = (float)det / 1000;
                mUI.py = mUI.py + (mVelY * t + 0.5f * t * t * mAccY);
                mVelY = mVelY + mAccY * t;
            }

            public void onFlyingTime(uint det, uint last)
            {
                if (mState == EState.flying)
                {
                    mWorld.moveForward(det, last);
                    calcFly(det, last);
                    //Console.WriteLine(mVelY);
                    //Console.WriteLine(last);
                    move collider = null;
                    if (checkCollid(out collider))
                    {
                        bonus bns = (collider as bonus);
                        if (bns != null)
                        {
                            if (checkBonus(bns) == EBonusRst.same)
                            {
                                upgrade();
                            }
                            else if (checkBonus(bns) == EBonusRst.change)
                            {
                                falling();
                            }
                        }
                    }

                    if(checkIfIsGround() )
                    {
                        die();
                        world.showEnd();
                    }
                    mUI.setDirty(true);
                }
                else if (mState == EState.falling)
                {
                    calcFly(det, last);
                    if(checkIfIsGround() )
                    {
                        die();
                        world.showEnd();
                    }
                }
            }

            private void die()
            {
                mState = EState.dead;
                TimerManager.get().clearTimer(flyingTimeID);
                setColor((uint)EColorUtil.black);
            }

            private bool checkIfIsGround()
            {
                if (mUI.py > mConstPyGround) return true;
                return false;
            }

            private void falling()
            {
                mState = EState.falling;
                mVelY = mConstFallenVel;
                mAccY = 0;

                Console.WriteLine("falling");
            }

            //进级
            private void upgrade()
            {
                Console.WriteLine("upgrade");
            }

            private EBonusRst checkBonus(bonus bns)
            {
                int num = bns.getNum();
                if (num == this.getNum())
                    return EBonusRst.same;
                else
                    return EBonusRst.invalid;
            }

            enum EBonusRst
            {
                same,
                change,
                invalid,
            }

            bool checkCollid(out move collider)
            {
                var collids = mWorld.getAllCollids();
                bool isCollid = false;
                foreach (var elem in collids)
                {
                    isCollid = elem.hasCollid(this);
                    if (isCollid)
                    {
                        collider = elem;
                        return true;
                    }
                }
                collider = null;
                return false;
            }

            public void starFly()
            {
                resetFly();
                mState = EState.flying;
                UIRoot.Instance.evtLeftDown += onLMD;
                flyingTimeID = TimerManager.get().setInterval(onFlyingTime, 10);
            }

            //public void stopControl()
            //{
            //    if (!mStarted) return;
            //    resetFly();
            //    mStarted = false;

            //    UIRoot.Instance.evtOnKey -= onKey;
            //    UIRoot.Instance.evtLeftDown -= onLMD;
            //    TimerManager.get().clearTimer(flyingTimeID);
            //}

            enum EState
            {
                init,
                flying,
                falling,
                dead,
            }

            EState mState = EState.init;
            private float mConstFallenVel = 200;

            public void fallen()
            {
                mState = EState.falling;
                UIRoot.Instance.evtLeftDown -= onLMD;
            }

            internal void hide()
            {
                mUI.visible = false;
            }

            internal void show()
            {
                mUI.visible = true;
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
                mWorld.init();
                return mWorld.mUI;
            }

            public void lostAttach()
            {
                return;
            }
        }
    }
}
