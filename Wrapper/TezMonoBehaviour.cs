using System;
using tezcat.Framework.Core;
using tezcat.Framework.DataBase;
using UnityEngine;

namespace tezcat.Framework.Wrapper
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
                this.refreshAfterInit();
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
                this.refreshOnEnable();
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

        public void refresh(RefreshPhase phase)
        {
            if (this.gameObject.activeSelf && m_Init)
            {
                this.onRefresh(phase);
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
        protected abstract void refreshAfterInit();

        /// <summary>
        /// 每当Enbale时刷新数据
        /// </summary>
        protected abstract void refreshOnEnable();

        /// <summary>
        /// 自定义情况下刷新数据
        /// </summary>
        protected abstract void onRefresh(RefreshPhase state);

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

        protected override void refreshAfterInit()
        {

        }

        protected override void refreshOnEnable()
        {

        }

        protected override void onRefresh(RefreshPhase state)
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