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
                int r = world.randRange(1, 0);
                bonus b = null;
                if (r == 0)
                {
                    b = new bonusNumber();
                }
                else
                {
                    b = new bonusOp();
                }
                mBonus.Add(b);
                return b;
            }
        }

        class world
        {
            public static System.Random mRandGen = new System.Random();
            public static int randRange(int max, int min = 0)
            {
                int r = mRandGen.Next() % (max - min + 1);
                return min + r;
            }


            public UIWidget mUI = null;
            const string XMLFrame = @"
<rect clip='false' enable='false' size='480,360'>
    <label name='prompt' text='Get Ready' color='gold' align='center' size='24' style='bold'></label>
    <label name='log' offset='24' color='green' align='leftTop' size='10' style='normal'></label>
    <label name='score' offsetX='24' offsetY='48' color='green' align='leftTop' size='10' style='normal'></label>
</rect>
";
            role mRole = null;
            public world()
            {
                mUI = UI.Instance.fromXML(XMLFrame, false);
                mPrompt = mUI.findByPath("prompt") as UILabel;
                mLog = mUI.findByPath("log") as UILabel;
                mScore = mUI.findByPath("score") as UILabel;
                mRole = new role(this);
            }

            UILabel mPrompt = null;
            UILabel mLog = null ;
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
                mRole.init();
                
                mState = EState.ready;
                showInit();
                
                UIRoot.Instance.evtLeftUp += onClick;
                mState = EState.ready;
                mUI.setDirty(true);
            }

            private void showInit()
            {
                //get ready
                mPrompt.visible = true;
                showColorString((uint)EColorUtil.gold, "get ready?");
                mLog.visible = false;
                mRole.hide();
            }

            int timeIDGenBonus = -1;
            public void start()
            {
                Console.WriteLine("world.start");
                mState = EState.gaming;
                mRole.show();
                mRole.start();

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
                Console.WriteLine("add bonus");
                var bb = new bonusBlock();
                var num = randRange(2, 1);

                float xpos = (int)getLastBonusX();
                Console.WriteLine("xpos:"+xpos);

                List<float> posYs = new List<float>();
                for (int i = 0; i < num; ++i)
                {
                    int y = randRange((int)mConstBonusMaxY, (int)mConstBonusMinY);
                    //b.mUI.py
                    bool tooClose = false;
                    for (int j = 0; j < posYs.Count; ++j)
                    {
                        if (Math.Abs(posYs[j] - y) < 90) { tooClose = true; break; }
                    }
                    if (tooClose) continue;
                    var b = bb.addNewBonus();
                    b.mUI.paresent = mUI;
                    b.mUI.px = xpos + mConstBonusIntervalX;
                    b.mUI.py = y;
                    posYs.Add(y);
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

            //结束画面
            internal void showFail()
            {
                showWaring("you fail!");
            }

            internal void showScore(int num)
            {
                mScore.visible = true;
                mScore.text = "score:" + num;
            }

            internal void showWaring(string p)
            {
                showColorString((uint)EColorUtil.red, p);
            }

            internal void showLog(string str)
            {
                mLog.visible = true;
                mLog.text = str;
            }

            private void showColorString(uint p1, string str)
            {
                mPrompt.visible = true;
                mPrompt.text = str;
                mPrompt.textColor = p1;
            }

            internal void clean()
            {
                UIRoot.Instance.evtLeftUp -= onClick;
                UIRoot.Instance.evtLeftUp -= failToInit;
                mState = EState.invalid;
                mPrompt.visible = false;
                mLog.visible = false;
                mScore.visible = false;
                clearBonus();
                if(mRole != null)
                    mRole.clean();
                TimerManager.get().clearTimer(timeIDGenBonus);
                TimerManager.get().clearTimer(timeIDFailToInit);
                UIRoot.Instance.evtLeftUp -= failToInit;
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

            void failToInit(int x, int y)
            {
                UIRoot.Instance.evtLeftUp -= failToInit;
                init();
            }

            int timeIDFailToInit = -1;
            internal void fail()
            {
                showFail();
                timeIDFailToInit = TimerManager.get().setTimeout(t => UIRoot.Instance.evtLeftUp += failToInit, 1000);
            }
        }

        class move
        {
            public UIWidget mUI = null;
            
            public move(int size)
            {
                mUI = new UIRect(size, size);
                mUI.name = "debug";
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

            UILabel mLb = null;

            public string getText()
            {
                return mLb.text;
            }
            
            public void setText(string str, bool align=false)
            {
                if (mLb == null)
                {
                    mLb = new UILabel(str);
                    mLb.paresent = mUI;
                    if (align)
                    {
                        mLb.alignParesent = mLb.align = UIWidget.EAlign.center;
                    }
                }
                else mLb.text = str;
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
                //mLb.align = UIWidget.EAlign.center;
            }

            public void setCenter()
            {
                mLb.align = UIWidget.EAlign.center;
                mLb.alignParesent = UIWidget.EAlign.center;
            }
        }

        class bonus : move
        {
            public bonus()
                : base(32)
            {
                setShink();
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

        class bonusOp : bonus
        {
            public static string[] ops = new string[] { "+", "-", "×", "÷" };
            public enum EOP : int {plus=0, minus, mul, div }
            public EOP mOp = EOP.plus;
            public bonusOp(EOP op)
            {
                mOp = 0;
                setText(ops[(int)op], true);
            }

            public bonusOp()
            {
                int r = world.randRange(3, 0);
                mOp = (EOP)r;
                setText(ops[r], true);
            }

            public static string opStringify(EOP op)
            {
                return ops[(int)op];
            }

            public static int calc(EOP op, int opr1, int opr2)
            {
                if (op == EOP.plus) { return opr1 + opr2; }
                if (op == EOP.minus) { return opr1 - opr2; }
                if (op == EOP.mul) { return opr1 * opr2; }
                if (opr2 == 0)
                    throw new GameExeption(GameExeption.EType.DivByZero);
                if (opr1 % opr2 != 0)
                    throw new GameExeption(GameExeption.EType.noExactDivision);
                return opr1 / opr2;
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
            }

            public EType mType = EType.NumberNow;
            public GameExeption(EType type)
            {
                mType = type;
            }

            public override string Message 
            {
                get
                {
                    if (mType == EType.DivByZero)
                    {
                        return "Div by zero";
                    }
                    else if (mType == EType.noExactDivision)
                    {
                        return "无整除";
                    }
                    else if (mType == EType.NumberNow)
                    {
                        return "取得不是整数";
                    }
                    else if (mType == EType.OperatorNow)
                    {
                        return "取得不是操作符";
                    }
                    throw new Exception();
                }
            }
        }

        class role : move
        {
            public role(world w):base(48)
            {
                mWorld = w;
                mUI.paresent = w.mUI;
                setNumber(2);
                setShink();
            }
            world mWorld = null;
            
            string mExpr = "2";
            int mResult = 0;
            bonusOp.EOP mOp;
            bool mNumberOrOp = true;
            void appendNum(int number)
            {
                if (mNumberOrOp) throw new GameExeption(GameExeption.EType.NumberNow);
                else
                {
                    mResult = bonusOp.calc(mOp, mResult, number);
                    mExpr = mExpr + number;
                    setText(mResult+":"+mExpr);
                    mNumberOrOp = true;

                    mWorld.showScore(mResult);
                }
            }

            void appendOp(bonusOp.EOP op)
            {
                if (!mNumberOrOp) throw new GameExeption(GameExeption.EType.OperatorNow);
                else
                {
                    mOp = op;
                    mExpr = mExpr + bonusOp.opStringify(op);
                    setText(mResult + ":" + mExpr);
                    mNumberOrOp = false;

                    mWorld.showScore(mResult);
                }
            }

            void setNumber(int number)
            {
                mNumberOrOp = true;
                mResult = number;
                setText(mResult.ToString());
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
                setColor((uint)EColorUtil.green);
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
                                else
                                {
                                    bonusOp bo = bns as bonusOp;
                                    if (bo != null)
                                    {
                                        appendOp(bo.mOp);
                                    }
                                }
                            }
                            catch (GameExeption ge)
                            {
                                falling();
                                mWorld.showLog(ge.Message);
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

            //private EBonusRst checkBonus(bonus bns)
            //{
            //    int num = bns.getNum();
            //    if (num == this.getNum())
            //        return EBonusRst.same;
            //    else
            //        return EBonusRst.invalid;
            //}

            //enum EBonusRst
            //{
            //    same,
            //    change,
            //    invalid,
            //}

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
