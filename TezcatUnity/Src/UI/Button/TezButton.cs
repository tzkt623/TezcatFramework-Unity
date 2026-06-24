using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Unity.UI
{
    public enum TezButtonState
    {
        Enter,
        Exit,
        Down,
        Up,
    }

    public abstract class TezButton
        : TezUIWidget
        , ITezFocusableWidget
        , ITezClickableWidget
    {
        public event Action<TezButton, PointerEventData> onClick;



        /// <summary>
        /// 控制按钮碰撞检测的对象
        /// </summary>
        public abstract Graphic graphicController { get; }

        protected override void onClose(bool self_close)
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
                //this.onClick?.Invoke(this, eventData);
            }
        }

        protected abstract void onPointerEnter(TezButton button, PointerEventData eventData);
        protected abstract void onPointerExit(TezButton button, PointerEventData eventData);
        protected abstract void onPointerDown(TezButton button, PointerEventData eventData);
        protected abstract void onPointerUp(TezButton button, PointerEventData eventData);
    }
}