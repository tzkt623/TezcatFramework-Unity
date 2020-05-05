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
        , ITezClickableWidget
    {
        public event TezEventExtension.Action<TezButton, PointerEventData> onClick;
        /// <summary>
        /// 控制按钮碰撞检测的对象
        /// </summary>
        public abstract Graphic graphicController { get; }

        protected override void onRefresh(TezRefreshPhase phase)
        {

        }

        protected override void onClose()
        {
            onClick = null;
        }

        protected override void onInteractable(bool value)
        {
            graphicController.raycastTarget = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onPointerEnter(this, eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onPointerExit(this, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onPointerDown(this, eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.interactable)
            {
                this.onPointerUp(this, eventData);
                this.onClick?.Invoke(this, eventData);
            }
        }

        protected abstract void onPointerEnter(TezButton button, PointerEventData eventData);
        protected abstract void onPointerExit(TezButton button, PointerEventData eventData);
        protected abstract void onPointerDown(TezButton button, PointerEventData eventData);
        protected abstract void onPointerUp(TezButton button, PointerEventData eventData);
    }
}