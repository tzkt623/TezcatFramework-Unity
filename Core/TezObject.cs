using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.String;
using tezcat.Utility;

namespace tezcat.Core
{
    public abstract class TezObject
        : TezSingleDataSlot<TezItem>
        , ITezPropertyOwner
    {
        public TezUID UID { get; private set; }
        public TezStaticString nameID { get; private set; }

        public TezItem databaseItem
        {
            get { return this.data; }
            protected set { this.data = value; }
        }

        public abstract TezDatabase.GroupType groupType { get; }
        public abstract TezDatabase.CategoryType categoryType { get; }

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

        bool m_Init = false;
        public void initObject()
        {
            if (!m_Init)
            {
                UID = new TezUID();
                this.onInitObject();
                this.registerProperty();
                m_Init = true;
            }
        }

        protected virtual void onInitObject()
        {

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

        public override void clear()
        {
            this.databaseItem.subRef();
            this.databaseItem = null;
        }

        public void copyDataFromItem(TezItem item)
        {
            if (item.groupType != this.groupType || this.categoryType != item.categoryType)
            {
                return;
            }

            this.databaseItem?.subRef();
            this.databaseItem = item;
            this.databaseItem.addRef();

            this.nameID = this.databaseItem.nameID;

            this.onCopyDataFromItem(item);
        }

        protected abstract void onCopyDataFromItem(TezItem item);


        public static bool operator true(TezObject obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }
    }
}

