using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    /// <summary>
    /// Are才是实际包含Widget的容器
    /// 用于功能区域的设计
    /// </summary>
    public abstract class TezArea
        : TezWidget
        , ITezFocusableWidget
        , ITezEventHandler
    {
        public TezWindow window { get; set; } = null;
        public TezUIEvent.Switcher eventSwitcher { get; private set; }

        List<TezArea> m_Children = new List<TezArea>();

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
                this.window?.onAreaNameChanged(m_AreaName, value);
                m_AreaName = value;
                this.name = m_AreaName;
            }
        }

        #region Widget
        protected override void preInit()
        {
            this.eventSwitcher = new TezUIEvent.Switcher();

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

        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public override void reset()
        {
            foreach (var child in m_Children)
            {
                child?.reset();
            }
        }

        protected override void clear()
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
            this.eventSwitcher.invoke(event_id, data);
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