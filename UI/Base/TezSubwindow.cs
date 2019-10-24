using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// Subwindow用于同一个Window下多个功能区域的设计
    /// </summary>
    public abstract class TezSubwindow
        : TezWidget
        , ITezFocusableWidget
        , ITezPrefab
    {
//        [SerializeField]
        private int m_SubwindowID = -1;
        public int subwindowID
        {
            get { return m_SubwindowID; }
            set { m_SubwindowID = value; }
        }

        [SerializeField]
        private string m_SubwindowName = null;
        public string subwindowName
        {
            get
            {
                return m_SubwindowName;
            }
            set
            {
                this.window?.onSubwindowNameChanged(this, value);
                m_SubwindowName = value;
                this.name = m_SubwindowName + m_SubwindowID;
            }
        }

        public TezWindow window { get; set; } = null;
        protected TezWidgetEvent.Dispatcher m_EventDispatcher = new TezWidgetEvent.Dispatcher();
        List<TezSubwindow> m_Children = new List<TezSubwindow>();

        #region Widget
        protected override void preInit()
        {
            foreach (RectTransform item in this.transform)
            {
                var area = item.GetComponent<TezSubwindow>();
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

            this.window.addSubwindow(this);
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

        protected override void onRefresh(TezRefreshPhase phase)
        {

        }

        public override void reset()
        {
//             foreach (var child in m_Children)
//             {
//                 child?.reset();
//             }
        }

        protected override void onClose()
        {
            foreach (var child in m_Children)
            {
                child?.close();
            }
            m_Children.Clear();
            m_Children = null;

            window.removeSubwindow(this.subwindowID);
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

        private void growSpace(TezSubwindow area)
        {
            while(m_Children.Count <= area.subwindowID)
            {
                m_Children.Add(null);
            }
        }

        public void addChild(TezSubwindow area)
        {
            this.growSpace(area);
            m_Children[area.subwindowID] = area;
            area.window = window;
        }

        public void removeChild(TezSubwindow area)
        {
            m_Children[area.subwindowID] = null;
        }
    }
}