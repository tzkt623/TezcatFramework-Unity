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
        bool m_Dirty = false;

        public void refresh()
        {
            m_Dirty = true;
            if (m_Dirty && m_Init && this.gameObject.activeSelf)
            {
                m_Dirty = false;
                this.onRefresh();
            }
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
                this.refresh();
            }
        }

        private void OnEnable()
        {
            if (m_Init)
            {
                this.linkEvent();
                this.refresh();
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
        /// 在这里刷新你的MB数据
        /// </summary>
        protected abstract void onRefresh();

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

        protected override void onRefresh()
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