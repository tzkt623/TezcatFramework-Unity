using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public enum TezWidgetLife
    {
        Normal,
        TypeOnly
    }

    public abstract class TezBaseWidget
        : UIBehaviour
        , ITezWidget
    {
        public RectTransform rectTransform => (RectTransform)this.transform;
        public TezWidgetLife life { get; set; } = TezWidgetLife.Normal;

        bool m_Interactable = true;

        /// <summary>
        /// 是否允许交互
        /// </summary>
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

        protected sealed override void Awake()
        {
            base.Awake();
            this.preInit();
        }

        protected override void Start()
        {
            base.Start();
            this.initWidget();
        }

        /// <summary>
        /// 在Widget初始化之前调用
        /// </summary>
        protected virtual void preInit() { }

        /// <summary>
        /// 在这里初始化你的Widget
        /// </summary>
        protected virtual void initWidget() { }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void close(bool self_close = true)
        {
            switch (life)
            {
                case TezWidgetLife.TypeOnly:
                    TezService.get<TezcatFramework>().removeTypeOnlyWidget(this);
                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }

        /// <summary>
        /// 是否可以交互
        /// </summary>
        protected virtual void onInteractable(bool value) { }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void reset() { }

        public void open()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Widget组件基类
    /// </summary>
    public abstract class TezUIWidget
        : TezBaseWidget
        , ITezRefreshHandler
    {
        bool m_Inited = false;
        bool m_Closed = false;

        TezRefreshPhase m_RefreshPhase = TezRefreshPhase.Ready;
        public TezRefreshPhase refreshPhase
        {
            set
            {
                if (this.gameObject.activeInHierarchy && m_Inited)
                {
                    switch (m_RefreshPhase)
                    {
                        case TezRefreshPhase.Ready:
                            TezService.get<TezcatFramework>().pushRefreshHandler(this);
                            m_RefreshPhase = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 在这里清理所有的托管资源
        /// </summary>
        protected virtual void onClose(bool self_close) { }

        /// <summary>
        /// 关闭并销毁控件
        /// </summary>
        public sealed override void close(bool self_close = true)
        {
            switch (life)
            {
                case TezWidgetLife.TypeOnly:
                    TezService.get<TezcatFramework>().removeTypeOnlyWidget(this);
                    break;
                default:
                    break;
            }

            ///设置关闭位
            ///这样是为了让控件可以立即释放资源
            ///而不是等到Destroy执行导致时间不确定
            m_Closed = true;
            this.onClose(self_close);
            Destroy(this.gameObject);
        }

        #region 规范流程
        /*
         * Awake(仅执行一次) -> OnEnable -> Start(仅执行一次)
         * 
         * 所以当脚本第一次执行时 没有设置Init位 只会执行Start刷新 所以OnEnable并不刷新
         * Enable刷新必须在Disable之后才会执行 并且不再会执行Start刷新
         * 
         */

        protected sealed override void OnEnable()
        {
            base.OnEnable();
            if (m_Inited)
            {
                this.onShow();
                this.refreshPhase = TezRefreshPhase.Refresh;
            }
        }

        protected sealed override void Start()
        {
            this.initWidget();
            m_Inited = true;
            this.refreshPhase = TezRefreshPhase.Refresh;
        }

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            if (m_Inited)
            {
                this.onHide();
            }
        }

        /// <summary>
        /// 控件自行销毁时
        /// </summary>
        protected sealed override void OnDestroy()
        {
            base.OnDestroy();
            ///如果此控件没有设置关闭位
            ///说明没有手动销毁他
            ///他一定是被父级带动销毁的
            if (!m_Closed)
            {
                this.onClose(false);
            }
        }

        #endregion

        #region 刷新流程
        /// <summary>
        /// 刷新
        /// </summary>
        public void refresh()
        {
            //             for (byte i = 0; i < m_DirtyCount; i++)
            //             {
            //                 this.onRefresh(m_RefreshPhaseArray[i]);
            //             }
            // 
            //             m_DirtyCount = 0;
            //             m_DirtyMask = 0;

            this.onRefresh();
            m_RefreshPhase = TezRefreshPhase.Ready;
        }

        /// <summary>
        /// 立即刷新阶段
        /// </summary>
        protected virtual void onRefresh() { }


        /// <summary>
        /// 显示控件时调用
        /// 立即调用
        /// 未初始化完成不会调用
        /// </summary>
        protected virtual void onShow() { }

        /// <summary>
        /// 隐藏控件时调用
        /// 立即调用
        /// 未初始化完成不会调用
        /// </summary>
        protected virtual void onHide() { }

        #endregion
        #region 重载操作
        public static bool operator true(TezUIWidget obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezUIWidget obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezUIWidget obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }

    /// <summary>
    /// 工具性组件基类
    /// </summary>
    public abstract class TezToolWidget : TezUIWidget
    {

    }
}