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

        protected override void Awake()
        {
            base.Awake();
            this.eventSwitcher = new TezUIEvent.Switcher();
        }

        public sealed override void close()
        {
            base.close();
        }

        public virtual bool checkOnClose()
        {
            return true;
        }

        protected override void clear()
        {
            window.removeArea(this);
            window = null;
        }

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
    }
}