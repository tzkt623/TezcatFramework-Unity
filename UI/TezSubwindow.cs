using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezSubwindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        public TezWindow window { get; set; }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            window.onFocusSubwindow(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            window.onFocusSubwindow(null);
        }

        public virtual void onPointerUp(PointerEventData eventData)
        {

        }

        public virtual void onPointerDown(PointerEventData eventData)
        {

        }
    }
}