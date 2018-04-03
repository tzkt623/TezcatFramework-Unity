using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public abstract class TezButton
        : TezUIObjectMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerDownHandler
        , IPointerUpHandler
    {
        Graphic m_Graphic = null;

        public sealed override bool interactable
        {
            get { return base.interactable; }

            set
            {
                base.interactable = value;
                m_Graphic.raycastTarget = value;
                this.onInteractable(value);
            }
        }

        protected abstract void onInteractable(bool value);

        protected override void Awake()
        {
            base.Awake();
            m_Graphic = this.gameObject.GetComponent<Graphic>();
        }

        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
    }
}