using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezWindow : TezUINodeMB
    {
        TezWindowTitle m_Title = null;
        List<TezUINodeMB> m_ChildrenList = new List<TezUINodeMB>();

        TezUINodeMB m_ActivedSubwindow = null;

        public ITezPointer currentPointer
        {
            get; private set;
        }

        public bool isOpen
        {
            get { return this.gameObject.activeSelf; }
        }

        protected override void onStart()
        {
            base.onStart();
            if (m_Title == null)
            {
                m_Title = this.GetComponentInChildren<TezWindowTitle>();
            }
        }

        public void open()
        {
            if (onOpen())
            {
                this.gameObject.SetActive(true);
            }
        }

        protected virtual bool onOpen()
        {
            return true;
        }

        public void close(bool clear = false)
        {
            if (onClose())
            {
                if (clear)
                {
                    Destroy(this.gameObject);
                }
                this.gameObject.SetActive(false);
            }
        }

        protected virtual bool onClose()
        {
            return true;
        }

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void setTitle(string title)
        {
            if (m_Title)
            {
                m_Title.setName(title);
            }
        }

        public void activeSubwindow(TezUINodeMB subwindow)
        {
            if (m_ActivedSubwindow != subwindow)
            {
                if(m_ActivedSubwindow != null)
                {
                    m_ActivedSubwindow.gameObject.SetActive(false);
                }

                m_ActivedSubwindow = subwindow;
                m_ActivedSubwindow.gameObject.SetActive(true);

                currentPointer = m_ActivedSubwindow as ITezPointer;
            }
        }
    }
}