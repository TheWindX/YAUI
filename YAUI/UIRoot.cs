
/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ns_YAUI
{
    public class UIRoot : Singleton<UIRoot>
    {
        #region root
        UIWidget mRoot;

        public UIWidget root
        {
            get
            {
                return mRoot;
            }
        }
        public UIRoot()
        {
            mRoot = new UIMap();//map
        }

        UIWidget mDirtyRoot = null;
        UIWidget mDirtyRootOld = null;//当mDirtyRoot发生变化时，off couse, mDirtyRootOld也要redraw

        public UIWidget dirtyRoot
        {
            get
            {
                return mDirtyRoot;
            }
            set
            {
                mDirtyRoot = value;
            }
        }

        public void dirtyRedraw()
        {
            if (mDirtyRoot == null) mDirtyRoot = root;
            if (mDirtyRootOld == null) mDirtyRootOld = root;

            mDirtyRoot = mDirtyRoot.getDirtyRoot();
            mDirtyRootOld = UIWidget.commonParesent(mDirtyRoot, mDirtyRootOld);
            if (mHandleDraw != null) mHandleDraw();
        }
        #endregion

        #region XML
        Dictionary<string, XmlElement> mName2Template = new Dictionary<string, XmlElement>();
        Dictionary<string, Stack<XmlElement>> mName2InnerTemplate = new Dictionary<string, Stack<XmlElement>>();

        Stack<Dictionary<string, string>> mPropertyInnerMap = new Stack<Dictionary<string, string>>();
        Stack<Boolean> mPropertyderived = new Stack<Boolean>();

        internal void pushProperty(Dictionary<string, string> map = null, bool derived = true)
        {
            if (map == null) map = new Dictionary<string, string>();
            mPropertyInnerMap.Push(map);
            mPropertyderived.Push(derived);
        }

        public void setPropertyderived(bool derived)
        {
            mPropertyderived.Pop();
            mPropertyderived.Push(derived);
        }

        public void setProperty(string key, ref string value)
        {
            if (value == "") return;
            if (value == null) return;
            value = value.ToLower();
            if (value[0] == '*')
            {
                value = value.Substring(1);
                var map = mPropertyInnerMap.Peek();
                map[key] = value;
            }
        }

        //继承获得属性
        public string getProperty(string key)
        {
            if (key == null && key.Count() == 0) return null;
            if (mPropertyderived.Count == 0) return null;
            if (!mPropertyderived.Peek())
            {
                return null;
            }
            if (mPropertyInnerMap.Count == 0) return null;
            var map = mPropertyInnerMap.Peek();
            string ret = null;
            map.TryGetValue(key, out ret);
            //向上继承
            if (ret == null)
            {
                var m = mPropertyInnerMap.Pop();
                var derived = mPropertyderived.Pop();
                ret = getProperty(key);
                pushProperty(m, derived);
            }
            else if (ret == "")//no invalid
            {
                return null;
            }
            return ret;
        }

        internal void popProperty()
        {
            mPropertyInnerMap.Pop();
            mPropertyderived.Pop();
        }

        void innerTemplatePush(string name, XmlNode node)
        {
            Stack<XmlElement> st;
            if (!mName2InnerTemplate.TryGetValue(name, out st))
            {
                st = new Stack<XmlElement>();
                mName2InnerTemplate.Add(name, st);
            }
            st.Push(node as XmlElement);
        }

        void innerTemplatePop(string name)
        {
            Stack<XmlElement> st;
            if (!mName2InnerTemplate.TryGetValue(name, out st))
            {
                throw new Exception("innerTemplatePop(" + name + ")" + " failed");
            }
            else
            {
                st.Pop();
            }
        }


        protected XmlNode innerTemplateTop(string name)
        {
            Stack<XmlElement> st;
            if (!mName2InnerTemplate.TryGetValue(name, out st))
            {
                throw new Exception("innerTemplateTop(" + name + ")" + " failed");
            }
            else
            {
                return st.Peek();
            }
        }

        public delegate XmlNodeList funcFromXML(XmlNode nd, out UIWidget ui, UIWidget p);

        static Dictionary<string, funcFromXML> mXML2widget = new Dictionary<string, funcFromXML>();

        public static void regMethodFromXML(string tag, funcFromXML method)
        {
            mXML2widget.Add(tag, method);
        }

        UIWidget loadFromXMLNode(XmlNode node, UIWidget p, XmlNode pnode, out string obtainInnerTemplateName)
        {
            UIWidget uiret = null;
            funcFromXML fromxmlFunc = null;
            obtainInnerTemplateName = null;
            //attribute inherite
            if (pnode != null)
            {
                var patts = pnode.Attributes;
                var nodeAttName = (node as XmlElement).GetAttribute("name");
                foreach (XmlAttribute patt in patts)
                {
                    var parts = patt.Name.Split('.');
                    if (parts.Count() > 1)
                    {
                        if (parts[0] == nodeAttName)
                        {
                            var name = parts.Skip(1).Aggregate("",
                                (si, str) => (si + (si == "" ? "" : ".") + str)
                                );
                            var value = patt.Value;
                            (node as XmlElement).SetAttribute(name, value);
                        }
                        else
                        {
                            (node as XmlElement).SetAttribute(patt.Name, patt.Value);
                        }
                    }
                }
            }

            //xml to widget, and children
            if (mXML2widget.TryGetValue(node.Name, out fromxmlFunc))
            {
                var uichildren = fromxmlFunc(node, out uiret, p);
                if (uichildren != null)
                {
                    loadXMLChildren(uichildren, uiret, node);
                }
            }

            //read template
            else if (node.Name.ToLower() == "template")//controller template
            {
                var ret = node.Attributes.GetNamedItem("name");
                if (ret != null)
                {
                    var templateName = ret.Value;
                    XmlElement templateNode = null;
                    if (mName2Template.TryGetValue(templateName, out templateNode))
                    {
                        throw new Exception("templateName:" + templateName + " is duplicate");
                    }
                    else
                    {
                        var childrens = node.ChildNodes;
                        if (childrens.Count < 1)
                        {
                            throw new Exception("no content in template:" + templateName);
                        }
                        else
                        {
                            templateNode = childrens[0] as XmlElement;
                            mName2Template[templateName] = templateNode;
                        }
                    }
                }
                else
                {
                    //exception
                }
                return null;
            }
            else if (node.Name.ToLower() == "innertemplate")
            {
                var ret = node.Attributes.GetNamedItem("name");
                if (ret != null)
                {
                    var templateName = ret.Value;
                    XmlElement templateNode = null;
                    var childrens = node.ChildNodes;
                    if (childrens.Count < 1)
                    {
                        throw new Exception("no content in template:" + templateName);
                    }
                    else
                    {
                        templateNode = childrens[0] as XmlElement;
                        innerTemplatePush(templateName, templateNode);
                        obtainInnerTemplateName = templateName;
                    }
                }
                else
                {
                    throw new Exception("innerTemplate has no 'name' property");
                }
                return null;
            }
            //apply template
            else if (node.Name.ToLower() == "apply")
            {
                var ret = node.Attributes.GetNamedItem("template");
                if (ret != null)
                {
                    var templateName = ret.Value;
                    XmlElement templateNode = null;
                    if (mName2Template.TryGetValue(templateName, out templateNode))
                    {
                        var applyNode = templateNode.CloneNode(true);

                        //挂上node的子结点
                        for (int i = 0; i < node.ChildNodes.Count; ++i)
                        {
                            applyNode.AppendChild(node.ChildNodes[i]);
                        }

                        foreach (XmlAttribute att in node.Attributes)
                        {
                            (applyNode as XmlElement).SetAttribute(att.Name, att.Value);
                        }
                        string innerTemplateNameRet = null;
                        var nodeRet = loadFromXMLNode(applyNode, p, pnode, out innerTemplateNameRet);
                        if (innerTemplateNameRet != null)
                        {
                            innerTemplatePop(innerTemplateNameRet);
                        }
                        return nodeRet;
                    }
                    else
                    {
                        throw new Exception("no template for " + templateName);
                    }
                }
                else
                {
                    ret = node.Attributes.GetNamedItem("innerTemplate");
                    if (ret != null)
                    {
                        var innerTemplateName = ret.Value;
                        XmlNode innerTemplateNode = innerTemplateTop(innerTemplateName);
                        if (innerTemplateNode == null)
                        {
                            throw new Exception("no innerTemplate for " + innerTemplateName);
                        }

                        var applyNode = innerTemplateNode.CloneNode(true);
                        //挂上node的子结点
                        for (int i = 0; i < node.ChildNodes.Count; ++i)
                        {
                            applyNode.AppendChild(node.ChildNodes[i]);
                        }

                        foreach (XmlAttribute att in node.Attributes)
                        {
                            (applyNode as XmlElement).SetAttribute(att.Name, att.Value);
                        }
                        string innerTemplateNameRet = null;
                        var nodeRet = loadFromXMLNode(applyNode, p, pnode, out innerTemplateNameRet);
                        if (innerTemplateNameRet != null)
                        {
                            innerTemplatePop(innerTemplateNameRet);
                        }
                        return nodeRet;
                    }
                    else
                    {
                        throw new Exception("no innerTemplate property for apply");
                    }
                }
            }
            return uiret;
        }

        internal void loadXMLChildren(XmlNodeList uichildren, UIWidget p, XmlNode pnode)
        {
            List<string> innerTemplateNames = new List<string>();
            pushProperty();
            foreach (XmlNode elem in uichildren)
            {
                string innerTemplateName = null;
                var uichild = loadFromXMLNode(elem, p, pnode, out innerTemplateName);
                if (innerTemplateName != null) innerTemplateNames.Add(innerTemplateName);
                //var v = elem.Attributes.GetNamedItem("height");
                //Console.WriteLine(v==null?':v.Value );
                if (uichild != null)
                {
                    uichild.paresent = p;
                }
            }
            popProperty();
            foreach (string e in innerTemplateNames)
            {
                innerTemplatePop(e);
            }
        }

        public UIWidget loadFromXML(string xml)
        {
            UIWidget nodeRet = null;
            //pushProperty();
            try
            {   
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var node = doc.ChildNodes[0];
                string innerTemplateNameRet = null;
                nodeRet = loadFromXMLNode(node, null, null, out innerTemplateNameRet);
                if (innerTemplateNameRet != null)
                {
                    innerTemplatePop(innerTemplateNameRet);
                }
            }
            catch (Exception e) { mLog(e.Message ); }
            //popProperty();
            return nodeRet;
        }
        #endregion

        #region init
        public UIRoot initXML()
        {
            regMethodFromXML("stub", UIStub.fromXML);
            regMethodFromXML("map", UIMap.fromXML);
            regMethodFromXML("blank", UIBlank.fromXML);
            regMethodFromXML("triangle", UIArrow.fromXML);
            regMethodFromXML("rect", UIRect.fromXML);
            regMethodFromXML("lable", UILable.fromXML);
            regMethodFromXML("edit", UIEdit.fromXML);
            regMethodFromXML("line", UILine.fromXML);
            regMethodFromXML("round_rect", UIRoundRect.fromXML);
            regMethodFromXML("round", UIRound.fromXML);
            regMethodFromXML("resizer", UIResizer.fromXML);
            pushProperty();
            return this;
        }

        public UIRoot initXMLWidget(string tagName, funcFromXML xmlConstruct)
        {
            regMethodFromXML(tagName, xmlConstruct);
            return this;
        }

        Action<string> mLog;
        public UIRoot initHandleLog(Action<string> log)
        {
            mLog = log;
            return this;
        }

        public UIRoot initTimer(Func<UInt32> timer)
        {
            TimerManager.Init(timer);
            return this;
        }

        public void updateTimer()
        {
            TimerManager.tickAll();
        }

        public UIRoot initEvt()
        {
            evtLeftDown += testLeftDown;
            evtMove += testMove;
            evtLeftUp += testLeftUp;
            mEvtRightDown += testRightDown;
            mEvtRightUp += testRightUp;
            mEvtMiddleDown += testMiddleDown;
            mEvtMiddleUp += testMiddleUp;
            mEvtDoubleClick += testDoubleClick;
            mEvtKey += testKey;
            return this;
        }

        #endregion

        #region handle
        public void handleDraw(Graphics g)
        {
            if (mDirtyRootOld == null) { mDirtyRootOld = root; }
            //Console.WriteLine(mDirtyRootOld.name);
            mDirtyRootOld.doDrawAlone(g);
            mDirtyRootOld = mDirtyRoot;
            mDirtyRoot = null;
        }

        public void handleLeftDown(int x, int y)
        {
            if (evtLeftDown != null) evtLeftDown(x, y);
        }

        public void handleEvtMove(int x, int y)
        {
            if (evtMove != null)
            {
                evtMove(x, y);
            }
        }

        public void handleEvtLeftUp(int x, int y)
        {
            if (evtLeftUp != null) evtLeftUp(x, y);
        }

        public void handleEvtRightDown(int x, int y)
        {
            if (mEvtRightDown != null) mEvtRightDown(x, y);
        }

        public void handleEvtRightUp(int x, int y)
        {
            if (mEvtRightUp != null) mEvtRightUp(x, y);
        }

        public void handleEvtMiddleDown(int x, int y)
        {
            if (mEvtMiddleDown != null) mEvtMiddleDown(x, y);
        }

        public void handleEvtMiddleUp(int x, int y)
        {
            if (mEvtMiddleUp != null) mEvtMiddleUp(x, y);
        }

        public void handleEvtDoubleClick(int x, int y)
        {
            if (mEvtDoubleClick != null) mEvtDoubleClick(x, y);
        }

        public void handleEvtWheel(int delta)
        {
            if (mEvtWheel != null) mEvtWheel(delta);
        }

        public void handleEvtKey(int kc, bool iC, bool iS)
        {
            if (mEvtKey != null) mEvtKey(kc, iC, iS);
        }

        #endregion

        #region focus
        internal UIWidget focusWidget
        {
            get
            {
                object o;
                bool ret = root.attrs.TryGetValue("focus", out o);
                if (!ret) return null;
                return o as UIWidget;
            }
            set
            {
                root.attrs["focus"] = value;
            }
        }

        UIWidget mCurrent = null;
        internal UIWidget currentWidget
        {
            get
            {
                return mCurrent;
            }
            set
            {
                if (value != mCurrent)
                {
                    if (mCurrent != null)
                    {
                        mCurrent.doExit();
                    }
                    if (value != null)
                    {
                        value.doEnter();
                    }
                }
                mCurrent = value;

            }
        }

        internal bool lockWidget
        {
            get
            {
                return (focusWidget != null);
            }
            set
            {
                if (value)
                    focusWidget = currentWidget;
                else
                    focusWidget = null;
            }

        }
        #endregion
        
        #region event
        static int count = 0;
        internal void testUIEvent(int x, int y, Func<UIWidget, Func<int, int, bool>> getAction)
        {
            UIWidget uiout;
            bool ret = true;
            uiout = focusWidget;
            if (uiout == null)
                ret = root.doTestPick(new Point(x, y), out uiout);
            if (ret)
            {
                if (currentWidget != uiout) 
                    currentWidget = uiout;
                
                count++;
                while (uiout != null)
                {
                    var act = getAction(uiout);
                    if (act != null)
                    {
                        if (!act(x, y))
                        {
                            break;//consumed
                        }
                    }
                    if (!uiout.propagate)
                    {
                        break;
                    }

                    uiout = uiout.paresent as UIWidget;
                }
            }
        }

        public Action<int, int> evtLeftDown;
        void testLeftDown(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtLeftDown(x1, y1); };
            });
        }

        public event Action<int, int> evtMove;
        public int cursorX;
        public int cursorY;
        void testMove(int x, int y)
        {
            cursorX = x;
            cursorY = y;
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.evtMove(x1, y1); };
            });
        }

        public event Action<int, int> evtLeftUp;
        void testLeftUp(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtLeftUp(x1, y1); };
            });
        }


        internal Action<int, int> mEvtRightDown;

        void testRightDown(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtRightDown(x1, y1); };
            });
        }


        internal Action<int, int> mEvtRightUp;

        void testRightUp(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtRightUp(x1, y1); };
            });
        }


        internal Action<int, int> mEvtMiddleDown;
        void testMiddleDown(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtMiddleDown(x1, y1); };
            });
        }


        internal Action<int, int> mEvtMiddleUp;
        void testMiddleUp(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtMiddleUp(x1, y1); };
            });
        }


        internal Action<int, int> mEvtDoubleClick;
        void testDoubleClick(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtDoubleClick(x1, y1); };
            });
        }


        internal Action<int, bool, bool> mEvtKey;
        void testKey(int kc, bool isC, bool isS)
        {
            var ui = currentWidget;
            while (ui != null)
            {
                if (!ui.doEvtOnChar(kc, isC, isS))
                {
                    return;
                }
                ui = ui.paresent as UIWidget;
            }
        }



        internal Action<int> mEvtWheel;

        internal Action mHandleDraw;
        public UIRoot initHandleDraw(Action handleDraw)
        {
            mHandleDraw = handleDraw;
            return this;
        }


        internal Action<bool, int, int> mHandleInputShow;
        public UIRoot initHandleInputShow(Action<bool, int, int> handleInputShow)
        {
            mHandleInputShow = handleInputShow;
            return this;
        }

        internal Action<string> mEvtInputDone;
        public void handleInputShow(string str)
        {
            if (mEvtInputDone != null) mEvtInputDone(str);

        }
        #endregion

        #region manager
        //ID manager
        Dictionary<string, UIWidget> mName2Widget = new Dictionary<string, UIWidget>();
        internal UIWidget getIDWidget(string strID)
        {
            UIWidget ret = null;
            mName2Widget.TryGetValue(strID, out ret);
            return ret;
        }

        internal void setIDWidget(string name, UIWidget ui)
        {
            var w = getIDWidget(name);
            if (w != null) throw new Exception("already has UI with ID:" + name + " ," + w.typeName);
            mName2Widget.Add(name, ui);
        }

        internal void clearIDMap()
        {
            mName2Widget.Clear();
        }

        #endregion
    }
}
