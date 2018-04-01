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
                m_Dirty = value;
                if (m_Init && this.gameObject.activeSelf && m_Dirty)
                {
                    this.onRefresh();
                    m_Dirty = false;
                }
            }
        }

        protected override void Start()
        {
            m_Init = true;
        }

        protected override void OnEnable()
        {
            this.dirty = true;
        }

        protected abstract void onRefresh();
    }
}