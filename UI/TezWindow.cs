using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        TezSubwindow focusSubwindow { get; set; }

        public ITezPointer currentPointer
        {
            get; private set;
        }

        public bool isOpen
        {
            get { return this.gameObject.activeSelf; }
        }

        protected override void Start()
        {
            base.Start();
            if (m_Title == null)
            {
                m_Title = this.GetComponentInChildren<TezWindowTitle>();
            }
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
            return true;
        }

        public void setTitle(string title)
        {
            if (m_Title)
            {
                m_Title.setName(title);
            }
        }

        public void onFocusSubwindow(TezSubwindow subwindow)
        {
            focusSubwindow = subwindow;
        }

        public void addSubWindow(TezSubwindow subwindow)
        {
            subwindow.window = this;
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
            this.focusSubwindow?.onPointerDown(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            this.focusSubwindow?.onPointerUp(eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            this.focusSubwindow?.onDrop(eventData);
        }
    }
}