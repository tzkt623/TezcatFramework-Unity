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
        : TezSingleDataSlot
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
        public TezStaticString NID { get; private set; } = new TezStaticString();

        /// <summary>
        /// 来源数据
        /// 
        /// Object本身并不持有这个来源Item的数据
        /// 而是拷贝一份使用
        /// 因为数据可能会随时发生变化
        /// 并且不能影响到来源数据
        /// </summary>
        public TezItem sourceItem
        {
            get; protected set;
        }

        /// <summary>
        /// 此Object可被选择器选中
        /// 并且是Object类型数据
        /// </summary>
        public TezSelectorType selectorType
        {
            get { return TezSelectorType.Object; }
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
        /// 标签
        /// </summary>
        public TezTagSet tags { get; private set; } = new TezTagSet();

        /// <summary>
        /// 属性
        /// </summary>
        public TezPropertyManager propertyManager { get; private set; }
        public List<TezPropertyValue> properties
        {
            get { return propertyManager.properties; }
        }

        public static T create<T>() where T : TezObject, new()
        {
            var v = new T();
            v.initObject();
            return v;
        }

        /// <summary>
        /// 初始化Object
        /// </summary>
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

        public override ITezData getTezData()
        {
            return this.sourceItem;
        }

        /// <summary>
        /// 注册当前Item的所有属性
        /// </summary>
        private void registerProperty()
        {
            this.propertyManager = new TezPropertyManager(this);
            this.onRegisterProperty(propertyManager);
            propertyManager.sortProperties();
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
            this.sourceItem = item;
            this.NID = this.sourceItem.NID;
            this.onCopyDataFromSourceItem(item);
        }

        protected abstract void onCopyDataFromSourceItem(TezItem item);

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public override void close()
        {
            this.sourceItem = null;

            propertyManager.clear();
            propertyManager = null;

            tags.clear();
            tags = null;

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

