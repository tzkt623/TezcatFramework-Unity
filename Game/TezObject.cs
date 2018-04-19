using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.TypeTraits;
using tezcat.Utility;

namespace tezcat
{
    public abstract class TezObject
        : ITezPropertyOwner
    {
        public TezUID UID { get; private set; }

        public TezItem sharedItem
        {
            get; protected set;
        }

        public abstract TezType groupType { get; }
        public abstract TezType categoryType { get; }

        protected TezPropertyManager m_PropertyManager = null;
        public TezPropertyManager propertyManager
        {
            get { return m_PropertyManager; }
        }

        public List<TezPropertyValue> properties
        {
            get { return m_PropertyManager.properties; }
        }

        public TezObject()
        {
            UID = new TezUID();
            this.registerProperty();
        }

        /// <summary>
        /// 注册当前Item的所有属性
        /// </summary>
        public void registerProperty()
        {
            m_PropertyManager = new TezPropertyManager(this);
            this.onRegisterProperty(m_PropertyManager);
            m_PropertyManager.sortProperties();
        }

        /// <summary>
        /// 在这里注册你的Item属性
        /// </summary>
        /// <param name="manager"></param>
        protected abstract void onRegisterProperty(TezPropertyManager manager);

        public virtual void clear()
        {
            this.sharedItem.subRef();
            this.sharedItem = null;
        }

        public void setItem(TezItem item)
        {
            if(item.groupType != this.groupType || this.categoryType != item.categoryType)
            {
                return;
            }

            this.sharedItem?.subRef();
            this.sharedItem = item;
            this.sharedItem.addRef();

            this.onItemSet(item);
        }

        protected abstract void onItemSet(TezItem item);
    }
}

