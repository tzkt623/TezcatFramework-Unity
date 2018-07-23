using tezcat.Core;
using tezcat.DataBase;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezWidget
        : UIBehaviour
        , ITezWidget
        , ITezPrefab
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

        protected static TezcatFramework framework
        {
            get { return TezcatFramework.instance; }
        }

        bool m_Init = false;
        bool m_Clear = false;

        public bool dirty
        {
            set
            {
                if (m_Init && this.gameObject.activeSelf)
                {
                    this.onRefresh();
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

            if (m_Clear)
            {
                this.clear();
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
            m_Clear = true;
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

        #region 重载操作
        public static bool operator true(TezWidget obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezWidget obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezWidget obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }

    public abstract class TezGameWidget : TezWidget
    {
        public override void clear()
        {

        }

        public override void reset()
        {

        }

        protected override void initWidget()
        {

        }

        protected override void linkEvent()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onRefresh()
        {

        }

        protected override void onShow()
        {

        }

        protected override void preInit()
        {

        }

        protected override void unLinkEvent()
        {

        }
    }

    public abstract class TezToolWidget : TezWidget
    {

    }
}