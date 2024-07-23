using tezcat.Unity.Database;
using UnityEngine;

namespace tezcat.Unity.UI
{

    /// <summary>
    /// UI窗口
    /// 窗口中包含各种UI设计
    /// </summary>
    public abstract class TezWindow
        : TezUIWidget
        , ITezSinglePrefab
    {
        TezLayer mLayer = null;
        public TezLayer layer
        {
            get
            {
                if (mLayer == null)
                {
                    var parent = this.transform.parent;
                    while (true)
                    {
                        var layer = parent.GetComponent<TezLayer>();
                        if (layer)
                        {
                            mLayer = layer;
                            break;
                        }
                        else
                        {
                            parent = parent.parent;
                        }
                    }
                }

                return mLayer;
            }
            set
            {
                mLayer = value;
            }
        }

        [SerializeField]
        int mWindowID = -1;
        public int windowID
        {
            get { return mWindowID; }
            set { mWindowID = value; }
        }

        [SerializeField]
        private string mWindowName = null;
        public string windowName
        {
            get
            {
                if (string.IsNullOrEmpty(mWindowName))
                {
                    mWindowName = this.name;
                }

                return mWindowName;
            }
            set
            {
                mWindowName = value;
                this.name = mWindowName;
            }
        }

        string mFullName = null;
        public string fullName
        {
            get
            {
                if (string.IsNullOrEmpty(mFullName))
                {
                    mFullName = mWindowName + mWindowID;
                }

                return mFullName;
            }
        }

        protected ITezFocusableWidget mFocusWidget = null;
        protected TezWidgetEvent.Dispatcher mEventDispatcher = new TezWidgetEvent.Dispatcher();

        #region Core
        protected override void preInit()
        {
            TezUIManager.addWindow(this);
        }

        protected override void initWidget()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected override void onClose(bool self_close)
        {
            TezUIManager.removeWindow(this);
            this.windowID = -1;
        }

        protected override void onRefresh()
        {

        }

        public void setFocusWidget(ITezFocusableWidget widget)
        {
            mFocusWidget = widget;
        }
        #endregion

        #region Event
        public void dispathEvent(int eventID, object data)
        {

        }

        public void onEvent(int eventID, object data)
        {
            mEventDispatcher.invoke(eventID, data);
        }
        #endregion
    }
}