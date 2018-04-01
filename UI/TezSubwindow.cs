using System;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezSubwindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        TezWindow m_Window;

        protected override void Start()
        {
            base.Start();
            m_Window = this.GetComponentInParent<TezWindow>();
            if (m_Window == null)
            {
                throw new ArgumentNullException("Window Not Found");
            }

            m_Window.addSubWindow(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            m_Window.onFocusSubwindow(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            m_Window.onFocusSubwindow(null);
        }

        public virtual void onPointerUp(PointerEventData eventData)
        {

        }

        public virtual void onPointerDown(PointerEventData eventData)
        {

        }

        public virtual void onDrop(PointerEventData eventData)
        {

        }

        public virtual void onWindowClose()
        {

        }
    }
}