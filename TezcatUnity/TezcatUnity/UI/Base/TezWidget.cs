﻿using tezcat.Framework.Core;
using tezcat.Unity.Database;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Unity.UI
{
    public abstract class TezBaseWidget
        : UIBehaviour
        , ITezBaseWidget
    {
        public RectTransform rectTransform => (RectTransform)this.transform;

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

        public virtual TezPrefabCount prefabCount => TezPrefabCount.Invaild;

        protected override void Awake()
        {
            base.Awake();
            this.preInit();
        }

        protected override void Start()
        {
            base.Start();
            this.initWidget();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onCloseThis();
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
        protected virtual void onCloseThis() { }

        /// <summary>
        /// 是否可以交互
        /// </summary>
        protected virtual void onInteractable(bool value) { }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void reset() { }

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
        }

        void ITezCloseable.closeThis()
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Widget组件基类
    /// </summary>
    public abstract class TezUIWidget
        : TezBaseWidget
        , ITezUIWidget
    {
        bool mInitComplete = false;
        bool mClosed = false;
        bool mAllowAdd = true;

        bool ITezUpdateHandler.allowAdd
        {
            get { return mAllowAdd && this.gameObject.activeInHierarchy; }
            set { mAllowAdd = value; }
        }
        bool ITezUpdateHandler.isComplete { get; set; }

        protected sealed override void onCloseThis()
        {
            base.onCloseThis();

            ///设置关闭位
            ///这样是为了让控件可以立即释放资源
            ///而不是等到Destroy执行导致时间不确定
            mClosed = true;
            this.onClose(mClosed);
        }

        /// <summary>
        /// 在这里清理所有资源
        /// </summary>
        /// <param name="self_close">
        /// true:自己主动关闭自己,资源由close函数释放
        /// false:由其他对象带动进行的销毁,资源由Unity自身函数OnDestroy释放
        /// </param>
        protected virtual void onClose(bool self_close) { }

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
            if (mInitComplete)
            {
                this.onShow();
            }
        }

        protected sealed override void Awake()
        {
            base.Awake();
            mAllowAdd = true;
        }

        protected sealed override void Start()
        {
            base.Start();
            this.addDelayInitHandler();
            mInitComplete = true;
        }

        /// <summary>
        /// 延迟初始化
        /// 
        /// 处于initWidget之后
        /// 会在队列中集中处理这个过程
        /// 避免了unity的随机性
        /// </summary>
        void ITezUpdateHandler.updateOnDelayInit()
        {
            this.onDelayInit();
        }

        protected virtual void onDelayInit() { }

        void ITezUpdateHandler.updateOnMainLoopLoop(float dt)
        {
            this.onUpdateOnMainLoopLoop(dt);
        }

        protected virtual void onUpdateOnMainLoopLoop(float dt) { }

        void ITezUpdateHandler.updateOnMainLoopOnce(float dt)
        {
            this.onUpdateOnMainLoopOnce(dt);
        }

        protected virtual void onUpdateOnMainLoopOnce(float dt) { }

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            if (mInitComplete)
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
        //         public void needRefresh()
        //         {
        //             if (this.gameObject.activeInHierarchy
        //                 //                && mInited
        //                 && !mRefreshMask)
        //             {
        //                 mRefreshMask = true;
        //                 TezcatUnity.pushRefreshHandler(this);
        //             }
        //         }

        /// <summary>
        /// 刷新
        /// </summary>
        //         void ITezRefreshHandler.refresh()
        //         {
        //             this.onRefresh();
        //             mRefreshMask = false;
        //         }

        /// <summary>
        /// 立即刷新阶段
        /// </summary>
        //protected virtual void onRefresh() { }


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
    }

    /// <summary>
    /// 工具性组件基类
    /// </summary>
    public abstract class TezToolWidget : TezUIWidget
    {

    }
}