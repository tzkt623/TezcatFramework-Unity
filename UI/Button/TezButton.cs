using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public interface ITezButtonListener
    {
        void onInteract(TezButton button, bool value);
        void onPointerEnter(TezButton button, PointerEventData eventData);
        void OnPointerExit(TezButton button, PointerEventData eventData);
        void OnPointerDown(TezButton button, PointerEventData eventData);
        void OnPointerUp(TezButton button, PointerEventData eventData);
    }

    public abstract class TezButton
        : TezUIWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        public event TezEventExtension.Action<TezButton, PointerEventData> onClick;
        public abstract Graphic graphicController { get; }
        public ITezButtonListener listener { get; set; } = null;

        class DefaultListener : ITezButtonListener
        {
            void ITezButtonListener.onInteract(TezButton button, bool value)
            {

            }

            void ITezButtonListener.OnPointerDown(TezButton button, PointerEventData eventData)
            {

            }

            void ITezButtonListener.onPointerEnter(TezButton button, PointerEventData eventData)
            {

            }

            void ITezButtonListener.OnPointerExit(TezButton button, PointerEventData eventData)
            {

            }

            void ITezButtonListener.OnPointerUp(TezButton button, PointerEventData eventData)
            {

            }
        }
        static DefaultListener m_DefaultListener = new DefaultListener();

        protected override void preInit()
        {
            base.preInit();
            if (listener == null)
            {
                listener = m_DefaultListener;
            }
        }

        protected override void onRefresh(TezRefreshPhase phase)
        {

        }

        public override void clear()
        {
            listener = null;
            onClick = null;
        }

        protected override void onInteractable(bool value)
        {
            graphicController.raycastTarget = value;
            listener.onInteract(this, value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.interactable)
            {
                listener.onPointerEnter(this, eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.interactable)
            {
                listener.OnPointerExit(this, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this.interactable)
            {
                listener.OnPointerDown(this, eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.interactable)
            {
                listener.OnPointerUp(this, eventData);
                this.onClick?.Invoke(this, eventData);
            }
        }
    }
}