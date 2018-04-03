using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezSubwindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        public TezWindow window { get; set; } = null;

        [SerializeField]
        private int m_SubwindowID = -1;
        public int subwindowID
        {
            get { return m_SubwindowID; }
            set { m_SubwindowID = value; }
        }

        [SerializeField]
        private string m_SubwindowName = null;
        public string windowName
        {
            get
            {
                return m_SubwindowName;
            }
            set
            {
                this.window?.onSubwindowNameChanged(m_SubwindowName, value);
                m_SubwindowName = value;
                this.name = m_SubwindowName;
            }
        }


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
            if (this.checkOnClose())
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