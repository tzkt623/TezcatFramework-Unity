using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezWindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerDownHandler
        , IPointerUpHandler
        , IDropHandler
    {
        TezWindowTitle m_Title = null;

        List<TezSubwindow> m_SubwindowList = new List<TezSubwindow>();
        TezSubwindow m_FocusSubwindow = null;

        public ITezPointer currentPointer
        {
            get; private set;
        }

        public bool isOpen
        {
            get { return this.gameObject.activeSelf; }
        }

        public void open()
        {
            if (onOpen())
            {
                this.gameObject.SetActive(true);
            }
        }

        protected virtual bool onOpen()
        {
            return true;
        }

        public void close(bool clear = false)
        {
            if (onClose())
            {
                if (clear)
                {
                    Destroy(this.gameObject);
                }
                this.gameObject.SetActive(false);
            }
        }

        protected virtual bool onClose()
        {
            foreach (var subwindow in m_SubwindowList)
            {
                subwindow.onWindowClose();
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

        public void onFocusSubwindow(TezSubwindow subwindow)
        {
            m_FocusSubwindow = subwindow;
        }

        public void addSubWindow(TezSubwindow subwindow)
        {
            m_SubwindowList.Add(subwindow);
        }

        protected override void onRefresh()
        {
            foreach (var sub in m_SubwindowList)
            {
                sub.dirty = true;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            m_FocusSubwindow?.onPointerDown(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            m_FocusSubwindow?.onPointerUp(eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            m_FocusSubwindow?.onDrop(eventData);
        }
    }
}