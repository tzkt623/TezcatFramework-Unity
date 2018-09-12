using System.Collections.Generic;
using tezcat.Core;
using tezcat.Signal;
using UnityEngine;

namespace tezcat.UI
{
    /// <summary>
    /// Window本身只包含Area
    /// 用于划分其中的显示区域
    /// </summary>
    public abstract class TezWindow : TezWidget
    {
        TezLayer m_Layer = null;
        public TezLayer layer
        {
            get
            {
                if (!m_Layer)
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

        /// <summary>
        /// 当前窗口的覆盖层
        /// </summary>
        public RectTransform overlay { get; private set; }

        /// <summary>
        /// Area
        /// </summary>
        List<TezArea> m_AreaList = new List<TezArea>();
        Dictionary<string, int> m_AreaDic = new Dictionary<string, int>();
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
                if (m_AreaDic.TryGetValue((string)data, out id))
                {
                    m_AreaList[id].open();
                }
            });

            GameObject go = new GameObject();
            overlay = go.AddComponent<RectTransform>();
            overlay.parent = this.transform;
            overlay.localPosition = Vector3.zero;
            overlay.name = "WindowOverlay";
            overlay.setLayoutZeroRect();
            overlay.SetAsLastSibling();
        }

        protected override void initWidget()
        {
            List<TezArea> list = new List<TezArea>();
            this.GetComponentsInChildren<TezArea>(true, list);
            foreach (var area in list)
            {
                this.registerArea(area);
            }
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onShow()
        {

        }

        public override bool checkForClose()
        {
            bool result = true;
            List<TezEventDispatcher.Action> close_function_list = new List<TezEventDispatcher.Action>(m_AreaList.Count);

            foreach (var area in m_AreaList)
            {
                result &= area.checkForClose();
                if (result)
                {
                    close_function_list.Add(area.close);
                }
                else
                {
                    return false;
                }
            }

            foreach (var function in close_function_list)
            {
                function();
            }

            return true;
        }

        public override void reset()
        {
            foreach (var area in m_AreaList)
            {
                area.reset();
            }
        }

        public override void clear()
        {
            overlay = null;

            for (int i = 0; i < m_AreaList.Count; i++)
            {
                m_AreaList[i].close();
            }
            m_AreaList.Clear();
            m_AreaList = null;

            m_AreaDic.Clear();
            m_AreaDic = null;

            TezcatFramework.instance.removeWindow(this);
        }

        protected override void onRefresh()
        {
            foreach (var sub in m_AreaList)
            {
                sub.dirty = true;
            }
        }

        public void setFocusWidget(ITezFocusableWidget widget)
        {
            m_FocusWidget = widget;
        }
        #endregion

        #region Area
        private void growArea(int id)
        {
            while (m_AreaList.Count <= id)
            {
                m_FreeID.Enqueue(m_AreaList.Count);
                m_AreaList.Add(null);
            }
        }

        private int giveID()
        {
            int id = -1;
            if (m_FreeID.Count > 0)
            {
                id = m_FreeID.Dequeue();
                while (m_AreaList[id])
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
                id = m_AreaList.Count;
                m_AreaList.Add(null);
            }

            return id;
        }

        private void registerArea(TezArea area)
        {
#if UNITY_EDITOR
            TezService.get<TezDebug>().isTrue(area.areaID >= 0, "UIWindow (" + m_WindowName + ")", "Window (" + area.areaName + ") ID Must EqualGreater Than 0");
#endif
            this.growArea(area.areaID);

            if (string.IsNullOrEmpty(area.areaName))
            {
                area.areaName = "Area_" + area.areaID;
            }

            if (m_AreaList[area.areaID])
            {
                area.areaID = this.giveID();
            }
            area.window = this;
            m_AreaList[area.areaID] = area;
            m_AreaDic.Add(area.areaName + area.areaID, area.areaID);

#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Register Area: " + area.areaName + " ID:" + area.areaID);
#endif
        }


        public void addArea(TezArea area)
        {
            if (area.areaID != -1 && !m_AreaDic.ContainsKey(area.areaName + area.areaID))
            {
                this.growArea(area.areaID);

                if (string.IsNullOrEmpty(area.areaName))
                {
                    area.areaName = "Area_" + area.areaID;
                }

                area.areaID = this.giveID();
                area.window = this;
                m_AreaList[area.areaID] = area;
                m_AreaDic.Add(area.areaName + area.areaID, area.areaID);

#if UNITY_EDITOR
                TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Add Area: " + area.areaName + " ID:" + area.areaID);
#endif
            }
        }

        public void removeArea(TezArea area)
        {
            m_AreaList[area.areaID] = null;
            m_AreaDic.Remove(area.areaName + area.areaID);
            m_FreeID.Enqueue(area.areaID);
        }

        public T getArea<T>() where T : TezArea
        {
            foreach (var area in m_AreaList)
            {
                if (area is T)
                {
                    return (T)area;
                }
            }

            return null;
        }

        public T getArea<T>(string name) where T : TezArea
        {
            int id = -1;
            if (m_AreaDic.TryGetValue(name, out id))
            {
                return (T)m_AreaList[id];
            }

            return null;
        }

        public T getArea<T>(int id) where T : TezArea
        {
            if (id > m_AreaList.Count || id < 0)
            {
                return null;
            }

            return (T)m_AreaList[id];
        }

        public void onAreaNameChanged(TezArea area, string new_name)
        {
#if UNITY_EDITOR
            TezService.get<TezDebug>().info("UIWindow (" + m_WindowName + ")", "Area Name: " + area.areaName + " Change To: " + new_name);
#endif
            m_AreaDic.Remove(area.areaName + area.areaID);
            m_AreaDic.Add(new_name + area.areaID, area.areaID);
        }
        #endregion

        #region Event
        public void dispathEvent(int event_id, object data)
        {
            foreach (var area in m_AreaList)
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