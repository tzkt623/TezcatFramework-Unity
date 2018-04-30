using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public abstract class TezButton
        : TezWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        Graphic m_Graphic = null;

        protected override void Awake()
        {
            base.Awake();
            m_Graphic = this.gameObject.GetComponent<Graphic>();
        }

        protected override void onInteractable(bool value)
        {
            m_Graphic.raycastTarget = value;
        }

        protected override void clear()
        {
            m_Graphic = null;
        }

        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
    }
}