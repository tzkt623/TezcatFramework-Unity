using tezcat.Framework.Core;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public enum TezWidgetLifeState
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
        public TezWidgetLifeState lifeState { get; set; } = TezWidgetLifeState.Normal;

        bool m_Interactable = true;

        class ControlMask
        {
            public const byte Inited = 1;
            public const byte Closed = 1 << 1;
            public const byte Disabled = 1 << 2;
        }
        TezBitMask_byte m_Mask = new TezBitMask_byte();

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
                if (this.gameObject.activeSelf && m_Mask.test(ControlMask.Inited))
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

        #region 启动流程
        /// <summary>
        /// 第一步 Awake
        /// </summary>
        protected sealed override void Awake()
        {
            base.Awake();
            this.preInit();
        }

        /// <summary>
        /// 第二步 OnEnable
        /// 由于init参数的限制
        /// 并不会在此步执行事件连接函数和触发刷新动作
        /// 所以在类disable之后再enable时
        /// 会触发OnEnable状态下的刷新动作
        /// 使类的数据自动被刷新
        /// </summary>
        protected sealed override void OnEnable()
        {
            base.OnEnable();
            if (m_Mask.test(ControlMask.Inited))
            {
                this.linkEvent();
                this.refreshPhase = TezRefreshPhase.P_OnEnable;
            }
        }

        /// <summary>
        /// 第三步 执行Start
        /// 执行事件连接函数
        /// 完成整个初始化步奏
        /// 并且在OnInit状态下刷新数据
        /// </summary>
        protected sealed override void Start()
        {
            base.Start();
            this.linkEvent();
            this.initWidget();
            m_Mask.set(ControlMask.Inited);
            this.refreshPhase = TezRefreshPhase.P_OnInit;
        }
        #endregion

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            if (m_Mask.test(ControlMask.Inited))
            {
                this.onHide();
                this.unLinkEvent();
            }
        }

        protected sealed override void OnDestroy()
        {
            base.OnDestroy();
            if (!m_Mask.test(ControlMask.Closed))
            {
                this.onClose();
            }
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
        protected abstract void onClose();

        /// <summary>
        /// 关闭并销毁控件
        /// </summary>
        public void close()
        {
            switch (lifeState)
            {
                case TezWidgetLifeState.TypeOnly:
                    TezService.get<TezcatFramework>().removeTypeOnlyWidget(this);
                    break;
                default:
                    break;
            }


            m_Mask.set(ControlMask.Closed);
            this.onClose();
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
        public RectTransform rectTransform => (RectTransform)this.transform;

        protected override void onClose()
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
        protected override void onClose()
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