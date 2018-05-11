using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.String;
using tezcat.Utility;

namespace tezcat.Core
{
    /// <summary>
    /// 基础Object
    /// 
    /// 由UID和NID来确定的一个唯一的Object
    /// 
    /// </summary>
    public abstract class TezObject
        : TezSingleDataSlot<TezItem>
        , ITezPropertyOwner
        , ITezSelectable
    {
        /// <summary>
        /// 全局唯一ID
        /// </summary>
        public TezUID UID { get; private set; } = null;

        /// <summary>
        /// 全局唯一名称ID
        /// </summary>
        public TezStaticString NID { get; private set; } = TezStaticString.empty;

        /// <summary>
        /// 来源数据
        /// Object本身并不持有这个来源Item的数据 而是拷贝一份使用
        /// 因为数据可能会随时发生变化
        /// 并且不能影响到来源的原本数据
        /// </summary>
        public TezItem sourceItem
        {
            get { return this.myData; }
            protected set { this.myData = value; }
        }

        /// <summary>
        /// 此Object可被选择器选中
        /// 并且是Object类型数据
        /// </summary>
        public TezSelectType selectType
        {
            get { return TezSelectType.Object; }
        }

        /// <summary>
        /// 数据库定义的组别信息
        /// </summary>
        public abstract TezDatabase.GroupType groupType { get; }

        /// <summary>
        /// 数据库定义的分类信息
        /// </summary>
        public abstract TezDatabase.CategoryType categoryType { get; }

        /// <summary>
        /// 属性注册器
        /// </summary>
        protected TezPropertyManager m_PropertyManager = null;
        public TezPropertyManager propertyManager
        {
            get { return m_PropertyManager; }
        }

        public List<TezPropertyValue> properties
        {
            get { return m_PropertyManager.properties; }
        }

        public void initObject()
        {
            if (!this.UID)
            {
                this.UID = new TezUID();
                this.onInitObject();
                this.registerProperty();
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

        /// <summary>
        /// 从源数据中拷贝基础数据
        /// </summary>
        /// <param name="item"></param>
        public void copyDataFromSourceItem(TezItem item)
        {
            if (item.groupType != this.groupType || this.categoryType != item.categoryType)
            {
                return;
            }

            this.sourceItem?.subRef();
            this.sourceItem = item;
            this.sourceItem.addRef();

            this.NID = this.sourceItem.NID;

            this.onCopyDataFromSourceItem(item);
        }

        protected abstract void onCopyDataFromSourceItem(TezItem item);

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public override void clear()
        {
            this.sourceItem.subRef();
            this.sourceItem = null;

            m_PropertyManager.clear();
            m_PropertyManager = null;

            this.UID = null;
            this.NID = null;
        }

        #region 重载操作
        public static bool operator true(TezObject obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }
}

