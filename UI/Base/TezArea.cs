using System.Collections.Generic;
using tezcat.Framework.DataBase;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// Are才是实际包含Widget的容器
    /// 用于功能区域的设计
    /// </summary>
    public abstract class TezArea
        : TezWidget
        , ITezFocusableWidget
        , ITezPrefab
    {
        [SerializeField]
        private int m_AreaID = -1;
        public int areaID
        {
            get { return m_AreaID; }
            set { m_AreaID = value; }
        }

        [SerializeField]
        private string m_AreaName = null;
        public string areaName
        {
            get
            {
                return m_AreaName;
            }
            set
            {
                this.window?.onAreaNameChanged(this, value);
                m_AreaName = value;
                this.name = m_AreaName + m_AreaID;
            }
        }

        public TezWindow window { get; set; } = null;
        protected TezWidgetEvent.Dispatcher m_EventDispatcher = new TezWidgetEvent.Dispatcher();
        List<TezArea> m_Children = new List<TezArea>();

        #region Widget
        protected override void preInit()
        {
            foreach (RectTransform item in this.transform)
            {
                var area = item.GetComponent<TezArea>();
                if (area)
                {
                    this.addChild(area);
                }
            }
        }

        protected override void initWidget()
        {
            Transform parent = this.transform;
            do
            {
                parent = parent.parent;
                this.window = parent.GetComponent<TezWindow>();
            } while (this.window == null);

            this.window.addArea(this);
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

        protected override void onRefresh(RefreshPhase phase)
        {

        }

        public override void reset()
        {
            foreach (var child in m_Children)
            {
                child?.reset();
            }
        }

        public override void clear()
        {
            foreach (var child in m_Children)
            {
                child?.close();
            }
            m_Children.Clear();
            m_Children = null;

            window.removeArea(this);
            window = null;
        }
        #endregion


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            window.setFocusWidget(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            window.setFocusWidget(null);
        }

        public void onEvent(int event_id, object data)
        {
            this.m_EventDispatcher.invoke(event_id, data);
        }

        private void growSpace(TezArea area)
        {
            while(m_Children.Count <= area.areaID)
            {
                m_Children.Add(null);
            }
        }

        public void addChild(TezArea area)
        {
            this.growSpace(area);
            m_Children[area.areaID] = area;
            area.window = window;
        }

        public void removeChild(TezArea area)
        {
            m_Children[area.areaID] = null;
        }
    }
}