using UnityEngine;

namespace tezcat
{
    public abstract class TezObjectMB
        : MonoBehaviour
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

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            m_Init = true;
        }

        protected virtual void OnEnable()
        {
            this.dirty = true;
        }

        protected virtual void OnDestroy()
        {

        }

        protected abstract void onRefresh();
    }
}