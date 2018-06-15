using tezcat.Core;
using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;

namespace tezcat.Wrapper
{
    public abstract class TezObjectMB
        : MonoBehaviour
        , ITezObjectWrapper
        , ITezPrefab
    {
        bool m_Init = false;
        bool m_Dirty = false;

        public string myName
        {
            get { return TezTranslator.translateName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(this.getObject().NID); }
        }

        public TezDatabase.GroupType group
        {
            get { return this.getObject().groupType; }
        }

        public TezDatabase.CategoryType category
        {
            get { return this.getObject().categoryType; }
        }

        public abstract TezObject getObject();

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
            this.unLinkEvent();
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

    public abstract class TezObjectMB<T> : TezObjectMB where T : TezObject
    {
        public T myObject { get; protected set; }

        public sealed override TezObject getObject()
        {
            return this.myObject;
        }

        public void bind(T my_object)
        {
            this.myObject = my_object;
            this.onBind();
            this.refresh();
        }

        protected abstract void onBind();

        protected override void clear()
        {
            this.myObject = null;
        }
    }
}