using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;

namespace ns_behaviour
{
    class TimeMain : Singleton<TimeMain>
    {
        public TimeMain()
        {
            build();
        }


        void build()
        {
            //this.dragAble = true;
            //var ui = UIRoot.Instance.loadFromXML(xmllayout);
            //ui.paresent = this;
            //TimerListModel.Instance.drawFrame();
            //TimerListModel.Instance.drawTest();
            //TimerListModel.Instance.showList();
            //TimeItemModel.Instance.showList();
            TimeTaskManager.Instance.read();
            TimerListModel.Instance.showList();
        }

        
    }

    class TimeItemModel : Singleton<TimeItemModel>
    {
        public TimeItemModel()
        {
            init();
        }

        UIWidget mRoot;
        UILable mCancel;
        UILable mConfirm;
        UILable mDelete;

        UILable mHour;
        UILable mMinute;

        UILable[] mWeekTask = new UILable[7];
        UILable mLoop;

        UILable mDesc;

        System.Drawing.Point formPos;
        System.Drawing.Point mousePos;
        bool moveEnable = false;
        void init()
        {
            mRoot = UIRoot.Instance.loadFromXML(strItemTemplate);


            mRoot.evtOnLMDown += (u, x, y) =>
                {
                    formPos = Globals.Instance.mPainter.Location;
                    mousePos = System.Windows.Forms.Control.MousePosition;
                    moveEnable = true;
                    return false;
                };
            mRoot.evtOnMMove += (u, x, y) =>
            {
                if (moveEnable)
                {
                    var newPos = System.Windows.Forms.Control.MousePosition;
                    Globals.Instance.mPainter.Location = new System.Drawing.Point(formPos.X + newPos.X - mousePos.X, formPos.Y + newPos.Y - mousePos.Y);
                }
                return false;
            };

            mRoot.evtOnLMUp += (u, x, y) =>
            {
                moveEnable = false;
                return false;
            };

            mCancel = mRoot.childOfPath("cancel") as UILable;
            mConfirm = mRoot.childOfPath("comfirm") as UILable;
            mDelete = mRoot.childOfPath("delete") as UILable;
            mHour = mRoot.childOfPath("hour/lable") as UILable;
            mMinute = mRoot.childOfPath("minute/lable") as UILable;

            mDesc = mRoot.childOfPath("desc") as UILable;

            mWeekTask[0] = mRoot.childOfPath("sun") as UILable;
            mWeekTask[1] = mRoot.childOfPath("mon") as UILable;
            mWeekTask[2] = mRoot.childOfPath("tue") as UILable;
            mWeekTask[3] = mRoot.childOfPath("wen") as UILable;
            mWeekTask[4] = mRoot.childOfPath("thu") as UILable;
            mWeekTask[5] = mRoot.childOfPath("fri") as UILable;
            mWeekTask[6] = mRoot.childOfPath("sat") as UILable;

            mWeekTask[0].attrs["col"] = mWeekTask[0].textColor;
            mWeekTask[1].attrs["col"] = mWeekTask[1].textColor;
            mWeekTask[2].attrs["col"] = mWeekTask[2].textColor;
            mWeekTask[3].attrs["col"] = mWeekTask[3].textColor;
            mWeekTask[4].attrs["col"] = mWeekTask[4].textColor;
            mWeekTask[5].attrs["col"] = mWeekTask[5].textColor;
            mWeekTask[6].attrs["col"] = mWeekTask[6].textColor;

            mLoop = mRoot.childOfPath("loop") as UILable;
            mLoop.attrs["col"] = mLoop.textColor;
        }

