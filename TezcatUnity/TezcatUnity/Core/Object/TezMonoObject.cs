using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Unity.Core
{
    /// <summary>
    /// 基础MonoObject
    /// </summary>
    public abstract class TezMonoObject
        : MonoBehaviour
        , ITezRefreshHandler
        , ITezDelayInitHandler
        , ITezCloseable
    {
        bool mInited = false;
        bool mClosed = false;
        bool mRefresh = false;

        protected void Awake()
        {
            this.preInit();
        }

        protected void Start()
        {
            this.initObject();
            TezcatUnity.pushDelayInitHandler(this);
        }

        protected void OnEnable()
        {
            if (mInited)
            {
                this.onShow();
                this.needRefresh();
            }
        }

        protected void OnDisable()
        {
            if (mInited)
            {
                this.onHide();
            }
        }

        protected void OnDestroy()
        {
            if (!mClosed)
            {
                this.onClose();
            }
        }

        protected virtual void onDelayInit() { }

        void ITezDelayInitHandler.delayInit()
        {
            this.onDelayInit();
            mInited = true;
        }

        void ITezRefreshHandler.refresh()
        {
            mRefresh = false;
            this.onRefresh();
        }

        public void needRefresh()
        {
            if (this.gameObject.activeInHierarchy
//                && mInited
                && !mRefresh)
            {
                mRefresh = true;
                TezcatUnity.pushRefreshHandler(this);
            }
        }

        /// <summary>
        /// 在MB初始化之前调用
        /// </summary>
        protected abstract void preInit();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void initObject();

        /// <summary>
        /// 初始化刷新阶段
        /// </summary>
        protected virtual void onRefresh() { }


        /// <summary>
        /// 重置你的MB
        /// </summary>
        public abstract void reset();

        /// <summary>
        /// 关闭组件
        /// </summary>
        protected abstract void onClose();

        /// <summary>
        /// 在显示时调用
        /// </summary>
        protected abstract void onShow();

        /// <summary>
        /// 隐藏时调用
        /// </summary>
        protected abstract void onHide();

        /// <summary>
        /// 显示此对象
        /// </summary>
        public void show()
        {
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏此对象
        /// </summary>
        public void hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void close()
        {
            mClosed = true;
            this.onClose();
            Destroy(this.gameObject);
        }
    }
}