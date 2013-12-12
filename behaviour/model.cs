using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_behaviour
{
    interface iNameItem
    {
        string name
        {
            get;
        }

        string type
        {
            get;
        }
    };


    interface iModel : iNameItem
    {
        iViewerSelf getViewerSelf();    
    }

    interface iModelName : iModel
    {
        iViewerNameItem getViewerNameItem();
    }

    interface iModelBeh : iModel
    {
        iViewerBehItem getViewerBehItem();
    }

    interface iViewer
    {
        iModel getModel();
        UIWidget asWidget();
        Dictionary<string, object> attr
        {
            get;
        }
    }


    interface iViewerSelf : iViewer
    {
    }

    interface iViewerNameItem : iViewer
    {
        void select();
        void unselect();
    }

    interface iViewerBehItem : iViewer
    {
    }


    class GlobalModel
    {
        public static iModel mModelCopy = null;
    }
}