        public void showItem(int id)
        {
            var task = TimeTaskManager.Instance.getTaskByID(id);
            if (task == null) return;

            UIRoot.Instance.root.clear();
            mRoot.paresent = UIRoot.Instance.root;

            mHour.text = ""+task.dt.Hour;
            mHour.evtOnLMDownClear();
            mHour.evtOnLMDown += (u, x, y) =>
                {
                    InputForm.Instance.show(true, x, y);
                    InputForm.Instance.evtInputExit = (text) =>
                    {
                        if (text != "")
                            mHour.text = text;
                    };
                    return true;
                };


            mMinute.text = "" + task.dt.Minute;
            mMinute.evtOnLMDown += (u, x, y) =>
            {
                InputForm.Instance.show(true, x, y);
                InputForm.Instance.evtInputExit = (text) =>
                {
                    if (text != "")
                        mMinute.text = text;
                };
                return false;
            };

            mDesc.text = task.taskDesc;
            mDesc.evtOnLMDownClear();
            mDesc.evtOnLMDown += (u, x, y) =>
            {
                InputForm.Instance.show(true, x, y);
                InputForm.Instance.evtInputExit = (text) =>
                {
                    if (text != "")
                        mDesc.text = text;
                };
                return false;
            };

            if (task.loop) mLoop.textColor = 0xffffffff;
            else mLoop.textColor = 0x77777777;
            mLoop.evtOnLMDownClear();
            mLoop.evtOnLMDown += (u, x, y) =>
                {
                    if (mLoop.textColor == 0x77777777) mLoop.textColor = 0xffffffff;
                    else if (mLoop.textColor == 0xffffffff) mLoop.textColor = 0x77777777;
                    return false;
                };

            for (int i = 0; i < 7; ++i)
            {
                bool b = task.weekMask[i];
                if (b) mWeekTask[i].textColor = 0xffffffff;
                else mWeekTask[i].textColor = 0x77777777;
                var ui = mWeekTask[i];
                ui.evtOnLMDownClear();
                ui.evtOnLMDown += (u, x, y) =>
                {
                    if (ui.textColor == 0x77777777) ui.textColor = 0xffffffff;
                    else if (ui.textColor == 0xffffffff) ui.textColor = 0x77777777;
                    return false;
                };
            }

            mCancel.evtOnLMDownClear();
            mCancel.evtOnLMDown += (u, x, y) =>
                {
                    TimerListModel.Instance.showList();
                    return false;
                };
            mConfirm.evtOnLMDownClear();
            mConfirm.evtOnLMDown += (u, x, y) =>
                {
                    try
                    {
                        getTaskFromUI(task);
                        TimeTaskManager.Instance.write();
                        TimerListModel.Instance.showList();
                    }
                    catch (Exception e)
                    {
                        TimerListModel.Instance.showList();
                    }
                    return false;
                };
            mDelete.evtOnLMDownClear();
            mDelete.evtOnLMDown += (u, x, y) =>
                {
                    TimeTaskManager.Instance.removeTask(task.id);
                    TimerListModel.Instance.showList();
                    return false;
                };


        }

        public void getTaskFromUI(PTaskTime t)
        {
            t.dt = new DateTime();
            int h = Convert.ToInt32(mHour.text);
            int m = Convert.ToInt32(mMinute.text);
            t.dt = t.dt.AddHours(h);
            t.dt = t.dt.AddMinutes(m);
            t.taskDesc = mDesc.text;
            for (int i = 0; i < 7; ++i)
            {
                if (mWeekTask[i].textColor == 0xffffffff) t.weekMask[i] = true;
                else t.weekMask[i] = false;
            }

            if (mLoop.textColor == 0xffffffff) t.loop = true;
            else t.loop = false;
        }

        public void showAddItem()
        {
            var n = DateTime.Now;
            var t = TimeTaskManager.Instance.newKillTask(n.Hour+1, n.Minute, "NONE");
            showItem(t.id);
            mCancel.evtOnLMDownClear();
            mCancel.evtOnLMDown += (u, x, y) =>
            {
                TimeTaskManager.Instance.removeTask(t.id);
                TimerListModel.Instance.showList();
                return false;
            };
        }

