using System.Collections.Generic;
using tezcat.Utility;
using UnityEngine;

namespace tezcat.UI
{
    /// <summary>
    /// Window本身只包含Area
    /// 用于划分其中的显示区域
    /// </summary>
    public class TezWindow
        : TezWidget
        , ITezEventHandler
        , ITezEventDispather
    {
        public TezLayer layer { get; set; } = null;

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
                if(string.IsNullOrEmpty(m_WindowName))
                {
                    m_WindowName = this.name;
                }

                return m_WindowName;
            }
            set
            {
                this.layer?.onWindowNameChanged(m_WindowName, value);
                m_WindowName = value;
                this.name = m_WindowName;
            }
        }

        /// <summary>
        /// Area
        /// </summary>
        List<TezArea> m_AreaList = new List<TezArea>();
        Dictionary<string, int> m_AreaDic = new Dictionary<string, int>();

        protected ITezFocusableWidget m_FocusWidget = null;
        public TezUIEvent.Switcher eventSwitcher { get; private set; } = null;
        public List<ITezEventHandler> handlers { get; private set; } = new List<ITezEventHandler>();

        TezPopupContent m_PopupContent = null;
        List<TezPopup> m_PopupList = new List<TezPopup>();

        #region Core
        protected override void preInit()
        {
            List<TezArea> list = new List<TezArea>();
            this.GetComponentsInChildren<TezArea>(true, list);
            foreach (var sub in list)
            {
                this.registerArea(sub);
            }

            this.eventSwitcher = new TezUIEvent.Switcher();
            this.eventSwitcher.register(TezUIEvent.ShowArea, (object data) =>
            {
                int id = -1;
                if (m_AreaDic.TryGetValue((string)data, out id))
                {
                    m_AreaList[id].open();
                }
            });
        }

        protected override void initWidget()
        {
            m_PopupContent = this.GetComponentInChildren<TezPopupContent>();
            if (m_PopupContent == null)
            {
                GameObject go = new GameObject();
                var rect = go.AddComponent<RectTransform>();
                go.transform.SetParent(this.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.SetAsLastSibling();
                m_PopupContent = go.AddComponent<TezPopupContent>();
                TezUILayout.setLayout(rect, 0, 0, 0, 0);
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
            List<TezEventBus.Action> close_function_list = new List<TezEventBus.Action>(m_AreaList.Count);

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
            foreach (var popup in m_PopupList)
            {
                popup.reset();
            }

            foreach (var area in m_AreaList)
            {
                area.reset();
            }
        }

        public override void clear()
        {
            foreach (var popup in m_PopupList)
            {
                popup.close();
            }
            m_PopupList.Clear();
            m_PopupList = null;

            foreach (var area in m_AreaList)
            {
                area.close();
            }
            m_AreaList.Clear();
            m_AreaList = null;

            m_AreaDic.Clear();
            m_AreaDic = null;
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


        #region Popup
        public T createPopup<T>(T prefab) where T : TezPopup
        {
            var widget = Instantiate(prefab, m_PopupContent.transform, false);
            widget.window = this;
            widget.transform.localPosition = Vector3.zero;
            widget.popupID = m_PopupList.Count;
            m_PopupList.Add(widget);
            return widget;
        }

        public void closePopup(TezPopup popup)
        {
            m_PopupList.Remove(
                popup.popupID,

                (TezPopup remove, TezPopup last) =>
                {
                    last.popupID = remove.popupID;
                },

                (TezPopup remove) =>
                {

                });
        }
        #endregion

        #region Area
        private void registerArea(TezArea area)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(area.areaID >= 0, "UIWindow (" + m_WindowName + ")", "Window (" + area.areaName + ") ID Must EqualGreater Than 0");
#endif
            while (m_AreaList.Count <= area.areaID)
            {
                m_AreaList.Add(null);
            }

            if (area.areaName == null)
            {
                area.areaName = m_WindowName + "_Area_" + m_AreaList.Count;
            }

            area.window = this;
            m_AreaList[area.areaID] = area;
            m_AreaDic.Add(area.areaName, area.areaID);
            this.handlers.Add(area);

#if UNITY_EDITOR
            TezDebug.info("UIWindow (" + m_WindowName + ")", "Register Area: " + area.areaName + " ID:" + area.areaID);
#endif
        }


        public void addArea(TezArea area)
        {
            if (area.areaName == null)
            {
                area.areaName = m_WindowName + "_Area_" + m_AreaList.Count;
            }

            if (!m_AreaDic.ContainsKey(area.areaName))
            {
                area.window = this;
                area.areaID = m_AreaList.Count;
                m_AreaList.Add(area);
                m_AreaDic.Add(area.areaName, area.areaID);
#if UNITY_EDITOR
                TezDebug.info("UIWindow (" + m_WindowName + ")", "Add Area: " + area.areaName + " ID:" + area.areaID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIWindow(" + m_WindowName + ")", "Area Didn't Exist : " + area.areaName + " ID: " + area.areaID);
            }
#endif
        }

        public void removeArea(int id)
        {
            m_AreaList.Remove(
                id,

                (TezArea remove, TezArea last) =>
                {
                    last.areaID = remove.areaID;
                    m_AreaDic.Remove(remove.areaName);
                    m_AreaDic[last.areaName] = last.areaID;
                },

                (TezArea remove) =>
                {
                    m_AreaDic.Remove(remove.areaName);
                });
        }

        public void removeArea(TezArea subwindow)
        {
            this.removeArea(subwindow.areaID);
        }

        public T getArea<T>() where T : TezArea
        {
            foreach (var area in m_AreaList)
            {
                if(area is T)
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

        public void onAreaNameChanged(string old_name, string new_name)
        {
#if UNITY_EDITOR
            TezDebug.info("UIWindow (" + m_WindowName + ")", "Area Name: " + old_name + " Change To: " + new_name);
#endif

            int id = m_AreaDic[old_name];
            m_AreaDic.Remove(old_name);
            m_AreaDic.Add(new_name, id);
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
            this.eventSwitcher.invoke(event_id, data);
        }
        #endregion
    }
}