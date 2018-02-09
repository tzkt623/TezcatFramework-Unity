using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezButton
        : TezUIObjectMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerDownHandler
        , IPointerUpHandler
    {
        public event TezEventBus.Action onClick;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            this.onEnter(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            this.onExit(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            this.onDown(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.onUp(eventData);

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    onClick.launch();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        protected abstract void onEnter(PointerEventData eventData);
        protected abstract void onExit(PointerEventData eventData);
        protected abstract void onDown(PointerEventData eventData);
        protected abstract void onUp(PointerEventData eventData);
    }
}