        string strItemTemplate = @"
                    <rect name=""root"" width=""320"" height=""480"" stokeColor=""0xffffffff"" fillColor=""0xff33b887"" >

                        <lable name=""cancel"" text=""取消"" size=""18"" offsetx=""4"" offsety=""4""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""delete"" text=""删除"" size=""18"" offsetx=""0"" offsety=""4"" color=""0xffffaf60""
                                    align=""middleTop"" alignParesent=""middleTop""> </lable>
                        <lable name=""comfirm"" text=""确定"" size=""18"" offsetx=""-4"" offsety=""4""
                                    align=""rightTop"" alignParesent=""rightTop""> </lable>
                        
                        <rect name=""hour"" width=""150"" height=""96"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""0"" offsety=""48""
                            stokeColor=""0xffdeb887"" fillColor=""0xffdeb887"" 
                            clip=""true"">
                            <lable name=""lable"" text=""12"" size=""30""
                                    align=""center"" alignParesent=""center""> </lable>
                        </rect>
                        <rect name="":"" width=""20"" height=""96"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""150"" offsety=""48""
                            stokeColor=""0xffdeb887"" fillColor=""0xffdeb887"" 
                            clip=""true"">
                            <lable name=""lable"" text="":"" size=""30""
                                    align=""center"" alignParesent=""center""> </lable>
                        </rect>
                        <rect name=""minute"" width=""150"" height=""96"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""170"" offsety=""48""
                            stokeColor=""0xffdeb887"" fillColor=""0xffdeb887"" 
                            clip=""true"">
                            <lable name=""lable"" text=""21"" size=""30""
                                    align=""center"" alignParesent=""center""> </lable>
                        </rect>
                        <!--rect width=""320"" height=""48"" offsetx=""0"" offsety=""170""
                                    align=""leftTop"" alignParesent=""leftTop""> </rect-->
                        <lable name=""desc"" text=""DESC"" size=""18"" offsetx=""4"" offsety=""150""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>

                        <lable name=""sun"" text=""日"" size=""18"" offsetx=""4"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""mon"" text=""一"" size=""18"" offsetx=""34"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""tue"" text=""二"" size=""18"" offsetx=""64"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""wen"" text=""三"" size=""18"" offsetx=""94"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""thu"" text=""四"" size=""18"" offsetx=""124"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""fri"" text=""五"" size=""18"" offsetx=""154"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""sat"" text=""六"" size=""18"" offsetx=""184"" offsety=""220""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                        <lable name=""loop"" text=""循环"" size=""18"" offsetx=""4"" offsety=""270""
                                    align=""leftTop"" alignParesent=""leftTop""> </lable>
                    </rect>";
    }



    class TimerListModel : Singleton<TimerListModel>
    {
        UIWidget mRoot = null;
        UIWidget mBar = null;
        UIWidget mAdd = null;
        
        List<UIWidget> mItemList = new List<UIWidget>();

        public TimerListModel()
        {
            init();
        }

        string itemTemplate = @"
                    <rect name=""item"" width=""320"" height=""48"" 
                        align=""leftTop"" alignParesent=""leftTop"" offsetx=""0"" offsety=""48""
                        stokeColor=""0xffffffff"" fillColor=""0xffdeb887"" 
                        clip=""true"">
                        <rect name=""time"" width=""100"" height=""48"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""0"" offsety=""0""
                            stokeColor=""0xffffffff"" fillColor=""0xffdeb887"">
                            <lable name=""lable"" text=""关闭"" size=""18""
                                align=""center"" alignParesent=""center""> </lable>
                        </rect>
                        <rect name=""type"" width=""100"" height=""48"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""100"" offsety=""0""
                            stokeColor=""0xffffffff"" fillColor=""0xffdeb887"">
                            <lable name=""lable"" text=""关闭"" size=""10""
                                align=""center"" alignParesent=""center""> </lable>
                        </rect>
                        <rect name=""desc"" width=""120"" height=""48"" 
                            align=""leftTop"" alignParesent=""leftTop"" offsetx=""200"" offsety=""0""
                            stokeColor=""0xffffffff"" fillColor=""0xffdeb887"" clip=""true"">
                            <lable name=""lable"" text=""-"" size=""12""
                                align=""center"" alignParesent=""center""> </lable>
                        </rect>
                    </rect>";

        
        public void init()
        {
            drawFrame();
        }

