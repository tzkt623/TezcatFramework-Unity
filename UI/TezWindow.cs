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
            if (this.onOpen())
            {
                this.gameObject.SetActive(true);
            }
        }

        protected virtual bool onOpen()
        {
            return true;
        }

        public void hide()
        {
            if(this.onHide())
            {
                this.gameObject.SetActive(false);
            }
        }

        protected virtual bool onHide()
        {
            return true;
        }

        public void close()
        {
            if (this.checkOnClose())
            {
                this.onClose();
                Destroy(this.gameObject);
            }
        }

        protected virtual bool checkOnClose()
        {
            List<TezEventBus.Action> close_function_list = new List<TezEventBus.Action>(m_SubwindowList.Count);

            bool result = true;
            foreach (var subwindow in m_SubwindowList)
            {
                result &= subwindow.checkOnWindowClose();
                if (result)
                {
                    close_function_list.Add(subwindow.onWindowClose);
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

        protected virtual void onClose()
        {

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