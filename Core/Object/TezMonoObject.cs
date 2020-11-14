using UnityEngine;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 非ECS系统基础Object
    /// </summary>
    public abstract class TezMonoObject
        : MonoBehaviour
        , ITezRefreshHandler
        , ITezCloseable
    {
        bool m_Inited = false;
        bool m_Closed = false;

        //         byte m_DirtyCount = 0;
        //         TezRefreshPhase m_DirtyMask = 0;
        //         TezRefreshPhase[] m_RefreshPhaseArray = new TezRefreshPhase[8];
        //         ITezRefresher m_NextRefresher = null;
        //         ITezRefresher ITezRefresher.next
        //         {
        //             get
        //             {
        //                 var temp = m_NextRefresher;
        //                 m_NextRefresher = null;
        //                 return temp;
        //             }
        //             set
        //             {
        //                 m_NextRefresher = value;
        //             }
        //         }

        TezRefreshPhase m_RefreshPhase = TezRefreshPhase.Ready;
        public TezRefreshPhase refreshPhase
        {
            set
            {
                if (this.gameObject.activeInHierarchy && m_Inited)
                {
                    //                     if ((m_DirtyMask & value) == 0)
                    //                     {
                    //                         if (m_DirtyCount == 0)
                    //                         {
                    //                             TezService.get<TezcatFramework>().pushRefresher(this);
                    //                         }
                    // 
                    //                         m_DirtyMask |= value;
                    //                         m_RefreshPhaseArray[m_DirtyCount++] = value;
                    //                     }

                    //                    switch (value)
                    //                     {
                    //                         case TezRefreshPhase.P_Immediately:
                    //                             this.onRefreshImmediately();
                    //                             break;
                    //                         case TezRefreshPhase.P_Delayed:
                    //                             if (m_RefreshPhase == TezRefreshPhase.P_Empty)
                    //                             {
                    //                                 TezService.get<TezcatFramework>().pushRefresher(this);
                    //                                 m_RefreshPhase = value;
                    //                             }
                    //                             break;
                    //                     }

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


        protected void Awake()
        {
            this.preInit();
        }

        protected void Start()
        {
            this.initObject();
            m_Inited = true;
            this.refreshPhase = TezRefreshPhase.Refresh;
        }

        protected void OnEnable()
        {
            if (m_Inited)
            {
                this.onShow();
                this.refreshPhase = TezRefreshPhase.Refresh;
            }
        }

        protected void OnDisable()
        {
            if (m_Inited)
            {
                this.onHide();
            }
        }

        protected void OnDestroy()
        {
            if (!m_Closed)
            {
                this.onClose(m_Closed);
            }
        }

        public void refresh()
        {
            //             for (int i = 0; i < m_DirtyCount; i++)
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
        protected abstract void onClose(bool self_close);

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
            m_Closed = true;
            this.onClose(m_Closed);
            Destroy(this.gameObject);
        }
    }
}