        public void showList()
        {
            TimeTaskManager.Instance.mState = TimeTaskManager.State.e_list;
            UIRoot.Instance.root.clear();
            drawFrame();
            mRoot.clear();
            drawFrame();
            mBar.paresent = mRoot;
            var tl = TimeTaskManager.Instance.allTask();
            mItemList.Clear();
            int height = 48;
            foreach (var t in tl)
            {
                var uiItem1 = UIRoot.Instance.loadFromXML(itemTemplate) as UIRect;
                uiItem1.offsety = height;
                height += 48;
                uiItem1.paresent = mRoot;
                mItemList.Add(uiItem1);
                
                var uiTimeLable = uiItem1.childOfPath("time/lable") as UILable;
                uiTimeLable.text = string.Format("{0}:{1}", t.dt.Hour, t.dt.Minute);

                var uiTypeLable = uiItem1.childOfPath("type/lable") as UILable;
                string strType = "周";
                for(int i = 0; i<t.weekMask.Length; ++i)
                {
                    if(t.weekMask[i])
                    {
                        if(i == 0)
                        {
                            strType += "日";
                        }
                        else if(i == 1)
                        {
                            strType += "一";
                        }
                        else if(i == 2)
                        {
                            strType += "二";
                        }
                        else if(i == 3)
                        {
                            strType += "三";
                        }
                        else if(i == 4)
                        {
                            strType += "四";
                        }
                        else if(i == 5)
                        {
                            strType += "五";
                        }
                        else if(i == 6)
                        {
                            strType += "六";
                        }
                    }
                }
                uiTypeLable.text = strType;
                var uiDescLable = uiItem1.childOfPath("desc/lable") as UILable;
                uiDescLable.text = t.taskDesc;
                uiItem1.evtOnLMDownClear();
                uiItem1.evtOnLMDown += (u, x, y) =>
                    {
                        TimeItemModel.Instance.showItem(t.id);
                        return false;
                    };
            }
            mAdd.evtOnLMDownClear();
            mAdd.evtOnLMDown += (w, x, y) =>
                {
                    TimeItemModel.Instance.showAddItem();
                    return false;
                };
        }
        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        public void drawFrame()
        {
            string lo_frame = @"
                <stub name=""root"">
                    <rect name=""main"" width=""320"" height=""480"" 
                        stokeColor=""0xffffffff"" fillColor=""0xff33b887"" 
                        clip=""true"">
                    </rect>
                </stub>
                ";
            
            string lo_tab = @"
                <stub name=""root"">
                    <rect name=""add"" width=""80"" height=""48"" 
                        stokeColor=""0x00000000"" fillColor=""0x00000000"" 
                        clip=""true"">
                            <lable name=""lable"" text=""+"" size=""30""
                            align=""center"" alignParesent=""center""> </lable>
                    </rect>
                    <rect name=""t2"" width=""80"" height=""48"" offsetx=""160"" offsety=""0"" align=""leftTop"" alignParesent=""leftTop""
                        stokeColor=""0xffffffff"" fillColor=""0xff33b887"" 
                        clip=""true"">
                            <lable name=""lable"" text="""" size=""14"" color=""0xdddddddd""
                            align=""center"" alignParesent=""center""> </lable>
                    </rect>
                    <rect name=""t3"" width=""80"" height=""48"" offsetx=""80"" offsety=""0"" align=""leftTop"" alignParesent=""leftTop""
                        stokeColor=""0xffffffff"" fillColor=""0xff33b887"" 
                        clip=""true"">
                            <lable name=""lable"" text=""SHUT"" size=""14""
                            align=""center"" alignParesent=""center""> </lable>
                    </rect>
                    <rect name=""close"" width=""80"" height=""48"" offsetx=""240"" offsety=""0"" align=""leftTop"" alignParesent=""leftTop""
                        stokeColor=""0xffffffff"" fillColor=""0xff33b887"" 
                        clip=""true"">
                            <lable name=""_"" text=""_"" size=""20"" 
                            align=""center"" alignParesent=""center"" offsetx=""-10"" offsety=""-10""
                            clip=""true""> </lable>
                            <lable name=""lable"" text=""x"" size=""20"" color=""0xffffaf60""
                            align=""center"" alignParesent=""center"" offsetx=""20"" offsety=""0""
                            clip=""true""> </lable>
                    </rect>
                </stub>";

            mRoot = UIRoot.Instance.loadFromXML(lo_frame);
            mBar = UIRoot.Instance.loadFromXML(lo_tab);
            var mClose = mBar.childOfPath("close/lable");
            mClose.evtOnLMDown += (u, x, y) =>
                {
                    System.Windows.Forms.Application.Exit();
                    return false;
                };

            var mMin = mBar.childOfPath("close/_");
            mMin.evtOnLMDown += (u, x, y) =>
            {
                //System.Windows.Forms.Application.Exit();
                //ShowWindow(FindWindow("Shell_TrayWnd", null), 0/9);
                PaintDriver.mIns.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                return false;
            };

            mRoot.evtOnLMDown += (u, x, y) =>
                {
                    formPos = Globals.Instance.mPainter.Location;
                    mousePos = System.Windows.Forms.Control.MousePosition;
                    moveEnable = true;
                    return false;
                };
            mRoot.evtOnMMove += (u, x, y) =>
            {
                if (moveEnable)
                {
                    var newPos = System.Windows.Forms.Control.MousePosition;
                    Globals.Instance.mPainter.Location = new System.Drawing.Point(formPos.X + newPos.X - mousePos.X, formPos.Y + newPos.Y - mousePos.Y);
                }
                return false;
            };

            mRoot.evtOnLMUp += (u, x, y) =>
            {
                moveEnable = false;
                return false;
            };


            mBar.paresent = mRoot;
            mAdd = mBar.childOfPath("add");

            mRoot.paresent = UIRoot.Instance.root;
        }

        
        System.Drawing.Point formPos;
        System.Drawing.Point mousePos;
        bool moveEnable = false;



    }

    class testProcessTimer : Singleton<testProcessTimer>
    {
        public testProcessTimer()
        {
            Globals.Instance.evtOnInit += main;
        }

        public void main()
        {
            ns_behaviour.TimerManager.get().setInterval((t) =>
            {
                var tasks = TimeTaskManager.Instance.nowTasks();
                foreach (var tt in tasks)
                {
                    if (tt.task != null && !tt.killed) { tt.killed = true;  tt.task(tt.taskDesc); }
                    if (tt.done)
                    {
                        PTaskTimeList.Instance.removeTask(tt.id);
                        TimerListModel.Instance.showList();
                    }
                }

            }, 1000);

            TimeMain tm = TimeMain.Instance;
        }
    }

    

    public class ProcessKiller : Singleton<ProcessKiller>
    {
        public void kill(string pname)
        {
            //foreach (var p in System.Diagnostics.Process.GetProcesses)
            var ps = System.Diagnostics.Process.GetProcesses();

            foreach (var p in ps)
            {
                Console.WriteLine(p.ToString());
                if (p.ProcessName.ToLower() == pname.ToLower())
                {
                    try
                    {
                        p.Kill();
                        ns_behaviour.Globals.Instance.mRepl.print("" + p.ToString() + " is killed");
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }
    }

    class PTaskTime
    {
        public bool[] weekMask = new bool[7];
        public bool loop = false;
        public DateTime dt;
        public Action<string> task;
        public string taskDesc = "";
        public int id;
        public bool done = false;
        public bool ontime = false;
        public bool killed = false;

        public override string ToString()
        {
            return taskDesc + " at " + dt.Hour + ":" + dt.Minute;
        }

        PTaskTime()
        {
            for (int i = 0; i < weekMask.Length; ++i)
            {
                weekMask[i] = false;
            }
        }

        public static PTaskTime newTimer(int hour, int minute, bool everyDay=false)
        {
            PTaskTime pt = new PTaskTime();
            pt.dt = pt.dt.AddHours(hour);
            pt.dt = pt.dt.AddMinutes(minute);
            if (everyDay)
            {
                for (int i = 0; i < 7; ++i)
                {
                    pt.weekMask[i] = true;
                }
            }
            else
            {
                int d = Convert.ToInt32(DateTime.Now.DayOfWeek);
                pt.weekMask[d] = true;
            }

            return pt;
        }

        public void setWeekDate(int d, bool set)
        {
            d = d % 7;
            weekMask[d] = set;
        }

        public void setLoop(bool set)
        {
            loop = set;
        }

        public void compareNow(DateTime d)
        {
            ontime = false;
            done = false;
            int dayInWeek = Convert.ToInt32(d.DayOfWeek);

            if(weekMask[dayInWeek] && dt.Hour == d.Hour && dt.Minute == d.Minute)
            {
                ontime = true;
            }

            if (loop)
            {
                done = false;
                return;
            }
            for (int i = dayInWeek; i < 7; ++i)
            {
                if (24 * 60 * i + dt.Hour * 60 + dt.Minute > 24 * 60 * dayInWeek + d.Hour * 60 + d.Minute)
                {
                    done = true;
                    return;
                }
            }
        }


    };

    class PTaskTimeList : Singleton<PTaskTimeList>
    {
        public SortedList<int, PTaskTime> mList = new SortedList<int, PTaskTime>();

        int idCount = 1;
        int newID()
        {
            return idCount++;
        }

        public void addTask(PTaskTime t)
        {
            var id = newID();
            t.id = id;
            mList.Add(id, t);
        }

        public void removeTask(int id)
        {
            if (mList.ContainsKey(id))
            {
                mList.Remove(id);
            }
        }
    };

    class TimeTaskManager : Singleton<TimeTaskManager>
    {
        public enum State
        {
            e_list,
            e_item_mod,
            e_item_add,
        }

        public State mState = State.e_list;
        string fname = @"date.xml";
        public void read()
        {
            XmlDocument doc = new XmlDocument();
            var path = System.Windows.Forms.Application.StartupPath +"\\" + fname;
            bool exist = File.Exists(path);
            byte[] bs = File.ReadAllBytes(path);
            string str = System.Text.Encoding.Default.GetString(bs);
            doc.LoadXml(str);

            XmlNode node = doc.ChildNodes[0];
            var childs = node.ChildNodes;

            Action<string, PTaskTime> str2bool = delegate(string strWeek, PTaskTime t)
            {
                for (int i = 0; i < 7; i++) t.setWeekDate(i, false);
                    if (strWeek.Contains("0"))
                    {
                        t.setWeekDate(0, true);
                    }
                    if (strWeek.Contains("1"))
                    {
                        t.setWeekDate(1, true);
                    }
                    if (strWeek.Contains("2"))
                    {
                        t.setWeekDate(2, true);
                    }
                    if (strWeek.Contains("3"))
                    {
                        t.setWeekDate(3, true);
                    }
                    if (strWeek.Contains("4"))
                    {
                        t.setWeekDate(4, true);
                    }
                    if (strWeek.Contains("5"))
                    {
                        t.setWeekDate(5, true);
                    }
                    if (strWeek.Contains("6"))
                    {
                        t.setWeekDate(6, true);
                    }
            };
            foreach (XmlNode c in childs)
            {
                bool loop = false;
                var ret = c.Attributes.GetNamedItem("loop");
                if (ret != null) loop = ret.Value.castBool();
                int hour = -1;
                int minute = -1;
                string strWeek = "";
                string strProcess = "";
                ret = c.Attributes.GetNamedItem("hour");
                if (ret != null) hour = ret.Value.castInt();
                ret = c.Attributes.GetNamedItem("minute");
                if (ret != null) minute = ret.Value.castInt();
                ret = c.Attributes.GetNamedItem("week");
                if (ret != null) strWeek = ret.Value;
                ret = c.Attributes.GetNamedItem("process");
                if (ret != null) strProcess = ret.Value;

                var t = TimeTaskManager.Instance.newKillTask(hour, minute, strProcess);
                t.setLoop(loop);
                str2bool(strWeek, t);
            }
        }

        public void write()
        {
            XmlDocument doc = new XmlDocument();
            //XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.AppendChild(docNode);

            XmlNode root = doc.CreateElement("root");
            doc.AppendChild(root);

            var ts = TimeTaskManager.Instance.allTask();
            foreach (PTaskTime t in ts)
            {
                XmlNode tn = doc.CreateElement("task");
                var att = doc.CreateAttribute("hour");
                att.Value = "" + t.dt.Hour;
                tn.Attributes.Append(att);

                att = doc.CreateAttribute("minute");
                att.Value = "" + t.dt.Minute;
                tn.Attributes.Append(att);

                att = doc.CreateAttribute("loop");
                att.Value = "" + t.loop;
                tn.Attributes.Append(att);

                att = doc.CreateAttribute("process");
                att.Value = "" + t.taskDesc;
                tn.Attributes.Append(att);

                att = doc.CreateAttribute("week");
                var strWeek = "w";
                string[] cvt = new string[7]{"0","1", "2","3","4","5","6"};
                for (int i = 0; i < 7; ++i)
                {
                    if (t.weekMask[i])
                    {
                        strWeek += cvt[i];
                    }
                }
                att.Value = strWeek;
                tn.Attributes.Append(att);
                root.AppendChild(tn);
            }
            doc.Save(fname);
        }

        public PTaskTime newKillTask(int hour, int minute, string task)
        {
            var t = PTaskTime.newTimer(hour, minute);
            t.taskDesc = ""+task.ToLower();
            t.task = (strTask)=>{
                ProcessKiller.Instance.kill(strTask);
            };

            PTaskTimeList.Instance.addTask(t);
            return t;
        }

        public void removeTask(int id)
        {
            PTaskTimeList.Instance.removeTask(id);
        }

        public List<PTaskTime> allTask()
        {
            return PTaskTimeList.Instance.mList.Values.ToList();
        }

        public PTaskTime getTaskByID(int id)
        {
            PTaskTime ret = null;
            PTaskTimeList.Instance.mList.TryGetValue(id, out ret);
            return ret;
        }

        public void clear()
        {
            PTaskTimeList.Instance.mList.Clear();
        }

        public List<PTaskTime> nowTasks()
        {
            var now = DateTime.Now;
            List<PTaskTime> ret = new List<PTaskTime>();
            foreach (KeyValuePair<int, PTaskTime> item in PTaskTimeList.Instance.mList)
            {
                item.Value.compareNow(now);
                if (item.Value.ontime)
                {
                    ret.Add(item.Value);
                }
                else
                {
                    item.Value.killed = false;
                }
            }
            return ret;
        }
    }
}

public class CMD
{
    public static void addTask(int hour, int minute, string pname)
    {
        var pt = ns_behaviour.TimeTaskManager.Instance.newKillTask(hour, minute, pname);
        ns_behaviour.Globals.Instance.mRepl.print(pt.ToString());

        ns_behaviour.TimerListModel.Instance.showList();
    }
}
