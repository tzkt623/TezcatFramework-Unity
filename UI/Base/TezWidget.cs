using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezWidget
        : UIBehaviour
        , ITezWidget
    {
        bool m_Interactable;
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                this.onInteractable(m_Interactable);
            }
        }

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

        protected sealed override void Awake()
        {
            base.Awake();
            this.preInit();
        }

        protected sealed override void Start()
        {
            base.Start();
            this.linkEvent();
            this.initWidget();
            m_Init = true;
            this.dirty = true;
        }

        protected sealed override void OnEnable()
        {
            base.OnEnable();
            if (m_Init)
            {
                this.linkEvent();
                this.onShow();
                this.dirty = true;
            }
        }

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            if (m_Init)
            {
                this.onHide();
                this.unLinkEvent();
            }
        }

        protected sealed override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected virtual void onInteractable(bool value)
        {

        }

        /// <summary>
        /// 在Widget初始化之前调用
        /// </summary>
        protected abstract void preInit();

        /// <summary>
        /// 在这里初始化你的Widget
        /// </summary>
        protected abstract void initWidget();

        /// <summary>
        /// 在这里连接你的所有事件通知
        /// </summary>
        protected abstract void linkEvent();

        /// <summary>
        /// 在这里断开你的所有事件通知
        /// </summary>
        protected abstract void unLinkEvent();

        /// <summary>
        /// 在这里刷新你的Widget数据
        /// </summary>
        protected abstract void onRefresh();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void onShow();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void onHide();


        /// <summary>
        /// 重置你的Widget
        /// </summary>
        public abstract void reset();

        /// <summary>
        /// 在这里清理所有的托管资源
        /// </summary>
        public abstract void clear();

        /// <summary>
        /// 关闭并销毁控件
        /// </summary>
        public void close()
        {
            this.unLinkEvent();
            this.clear();
            Destroy(this.gameObject);
        }

        public virtual bool checkForClose()
        {
            return true;
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