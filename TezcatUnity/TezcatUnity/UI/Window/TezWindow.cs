using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Unity.Database;
using UnityEngine;

namespace tezcat.Unity.UI
{
    /// <summary>
    /// Window本身只包含Area
    /// 用于划分其中的显示区域
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
                if (mFullName.isNullOrEmpty())
                {
                    mFullName = mWindowName + mWindowID;
                }

                return mFullName;
            }
        }

        /// <summary>
        /// Area
        /// </summary>
        List<TezSubwindow> mSubwindowList = new List<TezSubwindow>();
        Dictionary<string, int> mSubwindowDic = new Dictionary<string, int>();
        Queue<int> mFreeID = new Queue<int>();

        protected ITezFocusableWidget mFocusWidget = null;
        protected TezWidgetEvent.Dispatcher mEventDispatcher = new TezWidgetEvent.Dispatcher();

        #region Core
        protected override void preInit()
        {
            mEventDispatcher.register(TezWidgetEvent.ShowArea,
            (object data) =>
            {
                int id = -1;
                if (mSubwindowDic.TryGetValue((string)data, out id))
                {
                    mSubwindowList[id].open();
                }
            });
        }

        protected override void initWidget()
        {
            List<TezSubwindow> list = new List<TezSubwindow>();
            this.GetComponentsInChildren(true, list);
            foreach (var area in list)
            {
                this.registerSubwindow(area);
            }
        }

        protected override void onHide()
        {

        }

        public override void reset()
        {
            foreach (var area in mSubwindowList)
            {
                area.reset();
            }
        }

        protected override void onClose(bool self_close)
        {
            for (int i = 0; i < mSubwindowList.Count; i++)
            {
                mSubwindowList[i].close();
            }
            mSubwindowList.Clear();
            mSubwindowList = null;

            mSubwindowDic.Clear();
            mSubwindowDic = null;

            TezcatUnity.removeWindow(this);
        }

        protected override void onRefresh()
        {
            foreach (var sub in mSubwindowList)
            {
                sub.needRefresh();
            }
        }

        public void setFocusWidget(ITezFocusableWidget widget)
        {
            mFocusWidget = widget;
        }
        #endregion

        #region Area
        private void growSubwindow(int id)
        {
            while (mSubwindowList.Count <= id)
            {
                mFreeID.Enqueue(mSubwindowList.Count);
                mSubwindowList.Add(null);
            }
        }

        private int giveSubwindowID()
        {
            int id = -1;
            if (mFreeID.Count > 0)
            {
                id = mFreeID.Dequeue();
                while (mSubwindowList[id])
                {
                    if (mFreeID.Count == 0)
                    {
                        id = -1;
                        break;
                    }
                    id = mFreeID.Dequeue();
                }
            }

            if (id == -1)
            {
                id = mSubwindowList.Count;
                mSubwindowList.Add(null);
            }

            return id;
        }

        private void registerSubwindow(TezSubwindow subwindow)
        {
#if UNITY_EDITOR
            TezService.get<TezDebug>().isTrue(subwindow.subwindowID < 0, "UIWindow (" + m_WindowName + ")", "Window (" + subwindow.subwindowName + ") ID Must EqualGreater Than 0");
#endif
            subwindow.subwindowID = this.giveSubwindowID();
            this.growSubwindow(subwindow.subwindowID);

            if (string.IsNullOrEmpty(subwindow.subwindowName))
            {
                subwindow.subwindowName = "Area_" + subwindow.subwindowID;
            }

            if (mSubwindowList[subwindow.subwindowID])
            {
                subwindow.subwindowID = this.giveSubwindowID();
            }
            subwindow.window = this;
            mSubwindowList[subwindow.subwindowID] = subwindow;
            mSubwindowDic.Add(subwindow.subwindowName + subwindow.subwindowID, subwindow.subwindowID);

#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Register Area: " + subwindow.subwindowName + " ID:" + subwindow.subwindowID);
#endif
        }


        public void addSubwindow(TezSubwindow subwindow)
        {
            if (subwindow.subwindowID != -1 && !mSubwindowDic.ContainsKey(subwindow.subwindowName + subwindow.subwindowID))
            {
                this.growSubwindow(subwindow.subwindowID);

                if (string.IsNullOrEmpty(subwindow.subwindowName))
                {
                    subwindow.subwindowName = "Area_" + subwindow.subwindowID;
                }

                subwindow.subwindowID = this.giveSubwindowID();
                subwindow.window = this;
                mSubwindowList[subwindow.subwindowID] = subwindow;
                mSubwindowDic.Add(subwindow.subwindowName + subwindow.subwindowID, subwindow.subwindowID);

#if UNITY_EDITOR
                TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Add Area: " + subwindow.subwindowName + " ID:" + subwindow.subwindowID);
#endif
            }
        }

        public void removeSubwindow(int subwindow_id)
        {
            if(mSubwindowList != null)
            {
                var area = mSubwindowList[subwindow_id];
                mSubwindowDic.Remove(area.subwindowName + area.subwindowID);
                mFreeID.Enqueue(subwindow_id);
                mSubwindowList[subwindow_id] = null;
            }
        }

        public T getSubwindow<T>() where T : TezSubwindow
        {
            foreach (var area in mSubwindowList)
            {
                if (area is T)
                {
                    return (T)area;
                }
            }

            return null;
        }

        public T getSubwindow<T>(string name) where T : TezSubwindow
        {
            int id = -1;
            if (mSubwindowDic.TryGetValue(name, out id))
            {
                return (T)mSubwindowList[id];
            }

            return null;
        }

        public T getSubwindow<T>(int id) where T : TezSubwindow
        {
            if (id > mSubwindowList.Count || id < 0)
            {
                return null;
            }

            return (T)mSubwindowList[id];
        }

        public void onSubwindowNameChanged(TezSubwindow subwindow, string new_name)
        {
#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Area Name: " + subwindow.subwindowName + " Change To: " + new_name);
#endif
            mSubwindowDic.Remove(subwindow.subwindowName + subwindow.subwindowID);
            mSubwindowDic.Add(new_name + subwindow.subwindowID, subwindow.subwindowID);
        }
        #endregion

        #region Event
        public void dispathEvent(int event_id, object data)
        {
            foreach (var area in mSubwindowList)
            {
                area.onEvent(event_id, data);
            }
        }

        public void onEvent(int event_id, object data)
        {
            mEventDispatcher.invoke(event_id, data);
        }
        #endregion
    }
}