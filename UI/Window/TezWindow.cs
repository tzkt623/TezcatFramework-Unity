﻿using System.Collections.Generic;
using tezcat.Utility;
using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    /// <summary>
    /// Window本身只包含Area
    /// 用于划分其中的显示区域
    /// </summary>
    public abstract class TezWindow
        : TezWidget
        , ITezFocusableWidget
        , ITezDropableWidget
        , ITezEventHandler
        , ITezEventDispather
        , IDropHandler
        , IPointerDownHandler
        , IPointerUpHandler
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
                return m_WindowName;
            }
            set
            {
                this.layer?.onWindowNameChanged(m_WindowName, value);
                m_WindowName = value;
                this.name = m_WindowName;
            }
        }


        TezWindowTitle m_Title = null;

        #region Area
        List<TezArea> m_AreaList = new List<TezArea>();
        Dictionary<string, int> m_AreaDic = new Dictionary<string, int>();
        #endregion

        ITezFocusableWidget m_FocusWidget = null;
        public TezUIEvent.Switcher eventSwitcher { get; private set; } = null;
        public List<ITezEventHandler> handlers { get; private set; } = new List<ITezEventHandler>();
        List<TezPopup> m_PopupList = new List<TezPopup>();

        protected override void Awake()
        {
            base.Awake();

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

        protected override void Start()
        {
            base.Start();
        }

        protected override void onRefresh()
        {
            foreach (var sub in m_AreaList)
            {
                sub.dirty = true;
            }
        }

        public override void close()
        {
            if(this.checkForClose())
            {
                base.close();
            }
        }

        protected virtual bool checkForClose()
        {
            List<TezEventBus.Action> close_function_list = new List<TezEventBus.Action>(m_AreaList.Count);

            bool result = true;
            foreach (var area in m_AreaList)
            {
                result &= area.checkOnClose();
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

        public void setTitle(string title)
        {
            if (m_Title)
            {
                m_Title.setName(title);
            }
        }

        public void setTile(TezWindowTitle title)
        {
            m_Title = title;
        }

        public void onFocusWidget(ITezFocusableWidget widget)
        {
            m_FocusWidget = widget;
        }

        public T createPopup<T>(T prefab) where T : TezWidget, ITezPopup
        {
            var widget = Instantiate(prefab, this.transform);
            widget.window = this;
            m_PopupList.Add(widget);
            return widget;
        }



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

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            ((ITezDropableWidget)this).onDrop(eventData);
        }

        void ITezDropableWidget.onDrop(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    var widget = m_FocusWidget as ITezDropableWidget;
                    if (widget != null)
                    {
                        widget.onDrop(eventData);
                    }
                    else
                    {
                        TezDragDropManager.dropItem(this, eventData);
                    }
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
            }
        }

        public virtual TezEventBus.Action<ITezStorageItemWrapper> checkItemToDrop(ITezStorageItemWrapper wrapper, PointerEventData event_data)
        {
            return null;
        }
    }
}