using System;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezSubwindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        public TezWindow window { get; set; } = null;
        public abstract string windowName { get; }
        public int windowID { get; set; } = -1;

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
        }

        public void close()
        {
            if(this.checkOnClose())
            {
                window.removeSubwindow(this);
                window = null;
                this.onClose();
                Destroy(this.gameObject);
            }
        }

        public virtual void onWindowEvent(TezWindow.Event evt)
        {

        }

        public virtual bool checkOnClose()
        {
            return true;
        }

        public virtual void onClose()
        {

        }

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

        public virtual void onDrop(PointerEventData eventData)
        {

        }
    }
}