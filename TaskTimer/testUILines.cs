/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ns_behaviour
{
    class testUILines : Singleton<testUILines>
    {
        public testUILines()
        {
            Globals.Instance.evtOnInit += this.main;
        }

        UILineLinker.cons mTester;

        CEditCreate mCreate;
        CEditMoveFragment mMoveFrag;
        void main()
        {
            var r = UIRoot.Instance.root;
            mTester = new UILineLinker.cons(r);

            mCreate = new CEditCreate(this);
            mMoveFrag = new CEditMoveFragment(this);

            Globals.Instance.mPainter.evtOnKey += onKey;
        }

        class CEditInfo
        {
            public virtual void onAdd()
            {
            }

            public virtual void onRemove()
            {
            }
        }

        class CEditCreate : CEditInfo
        {
            UILineLinker.cons mTester;

            public CEditCreate(testUILines tester)
            {
                mTester = tester.mTester;
            }

            void onClick(int x, int y)
            {
                mTester.lineTo(new Point(x, y));
            }

            public override void onAdd()
            {
                Globals.Instance.mPainter.evtLeftDown += onClick;
            }

            public override void onRemove()
            {
                Globals.Instance.mPainter.evtLeftDown -= onClick;
            }

        }

        class CEditMoveFragment : CEditInfo
        {
            UILineLinker.cons mTester;

            public CEditMoveFragment(testUILines tester)
            {
                mTester = tester.mTester;
            }

            UILineNode mNodeSelect;

            bool selectNode(UIWidget ui, int x, int y)
            {
                var orgMPos = new Point(x, y);
                mNodeSelect = ui as UILineNode;
                Point orgUiPos = mNodeSelect.position;

                var onMove = new PaintDriver.EvtMouse( (px, py) =>
                    {
                        int dx = px - x;
                        int dy = py - y;
                        Point newUiPos = new Point(orgUiPos.X + dx, orgUiPos.Y + dy);
                        mNodeSelect.adjustFromFrontToBothSide(newUiPos);
                    } );

                Globals.Instance.mPainter.evtLeftUp += (px, py) =>
                    {
                        Globals.Instance.mPainter.evtMove -= onMove;
                    };

                Globals.Instance.mPainter.evtMove += onMove;

                return false;
            }

            public override void onAdd()
            {
                var linker = mTester.mLinker;

                foreach (var elem in linker.nodeIter())
                {
                    elem.evtOnLMDown += selectNode;
                }
            }

            public override void onRemove()
            {
                var linker = mTester.mLinker;

                foreach (var elem in linker.nodeIter())
                {
                    elem.evtOnLMDown -= selectNode;
                }
            }
        }

        CEditInfo mCurrentEdit;
        void onKey(int kc, bool isControl, bool isShift)
        {
            if (kc == (int)System.Windows.Forms.Keys.C)
            {
                if (mCurrentEdit != null)
                {
                    mCurrentEdit.onRemove();
                }
                mCurrentEdit = mCreate;
                mCurrentEdit.onAdd();
            }
            else if (kc == (int)System.Windows.Forms.Keys.M)
            {
                if (mCurrentEdit != null)
                {
                    mCurrentEdit.onRemove();
                }
                mCurrentEdit = mMoveFrag;
                mCurrentEdit.onAdd();
            }
            else if (kc == (int)System.Windows.Forms.Keys.Space)
            {
                if (UIRoot.Instance.lockWidget)
                {
                    UIRoot.Instance.lockWidget = false;
                }
                else
                {
                    UIRoot.Instance.lockWidget = true;
                }
            }
        }

    }
}
