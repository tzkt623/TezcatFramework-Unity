using tezcat.Extension;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public abstract class TezButton
        : TezUIWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        public event TezEventExtension.Action<TezButton> onInteract;
        public event TezEventExtension.Action<TezButton> onFocus;
        public event TezEventExtension.Action<TezButton> onUnFocus;
        public event TezEventExtension.Action<TezButton, PointerEventData> onClick;

        public abstract Graphic graphicController { get; }

        public override void clear()
        {
            onInteract = null;
            onFocus = null;
            onUnFocus = null;
            onClick = null;
        }

        protected override void onInteractable(bool value)
        {
            graphicController.raycastTarget = value;
            this.onInteract?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onFocus?.Invoke(this);
                this.onPointerEnter(eventData);
            }
        }
        protected abstract void onPointerEnter(PointerEventData eventData);


        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onUnFocus?.Invoke(this);
                this.onPointerExit(eventData);
            }
        }
        protected abstract void onPointerExit(PointerEventData eventData);


        public void OnPointerDown(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onPointerDown(eventData);
            }
        }
        protected abstract void onPointerDown(PointerEventData eventData);


        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onClick?.Invoke(this, eventData);
                this.onPointerUp(eventData);
            }
        }
        protected abstract void onPointerUp(PointerEventData eventData);

    }
}