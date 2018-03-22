using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public abstract class TezUINodeMB : TezUIObjectMB
    {
        bool m_Init = false;

        bool m_Dirty = false;
        public bool dirty
        {
            get { return m_Dirty; }
            set
            {
                m_Dirty = (value && this.gameObject.activeSelf);
                if (m_Dirty && m_Init)
                {
                    m_Dirty = false;
                    this.onRefresh();
                }
            }
        }

        protected override void Start()
        {
            m_Init = true;
        }

        protected abstract void onRefresh();

        protected override void OnEnable()
        {
            if(m_Init)
            {
                this.dirty = true;
            }
        }
    }
}