using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezWidget
        : UIBehaviour
        , IWidget
    {
        public virtual bool interactable { get; set; } = true;

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

        public abstract void clear();

        /// <summary>
        /// 关闭并销毁控件
        /// </summary>
        public virtual void close()
        {
            this.clear();
            Destroy(this.gameObject);
        }

        public void open()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}