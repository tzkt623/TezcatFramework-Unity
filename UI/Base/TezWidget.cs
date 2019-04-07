using tezcat.Framework.Core;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public enum TezWidgetLifeForm
    {
        Normal,
        TypeOnly
    }

    /// <summary>
    /// Widget组件基类
    /// </summary>
    public abstract class TezWidget
        : UIBehaviour
        , ITezRefresher
        , ITezWidget
    {
        public TezWidgetLifeForm lifeForm { get; set; } = TezWidgetLifeForm.Normal;

        bool m_Init = false;
        bool m_Clear = false;

        bool m_Interactable = true;

        TezRefreshPhase m_DirtyMask = 0;
        byte m_DirtyCount = 0;
        TezRefreshPhase[] m_RefreshPhaseArray = new TezRefreshPhase[8];
        ITezRefresher m_NextRefresher = null;
        ITezRefresher ITezRefresher.next
        {
            get
            {
                ///清空当前刷新链表里的下一个缓存
                var temp = m_NextRefresher;
                m_NextRefresher = null;
                return temp;
            }
            set
            {
                m_NextRefresher = value;
            }
        }

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

        public TezRefreshPhase refreshPhase
        {
            set
            {
                if (m_Init && this.gameObject.activeSelf)
                {
                    if ((m_DirtyMask & value) == 0)
                    {
                        if (m_DirtyCount == 0)
                        {
                            TezService.get<TezcatFramework>().pushRefresher(this);
                        }

                        m_DirtyMask |= value;
                        m_RefreshPhaseArray[m_DirtyCount++] = value;
                    }
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
                this.refreshPhase = TezRefreshPhase.P_OnInit;
            }
        }

        protected sealed override void OnEnable()
        {
            if (m_Init)
            {
                base.OnEnable();
                this.linkEvent();
                this.refreshPhase = TezRefreshPhase.P_OnEnable;
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

//             if (m_Clear)
//             {
//                 this.clear();
//             }
        }

        protected sealed override void OnDestroy()
        {
            base.OnDestroy();
            this.clear();
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
        /// 刷新
        /// </summary>
        public void refresh()
        {
            for (byte i = 0; i < m_DirtyCount; i++)
            {
                this.onRefresh(m_RefreshPhaseArray[i]);
            }

            m_DirtyCount = 0;
            m_DirtyMask = 0;
        }

        /// <summary>
        /// 自定义刷新数据
        /// </summary>
        protected abstract void onRefresh(TezRefreshPhase phase);

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
            TezService.get<TezcatFramework>().removeWidget(this);
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

    }
}