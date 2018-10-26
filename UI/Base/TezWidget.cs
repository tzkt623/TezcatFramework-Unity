using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// Widget组件基类
    /// </summary>
    public abstract class TezWidget
        : UIBehaviour
        , ITezWidget
    {
        bool m_Interactable = true;
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                if (m_Interactable == value)
                {
                    return;
                }

                this.onInteractable(value);
                m_Interactable = value;
            }
        }

        public enum RefreshPhase : byte
        {
            System1,
            System2,
            Custom1,
            Custom2,
            Custom3,
            Custom4,
            Custom5
        }

        bool m_Init = false;
        bool m_Clear = false;

        public RefreshPhase refresh
        {
            set
            {
                if(m_Init && this.gameObject.activeSelf)
                {
                    this.onRefresh(value);
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
            if (!m_Init)
            {
                m_Init = true;
                base.Start();
                this.linkEvent();
                this.initWidget();
                this.refreshAfterInit();
            }
        }

        protected sealed override void OnEnable()
        {
            if (m_Init)
            {
                base.OnEnable();
                this.linkEvent();
                this.onOpenAndRefresh();
            }
        }

        protected sealed override void OnDisable()
        {
            if (m_Init)
            {
                base.OnDisable();
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
        /// 初始化完成后刷新数据
        /// </summary>
        protected abstract void refreshAfterInit();

        /// <summary>
        /// 自定义刷新数据
        /// </summary>
        protected abstract void onRefresh(RefreshPhase phase);

        /// <summary>
        /// 
        /// </summary>
        protected abstract void onOpenAndRefresh();

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

    /// <summary>
    /// 游戏UI类组件基类
    /// </summary>
    public abstract class TezUIWidget : TezWidget
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

        protected override void refreshAfterInit()
        {

        }

        protected override void onOpenAndRefresh()
        {

        }

        protected override void onRefresh(RefreshPhase phase)
        {

        }

        protected override void preInit()
        {

        }

        protected override void unLinkEvent()
        {

        }
    }

    /// <summary>
    /// 功能性组件基类
    /// </summary>
    public abstract class TezFunctionWidget : TezWidget
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

        protected override void refreshAfterInit()
        {

        }

        protected override void onOpenAndRefresh()
        {

        }

        protected override void onRefresh(RefreshPhase phase)
        {

        }

        protected override void preInit()
        {

        }

        protected override void unLinkEvent()
        {

        }
    }

    /// <summary>
    /// 工具性组件基类
    /// </summary>
    public abstract class TezToolWidget : TezWidget
    {
        protected override void refreshAfterInit()
        {

        }

        protected override void onOpenAndRefresh()
        {

        }

        protected override void onRefresh(RefreshPhase phase)
        {

        }
    }
}