using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Unity.Core
{
    /// <summary>
    /// 基础MonoObject
    /// </summary>
    public abstract class TezMonoObject
        : MonoBehaviour
        , ITezUpdateHandler
        , ITezCloseable
    {
        bool mInitComplete = false;
        bool mClosed = false;
        bool mAllowAdd = false;

        bool ITezUpdateHandler.allowAdd
        {
            get { return this.gameObject.activeInHierarchy && mAllowAdd; }
            set { mAllowAdd = value; }
        }

        bool ITezUpdateHandler.isComplete { get; set; }

        private void Awake()
        {
            mAllowAdd = true;
            this.preInit();
        }

        private void Start()
        {
            this.initObject();
            this.addDelayInitHandler();
            mInitComplete = true;
        }

        private void OnEnable()
        {
            if (mInitComplete)
            {
                this.onShow();
            }
        }

        private void OnDisable()
        {
            if (mInitComplete)
            {
                this.onHide();
            }
        }

        private void OnDestroy()
        {
            if (!mClosed)
            {
                this.onClose();
            }
        }

        void ITezUpdateHandler.updateOnDelayInit()
        {
            this.onDelayInit();
        }

        protected virtual void onDelayInit() { }

        void ITezUpdateHandler.updateOnMainLoopLoop(float dt)
        {
            this.onUpdateOnMainLoop(dt);
        }

        /// <summary>
        /// 初始化刷新阶段
        /// </summary>
        protected virtual void onUpdateOnMainLoop(float dt) { }

        void ITezUpdateHandler.updateOnMainLoopOnce(float dt)
        {
            this.onUpdateOnMainLoopOnce(dt);
        }

        protected virtual void onUpdateOnMainLoopOnce(float dt)
        {

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