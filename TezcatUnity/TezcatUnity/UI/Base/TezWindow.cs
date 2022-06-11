using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// Window本身只包含Area
    /// 用于划分其中的显示区域
    /// </summary>
    public abstract class TezWindow
        : TezUIWidget
        , ITezSinglePrefab
    {
        TezLayer m_Layer = null;
        public TezLayer layer
        {
            get
            {
                if (m_Layer == null)
                {
                    var parent = this.transform.parent;
                    while (true)
                    {
                        var layer = parent.GetComponent<TezLayer>();
                        if (layer)
                        {
                            m_Layer = layer;
                            break;
                        }
                        else
                        {
                            parent = parent.parent;
                        }
                    }
                }

                return m_Layer;
            }
            set
            {
                m_Layer = value;
            }
        }

        [SerializeField]
        int m_WindowID = -1;
        public int windowID
        {
            get { return m_WindowID; }
            set { m_WindowID = value; }
        }

        [SerializeField]
        private string m_WindowName = null;
        public string windowName
        {
            get
            {
                if (string.IsNullOrEmpty(m_WindowName))
                {
                    m_WindowName = this.name;
                }

                return m_WindowName;
            }
            set
            {
                m_WindowName = value;
                this.name = m_WindowName;
            }
        }

        string m_FullName = null;
        public string fullName
        {
            get
            {
                if (m_FullName.isNullOrEmpty())
                {
                    m_FullName = m_WindowName + m_WindowID;
                }

                return m_FullName;
            }
        }

        /// <summary>
        /// Area
        /// </summary>
        List<TezSubwindow> m_SubwindowList = new List<TezSubwindow>();
        Dictionary<string, int> m_SubwindowDic = new Dictionary<string, int>();
        Queue<int> m_FreeID = new Queue<int>();

        protected ITezFocusableWidget m_FocusWidget = null;
        protected TezWidgetEvent.Dispatcher m_EventDispatcher = new TezWidgetEvent.Dispatcher();

        #region Core
        protected override void preInit()
        {
            m_EventDispatcher.register(TezWidgetEvent.ShowArea,
            (object data) =>
            {
                int id = -1;
                if (m_SubwindowDic.TryGetValue((string)data, out id))
                {
                    m_SubwindowList[id].open();
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
            foreach (var area in m_SubwindowList)
            {
                area.reset();
            }
        }

        protected override void onClose(bool self_close)
        {
            for (int i = 0; i < m_SubwindowList.Count; i++)
            {
                m_SubwindowList[i].close();
            }
            m_SubwindowList.Clear();
            m_SubwindowList = null;

            m_SubwindowDic.Clear();
            m_SubwindowDic = null;

            TezcatUnity.instance.removeWindow(this);
        }

        protected override void onRefresh()
        {
            foreach (var sub in m_SubwindowList)
            {
                sub.refreshMask = true;
            }
        }

        public void setFocusWidget(ITezFocusableWidget widget)
        {
            m_FocusWidget = widget;
        }
        #endregion

        #region Area
        private void growSubwindow(int id)
        {
            while (m_SubwindowList.Count <= id)
            {
                m_FreeID.Enqueue(m_SubwindowList.Count);
                m_SubwindowList.Add(null);
            }
        }

        private int giveSubwindowID()
        {
            int id = -1;
            if (m_FreeID.Count > 0)
            {
                id = m_FreeID.Dequeue();
                while (m_SubwindowList[id])
                {
                    if (m_FreeID.Count == 0)
                    {
                        id = -1;
                        break;
                    }
                    id = m_FreeID.Dequeue();
                }
            }

            if (id == -1)
            {
                id = m_SubwindowList.Count;
                m_SubwindowList.Add(null);
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

            if (m_SubwindowList[subwindow.subwindowID])
            {
                subwindow.subwindowID = this.giveSubwindowID();
            }
            subwindow.window = this;
            m_SubwindowList[subwindow.subwindowID] = subwindow;
            m_SubwindowDic.Add(subwindow.subwindowName + subwindow.subwindowID, subwindow.subwindowID);

#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Register Area: " + subwindow.subwindowName + " ID:" + subwindow.subwindowID);
#endif
        }


        public void addSubwindow(TezSubwindow subwindow)
        {
            if (subwindow.subwindowID != -1 && !m_SubwindowDic.ContainsKey(subwindow.subwindowName + subwindow.subwindowID))
            {
                this.growSubwindow(subwindow.subwindowID);

                if (string.IsNullOrEmpty(subwindow.subwindowName))
                {
                    subwindow.subwindowName = "Area_" + subwindow.subwindowID;
                }

                subwindow.subwindowID = this.giveSubwindowID();
                subwindow.window = this;
                m_SubwindowList[subwindow.subwindowID] = subwindow;
                m_SubwindowDic.Add(subwindow.subwindowName + subwindow.subwindowID, subwindow.subwindowID);

#if UNITY_EDITOR
                TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Add Area: " + subwindow.subwindowName + " ID:" + subwindow.subwindowID);
#endif
            }
        }

        public void removeSubwindow(int subwindow_id)
        {
            if(m_SubwindowList != null)
            {
                var area = m_SubwindowList[subwindow_id];
                m_SubwindowDic.Remove(area.subwindowName + area.subwindowID);
                m_FreeID.Enqueue(subwindow_id);
                m_SubwindowList[subwindow_id] = null;
            }
        }

        public T getSubwindow<T>() where T : TezSubwindow
        {
            foreach (var area in m_SubwindowList)
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
            if (m_SubwindowDic.TryGetValue(name, out id))
            {
                return (T)m_SubwindowList[id];
            }

            return null;
        }

        public T getSubwindow<T>(int id) where T : TezSubwindow
        {
            if (id > m_SubwindowList.Count || id < 0)
            {
                return null;
            }

            return (T)m_SubwindowList[id];
        }

        public void onSubwindowNameChanged(TezSubwindow subwindow, string new_name)
        {
#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Area Name: " + subwindow.subwindowName + " Change To: " + new_name);
#endif
            m_SubwindowDic.Remove(subwindow.subwindowName + subwindow.subwindowID);
            m_SubwindowDic.Add(new_name + subwindow.subwindowID, subwindow.subwindowID);
        }
        #endregion

        #region Event
        public void dispathEvent(int event_id, object data)
        {
            foreach (var area in m_SubwindowList)
            {
                area.onEvent(event_id, data);
            }
        }

        public void onEvent(int event_id, object data)
        {
            m_EventDispatcher.invoke(event_id, data);
        }
        #endregion
    }
}