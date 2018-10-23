using System;
using tezcat.Core;
using tezcat.DataBase;
using UnityEngine;

namespace tezcat.Wrapper
{
    public interface ITezMBWrapper : ITezWrapper
    {
        TezObject getObject();
    }

    public abstract class TezMonoBehaviour
        : MonoBehaviour
        , ITezPrefab
    {
        bool m_Init = false;
        public enum RefreshState : byte
        {
            Init,
            Enable,
            Custom
        }

        private void Awake()
        {
            this.preInit();
        }

        private void Start()
        {
            if (!m_Init)
            {
                m_Init = true;
                this.initObject();
                this.linkEvent();
                this.refresh(RefreshState.Init);
            }
            else
            {
                throw new Exception("MB is Init");
            }
        }

        private void OnEnable()
        {
            if (m_Init)
            {
                this.linkEvent();
                this.refresh(RefreshState.Enable);
            }
        }

        private void OnDisable()
        {
            if (m_Init)
            {
                this.unLinkEvent();
            }
        }

        private void OnDestroy()
        {
            this.clear();
        }

        public void refresh(RefreshState state = RefreshState.Custom)
        {
            if (this.gameObject.activeSelf && m_Init)
            {
                switch (state)
                {
                    case RefreshState.Init:
                        this.onRefreshInit();
                        break;
                    case RefreshState.Enable:
                        this.onRefreshEnable();
                        break;
                    case RefreshState.Custom:
                        this.onRefreshCustom();
                        break;
                    default:
                        break;
                }
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
        /// 在这里连接你的所有事件通知
        /// </summary>
        protected abstract void linkEvent();

        /// <summary>
        /// 在这里断开你的所有事件通知
        /// </summary>
        protected abstract void unLinkEvent();

        /// <summary>
        /// 初始化时刷新数据
        /// </summary>
        protected abstract void onRefreshInit();

        /// <summary>
        /// 每当Enbale时刷新数据
        /// </summary>
        protected abstract void onRefreshEnable();

        /// <summary>
        /// 自定义情况下刷新数据
        /// </summary>
        protected abstract void onRefreshCustom();

        /// <summary>
        /// 重置你的MB
        /// </summary>
        public abstract void reset();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void clear();

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void close()
        {
            Destroy(this.gameObject);
        }
    }

    public abstract class TezGameMB : TezMonoBehaviour
    {
        public override void reset()
        {

        }

        protected override void clear()
        {

        }

        protected override void initObject()
        {

        }

        protected override void linkEvent()
        {

        }

        protected override void onRefreshInit()
        {

        }

        protected override void onRefreshEnable()
        {

        }

        protected override void onRefreshCustom()
        {

        }

        protected override void preInit()
        {

        }

        protected override void unLinkEvent()
        {

        }
    }
}