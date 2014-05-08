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
                int r = 0;// world.randRange(1, 0);
                bonus b = null;
                if (r == 0)
                {
                    b = new bonusNumber();
                }
                else
                {
                    //b = new bonusOp();
                }
                mBonus.Add(b);
                return b;
            }
        }

        class world
        {
            public const int ROW_BLOCKS = 4;
            public static System.Random mRandGen = new System.Random();
            public static int randRange(int max, int min = 0)
            {
                int r = mRandGen.Next() % (max - min + 1);
                return min + r;
            }

            public static List<int> randList(int max, int min = 0)
            {
                List<int> ls = new List<int>();
                int count = max - min + 1;
                for (int i = 0; i < count; ++i)
                {
                    ls.Add(min + i);
                }
                for (int i = 0; i < count; ++i)
                {
                    int r =randRange(count-1, 0);
                    int m = ls[i];
                    ls[i] = ls[r];
                    ls[r] = m;
                }
                return ls;
            }

            public UIWidget mRoot = null;
            public UIWidget mUIx = null;
            public UIWidget mUIMainRole = null;
            public UIWidget mUIRoles = null;

            const string XMLFrame = @"
<rect color='black' clip='true' enable='false' size='480,360'>
    <div name='roles'></div>
    <div name='mainRole'></div>
    <div name='forground' layout='expand'>
        <label name='prompt' text='Get Ready' color='gold' align='center' size='24' style='bold'></label>
        <label name='score' offset='24' color='green' align='leftTop' size='10' style='normal'></label>
    </div>
</rect>
";
            mainRole mMainRole = null;
            public world()
            {
                mRoot = UI.Instance.fromXML(XMLFrame, false);
                mUIMainRole = mRoot.findByPath("mainRole");
                mUIRoles = mRoot.findByPath("roles");
                mPrompt = mRoot.findByPath("forground/prompt") as UILabel;
                mScore = mRoot.findByPath("forground/score") as UILabel;
                mMainRole = new mainRole(this);
            }

            UILabel mPrompt = null;
            UILabel mScore = null;
            
            enum EState
            {
                invalid,
                ready,
                gaming,
                fail,
                youwin,
            }

            public void onClick(int x, int y)
            {
                start();
                UIRoot.Instance.evtLeftUp -= onClick;
            }

            EState mState = EState.invalid;
            public void init()
            {
                clean();
                mMainRole.init();
                mMainRole.hide();
                
                mState = EState.ready;
                showColorHint("get ready?");
                
                UIRoot.Instance.evtLeftUp += onClick;
                mState = EState.ready;
                mRoot.setDirty(true);
            }

            int timeIDGenBonus = -1;
            public void start()
            {
                mState = EState.gaming;
                mMainRole.show();
                mMainRole.start();
                showColorHint();

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

            Queue<bonusBlock> mBonus = new Queue<bonusBlock>();

            float mConstBonusMaxY= 300;
            float mConstBonusMinY = 50;

            float mConstBonusStartPosX = 300;
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
                //Console.WriteLine("add bonus");
                var bb = new bonusBlock();
                //var num = randRange(2, 1);

                float xpos = (int)getLastBonusX();
                //Console.WriteLine("xpos:"+xpos);

                int blankIdx = randRange(ROW_BLOCKS-1, 1);
                var ls = randList(9, 0);
                for (int i = 0; i < world.ROW_BLOCKS; ++i)
                {
                    if (i != blankIdx)
                    {
                        var b = new bonusNumber();
                        b.setIdx(ls[i]);
                        b.mUI.paresent = mUIRoles;
                        b.mUI.px = xpos + mConstBonusIntervalX;
                        b.mUI.py = (360 / ROW_BLOCKS) * i;
                        bb.mBonus.Add(b);
                    }
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
                foreach(var elem in mBonus)
                {
                    var bs = elem.mBonus;
                    foreach (var belem in bs)
                    {
                        if (belem.mUI.visible) 
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

            internal void showScore(int num)
            {
                mScore.visible = true;
                mScore.text = "score:" + num;
            }

            public void showColorHint(string str="", uint p1=(uint)EColorUtil.gold)
            {
                if (str == "")
                {
                    mPrompt.visible = false;
                    return;
                }
                mPrompt.visible = true;
                mPrompt.text = str;
                mPrompt.textColor = p1;
            }

            internal void clean()
            {
                UIRoot.Instance.evtLeftUp -= onClick;
                UIRoot.Instance.evtLeftUp -= endToWin;
                mState = EState.invalid;
                mPrompt.visible = false;
                mScore.visible = false;
                clearBonus();
                if(mMainRole != null)
                    mMainRole.clean();
                TimerManager.get().clearTimer(timeIDGenBonus);
                TimerManager.get().clearTimer(timeIDFailToInit);
                UIRoot.Instance.evtLeftUp -= endToWin;
            }

            private void clearBonus()
            {
                foreach (var elem in mBonus)
                {
                    var bs = elem.mBonus;
                    foreach (var belem in bs)
                    {
                        belem.mUI.paresent = null;
                    }
                }

                mBonus.Clear();
            }

            void endToWin(int x, int y)
            {
                UIRoot.Instance.evtLeftUp -= endToWin;
                init();
            }

            int timeIDFailToInit = -1;
            internal void fail()
            {
                showColorHint("You failed!", (uint)EColorUtil.red);
                timeIDFailToInit = TimerManager.get().setTimeout(t => UIRoot.Instance.evtLeftUp += endToWin, 1000);
            }

            internal void win()
            {
                showColorHint("You win!", (uint)EColorUtil.goldenrod);
                timeIDFailToInit = TimerManager.get().setTimeout(t => UIRoot.Instance.evtLeftUp += endToWin, 1000);
            }
        }

        class move
        {
            public UIWidget mUI = null;
            
            public move(int size)
            {
                mUI = new UIRect(size, size);
                mLb = new UILabel();
                mLb.paresent = mUI;
                mUI.offsety = mUI.offsetx = 2;
            }

            public void destory()
            {
                mUI.paresent = null;
                mUI = null;
            }

            public void setSize(float sz)
            {
                mUI.height = mUI.width = sz;
            }

            public void setPosition(float x, float y)
            {
                mUI.px = x;
                mUI.py = y;
            }

            protected UILabel mLb = null;

            public string getText()
            {
                return mLb.text;
            }
            
            public void setText(string str)
            {
                mLb.text = str;
            }

            public void setTextColor(uint color)
            {
                mLb.textColor = color;
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

            public void setShink(bool bShink = true)
            {
                mUI.paddingY = mUI.paddingX = 4;
                mUI.shrinkAble = true;
            }

        }

        class bonus : move
        {
            public bonus()
                : base(32)
            {
                mLb.alignParesent = mLb.align = UIWidget.EAlign.center;
                mUI.width = 64;
                mUI.height = 360 / (float)world.ROW_BLOCKS;
            }

            public void hide()
            {
                mUI.visible = false;
            }
        }

        class bonusNumber : bonus
        {
            public static int[] nums= new int[]{2, 4, 8, 16, 32, 64, 128, 256, 512, 1024};

            public int number = 0;
            public void setIdx(int idx)
            {
                number = nums[idx];
                setText(number.ToString());
            }

            public bonusNumber(int num)
            {
                number = num;
                setText(num.ToString() );
            }

            public bonusNumber()
            {
                int r = world.randRange(9, 0);
                number = nums[r];
                setText(number.ToString() );
            }
        }

        class GameExeption : Exception
        {
            public enum EType
            {
                NumberNow,
                OperatorNow,
                DivByZero,
                noExactDivision,
                noTheSameNumber,
            }

            public EType mType = EType.NumberNow;
            public GameExeption(EType type)
            {
                mType = type;
            }
        }

        class mainRole : move
        {
            void setNumber(int number)
            {
                mResult = number;
                setText(mResult.ToString());
            }

            public mainRole(world w):base(24)
            {
                mWorld = w;
                mUI.paresent = w.mUIMainRole;
                setNumber(2);
                setShink();
            }
            world mWorld = null;
            
            string mExpr = "2";
            int mResult = 2;
            void appendNum(int number)
            {
                if (number == mResult)
                {
                    mResult = mResult * 2;
                    setText(mResult.ToString() );
                }
                else
                {
                    throw new GameExeption(GameExeption.EType.noTheSameNumber);
                }
            }

           
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
                mExpr = "";
                mVelY = 0;
                mAccY = mConstFallenVel;
                flyingTimeID = -1;
                mState = EState.init;
                mUI.px = mConstPx;
                mUI.py = mConstPy;
                setNumber(2);
                setColor((uint)EColorUtil.gold);
            }

            //const
            float mConstPx = 100;//初始位置
            float mConstPy = 100;
            float mConstPyGround = 320;//地面高度，判断是否撞地
            
            //控制飞行
                //点击控制
            float mConstUpV = -110;
            bool mTap = false;
            float mVelY = 0; //
            float mAccY = 100;
            int flyingTimeID = -1;




            public void flyUp()
            {
                mTap = true;
            }

            public void calcFly(uint det, uint last)
            {
                float t = (float)det / 1000;
                mUI.py = mUI.py + (mVelY * t + 0.5f * t * t * mAccY);
                mVelY = mVelY + mAccY * t;
            }

            public void onTimeFlying(uint det, uint last)
            {
                if (mState == EState.flying)
                {
                    if (mTap)
                    {
                        mVelY = mConstUpV;
                        mTap = false;
                    }
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
                            bns.hide();
                            try
                            {
                                bonusNumber bn = bns as bonusNumber;
                                if (bn != null)
                                {
                                    appendNum(bn.number);
                                }
                               
                                if (mResult == 32)
                                {
                                    mState = EState.init;
                                    mWorld.win();
                                }
                            }
                            catch (GameExeption ge)
                            {
                                falling();
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }

                    if (checkIfGround())
                    {
                        die();
                    }
                    
                }
                else if (mState == EState.falling)
                {
                    calcFly(det, last);
                    if(checkIfGround() )
                    {
                        die();
                    }
                }
                else if (mState == EState.dead)
                {
                }
                mUI.setDirty(true);
            }

            private void die()
            {
                mState = EState.dead;
                setColor((uint)EColorUtil.black);
                clean();
                mWorld.fail();
            }

            private bool checkIfGround()
            {
                if (mUI.py > mConstPyGround) return true;
                return false;
            }

            private void falling()
            {
                UIRoot.Instance.evtLeftDown -= onLMD;
                mTap = false;
                mState = EState.falling;
                mVelY = mConstFallenVel;
                mAccY = 0;
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

            internal void init()
            {
                resetFly();
            }

            public void start()
            {   
                UIRoot.Instance.evtLeftDown += onLMD;
                mState = EState.flying;
                flyingTimeID = TimerManager.get().setInterval(onTimeFlying, 10);
            }

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

            internal void clean()
            {
                mTap = false;
                mExpr = "";
                TimerManager.get().clearTimer(flyingTimeID);
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
                return "flappy 2048";
            }

            public string desc()
            {
                return @"
the game you know
";
            }


            public ns_YAUI.UIWidget getAttach()
            {
                mWorld = new world();
                mWorld.init();
                return mWorld.mRoot;
            }

            public void lostAttach()
            {
                if (mWorld != null)
                    mWorld.clean();
            }
        }
    }
}
