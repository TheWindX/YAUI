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


namespace ns_behaviour
{
    class UIRoot : Singleton<UIRoot>
    {
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
            Globals.Instance.evtOnInit += init;
        }

        //xml
        protected Dictionary<string, XmlElement> mName2Template = new Dictionary<string, XmlElement>();

        public delegate XmlNodeList funcFromXML(XmlNode nd, out UIWidget ui, UIWidget p);

        static Dictionary<string, funcFromXML> mXML2widget = new Dictionary<string, funcFromXML>();

        public static void regMethodFromXML(string tag, funcFromXML method)
        {
            mXML2widget.Add(tag, method);
        }

        void XMLinit()
        {
            regMethodFromXML("stub", UIStub.fromXML);
            regMethodFromXML("map", UIMap.fromXML);
            regMethodFromXML("rect", UIRect.fromXML);
            regMethodFromXML("lable", UILable.fromXML);
            regMethodFromXML("edit", UIEdit.fromXML);
            regMethodFromXML("line", UILine.fromXML);
        }

        UIWidget loadFromXMLNode(XmlNode node, UIWidget p, XmlNode pnode)
        {
            UIWidget uiret = null;
            funcFromXML fromxmlFunc = null;

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
                                (si, str) => (si + (si == "" ? "" : ",") + str)
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
            if(mXML2widget.TryGetValue(node.Name, out fromxmlFunc) )
            {
                var uichildren = fromxmlFunc(node, out uiret, p);
                foreach (XmlNode elem in uichildren)
                {
                    var uichild = loadFromXMLNode(elem, uiret, node);
                    if (uichild != null)
                    {
                        uichild.paresent = uiret;
                    }
                }
            }

            //read template
            else if(node.Name.ToLower() == "template")//controller template
            {
                var ret = node.Attributes.GetNamedItem("name");
                if (ret != null)
                {
                    var templateName = ret.Value;
                    XmlElement templateNode = null;
                    if (mName2Template.TryGetValue(templateName, out templateNode))
                    {
                        //TODO, 已经存在这个node, exception
                        
                    }
                    else
                    {
                        var childrens = node.ChildNodes;
                        if (childrens.Count < 1)
                        {
                            //TODO, no content for controll
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
                        foreach(XmlAttribute att in node.Attributes)
                        {
                            (applyNode as XmlElement).SetAttribute(att.Name, att.Value);
                        }
                        return loadFromXMLNode(applyNode, p, pnode);
                    }
                    else
                    {
                        //TODO, template node不存在, exception
                    }
                }
                else
                {
                    //Exception
                }
                return null;
            }
            return uiret;
        }

        public UIWidget loadFromXML(string xml)//加上一个style默认，style可以是一个状态机
        {
            UIWidget ret = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var node = doc.ChildNodes[0];
                ret = loadFromXMLNode(node, null, null);
            }
            catch (Exception e){ Globals.Instance.mRepl.print(e.ToString() ); }
            return ret;
        }

        void init()
        {
            XMLinit();

            mRoot = new UIMap();

            Globals.Instance.mPainter.evtPaint += (g) =>
            {
                Instance.draw(g);
            };
            Globals.Instance.mPainter.evtMove += (x, y) =>
            {
                Instance.testMouseMove(x, y);
            };
            Globals.Instance.mPainter.evtLeftDown += (x, y) =>
            {
                Instance.testLMD(x, y);
            };
            Globals.Instance.mPainter.evtLeftUp += (x, y) =>
            {
                Instance.testLMU(x, y);
            };
            Globals.Instance.mPainter.evtRightDown += (x, y) =>
            {
                Instance.testRMD(x, y);
            };
            Globals.Instance.mPainter.evtRightUp += (x, y) =>
            {
                Instance.testRMU(x, y);
            };
            Globals.Instance.mPainter.evtMidDown += (x, y) =>
            {
                Instance.testMMD(x, y);
            };
            Globals.Instance.mPainter.evtMidUp += (x, y) =>
            {
                testMMU(x, y);
            };
            Globals.Instance.mPainter.evtDClick += (x, y) =>
            {
                testDClick(x, y);
            };

            //keys
            Globals.Instance.mPainter.evtOnKey += (kc, isC, isS) =>
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
                };
        }

        public void draw(Graphics g)
        {
            root.adjustLayout();
            root.doDraw(g);
        }

        void onKeyLock(int kc, bool isControl, bool isShift)
        {
            if (kc == (int)System.Windows.Forms.Keys.Space)
            {
                if (lockWidget)
                {
                    lockWidget = false;
                }
                else
                {
                    lockWidget = true;
                }
            }
        }
        bool mLockable = false;
        void setLock()
        {
            if (mLockable == true) return;
            mLockable = true;
            Globals.Instance.mPainter.evtOnKey += onKeyLock;
        }

        void unsetLock()
        {
            if (mLockable == false) return;
            mLockable = false;
            Globals.Instance.mPainter.evtOnKey -= onKeyLock;
        }

        public bool lockable
        {
            get
            {
                return mLockable;
            }
            set
            {
                if (value)
                    setLock();
                else
                    unsetLock();
            }
        }

        public UIWidget focusWidget
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

        public UIWidget currentWidget
        {
            get
            {
                object o;
                bool ret = root.attrs.TryGetValue("current", out o);
                if (!ret) return null;
                return o as UIWidget;
            }
            set
            {
                root.attrs["current"] = value;
            }
        }

        public bool lockWidget
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

        public void testUIEvent(int x, int y, Func<UIWidget, Func<int, int, bool>> getAction)
        {
            UIWidget uiout;
            bool ret = true;
            uiout = focusWidget;
            if(uiout == null)
                ret  = root.doTestPick(new Point(x, y), out uiout);
            if (ret)
            {
                currentWidget = uiout;
                while (uiout != null)
                {
                    var act = getAction(uiout);
                    if (act != null)
                    {
                        if (!act(x, y) )
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

        public void testLMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {   
                return (x1, y1) => { return ui.doEvtOnLMDown(x1, y1); };
            });
        }

        public void testMouseMove(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnMMove(x1, y1); };
            });
        }

        public void testLMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
               return (x1, y1) => { return ui.doEvtOnLMUp(x1, y1); };
            });
        }

        public void testRMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnRMDown(x1, y1); };
            });
        }

        public void testRMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnRMUp(x1, y1); };
            });
        }

        public void testMMD(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnMMDown(x1, y1); };
            });
        }

        public void testMMU(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnMMUp(x1, y1); };
            });
        }

        public void testDClick(int x, int y)
        {
            testUIEvent(x, y, (ui) =>
            {
                return (x1, y1) => { return ui.doEvtOnDClick(x1, y1); };
            });
        }
    }
}
