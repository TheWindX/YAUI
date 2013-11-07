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
            GlobalInit.Instance.evtOnInit += this.main;
        }

        UILineLinker.cons mTester;

        CEditCreate mCreate;
        CEditMoveFragment mMoveFrag;
        void main()
        {
            var r = UIRoot.Instance.mRoot;
            mTester = new UILineLinker.cons(r);

            var rc = new UIRect(128, 64, 0x88ffffff, 0x33333333);
            rc.setParesent(UIRoot.Instance.mRoot);
            mCreate = new CEditCreate(this);
            mMoveFrag = new CEditMoveFragment(this);

            GlobalInit.Instance.mPainter.evtOnKey += onKey;
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
                GlobalInit.Instance.mPainter.evtLeftDown += onClick;
            }

            public override void onRemove()
            {
                GlobalInit.Instance.mPainter.evtLeftDown -= onClick;
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
                Point orgUiPos = mNodeSelect.mPos;

                var onMove = new PaintDriver.EvtMove( (px, py) =>
                    {
                        int dx = px - x;
                        int dy = py - y;
                        Point newUiPos = new Point(orgUiPos.X + dx, orgUiPos.Y + dy);
                        mNodeSelect.adjustPos(newUiPos);
                    } );

                GlobalInit.Instance.mPainter.evtLeftUp = (px, py) =>
                    {
                        GlobalInit.Instance.mPainter.evtMove -= onMove;
                    };

                GlobalInit.Instance.mPainter.evtMove += onMove;

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
        void onKey(int kc)
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
            if (kc == (int)System.Windows.Forms.Keys.M)
            {
                if (mCurrentEdit != null)
                {
                    mCurrentEdit.onRemove();
                }
                mCurrentEdit = mMoveFrag;
                mCurrentEdit.onAdd();
            }

        }

    }
}
