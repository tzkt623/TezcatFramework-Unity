using tezcat.Framework.Database;
using UnityEngine;

namespace tezcat.Framework.Core
{
    public abstract class TezMonoObject
        : MonoBehaviour
        , ITezRefresher
        , ITezPrefab
    {
        bool m_Init = false;

        byte m_DirtyCount = 0;
        TezRefreshPhase m_DirtyMask = 0;
        TezRefreshPhase[] m_RefreshPhaseArray = new TezRefreshPhase[8];
        ITezRefresher m_NextRefresher = null;
        ITezRefresher ITezRefresher.next
        {
            get
            {
                var temp = m_NextRefresher;
                m_NextRefresher = null;
                return temp;
            }
            set
            {
                m_NextRefresher = value;
            }
        }
        public TezRefreshPhase refreshPhase
        {
            set
            {
                if (this.gameObject.activeSelf)
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


        protected void Awake()
        {
            this.preInit();
        }

        protected void Start()
        {
            this.initObject();
            this.linkEvent();
            this.refreshPhase = TezRefreshPhase.P_OnInit;
            m_Init = true;
        }

        protected void OnEnable()
        {
            if (m_Init)
            {
                this.linkEvent();
                this.refreshPhase = TezRefreshPhase.P_OnEnable;
            }
        }

        protected void OnDisable()
        {
            if (m_Init)
            {
                this.unLinkEvent();
            }
        }

        protected void OnDestroy()
        {

        }

        public void refresh()
        {
            for (int i = 0; i < m_DirtyCount; i++)
            {
                this.onRefresh(m_RefreshPhaseArray[i]);
            }

            m_DirtyCount = 0;
            m_DirtyMask = 0;
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
        /// 自定义情况下刷新数据
        /// </summary>
        protected abstract void onRefresh(TezRefreshPhase phase);

        /// <summary>
        /// 重置你的MB
        /// </summary>
        public abstract void reset();

        /// <summary>
        /// 关闭组件
        /// </summary>
        protected abstract void onClose();

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
            this.onClose();
            Destroy(this.gameObject);
        }
    }
}