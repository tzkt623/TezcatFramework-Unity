using tezcat.Unity.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Unity.UI
{
    public enum TezWidgetLife
    {
        Normal,
        TypeOnly
    }

    public abstract class TezBaseWidget
        : UIBehaviour
        , ITezBaseWidget
    {
        public RectTransform rectTransform => (RectTransform)this.transform;
        public TezWidgetLife life { get; set; } = TezWidgetLife.Normal;

        bool mInteractable = true;

        /// <summary>
        /// 是否允许交互
        /// </summary>
        public bool interactable
        {
            get { return mInteractable; }
            set
            {
                if (mInteractable == value)
                {
                    return;
                }

                this.onInteractable(value);
                mInteractable = value;
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
        public virtual void close()
        {
            switch (life)
            {
                case TezWidgetLife.TypeOnly:
                    TezcatUnity.removeTypeOnlyWidget(this);
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
        , ITezUIWidget
    {
        bool mInited = false;
        bool mClosed = false;
        bool mRefreshMask = false;


        /// <summary>
        /// 在这里清理所有资源
        /// </summary>
        /// <param name="self_close">
        /// true:自己主动关闭自己,资源由close函数释放
        /// false:由其他对象带动进行的销毁,资源由Unity自身函数OnDestroy释放
        /// </param>
        protected virtual void onClose(bool self_close) { }

        /// <summary>
        /// 关闭并销毁控件
        /// </summary>
        public sealed override void close()
        {
            switch (life)
            {
                case TezWidgetLife.TypeOnly:
                    TezcatUnity.removeTypeOnlyWidget(this);
                    break;
                default:
                    break;
            }

            ///设置关闭位
            ///这样是为了让控件可以立即释放资源
            ///而不是等到Destroy执行导致时间不确定
            mClosed = true;
            this.onClose(mClosed);
            Destroy(this.gameObject);
        }

        #region 规范流程
        /*
         * Awake(仅执行一次) -> OnEnable -> Start(仅执行一次)
         * 
         * 当脚本第一次执行时 由于没有设置mInited 只会执行Start刷新 OnEnable中并不刷新
         * Enable刷新必须在mInited之后才会执行 并且不再会执行Start刷新
         * 
         */
        protected sealed override void OnEnable()
        {
            base.OnEnable();
            if (mInited)
            {
                this.onShow();
                this.needRefresh();
            }
        }

        protected sealed override void Start()
        {
            this.initWidget();
            TezcatUnity.pushDelayInitHandler(this);
        }

        /// <summary>
        /// 延迟初始化
        /// 
        /// 处于initWidget之后
        /// 会在队列中集中处理这个过程
        /// 避免了unity的随机性
        /// </summary>
        void ITezDelayInitHandler.delayInit()
        {
            this.onDelayInit();
            mInited = true;
        }

        protected virtual void onDelayInit() { }

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            if (mInited)
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
            if (!mClosed)
            {
                this.onClose(mClosed);
            }
        }

        #endregion

        #region 刷新流程
        /// <summary>
        /// 通知UI刷新自己
        /// </summary>
        public void needRefresh()
        {
            if (this.gameObject.activeInHierarchy
//                && mInited
                && !mRefreshMask)
            {
                mRefreshMask = true;
                TezcatUnity.pushRefreshHandler(this);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        void ITezRefreshHandler.refresh()
        {
            this.onRefresh();
            mRefreshMask = false;
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
        #endregion
    }

    /// <summary>
    /// 工具性组件基类
    /// </summary>
    public abstract class TezToolWidget : TezUIWidget
    {

    }
}