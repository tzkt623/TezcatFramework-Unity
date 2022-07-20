using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.ECS
{
    public abstract class TezDataComponent
        : TezBaseComponent
        , IEquatable<TezDataComponent>
        , ITezItemTypeID
    {
        /// <summary>
        /// 注册ID
        /// </summary>
        public static int SComUID;
        public sealed override int comUID => SComUID;

        /// <summary>
        /// UID
        /// 为0则表示没有分配
        /// </summary>
        public uint objectUID { get; private set; } = 0;

        /// <summary>
        /// 物品类型ID
        /// 如果此ID为-1
        /// 则表示此对象没有分类系统
        /// </summary>
        public int itemTypeID { get; set; } = -1;

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; protected set; }

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initNew()
        {
            this.preInit();
            this.onInitNew();
            this.postInit();
        }

        /// <summary>
        /// 新建一个白板对象
        /// 不依赖数据模板
        /// </summary>
        protected virtual void onInitNew()
        {

        }

        /// <summary>
        /// 使用物品模板初始化对象
        /// </summary>
        public void initWithData(ITezSerializableItem item)
        {
            var data_item = (TezDatabaseGameItem)item;
            this.NID = data_item.NID;
            this.itemTypeID = data_item.itemTypeID;

            this.preInit();
            this.onInitWithData(item);
            this.postInit();
        }

        protected virtual void onInitWithData(ITezSerializableItem item)
        {

        }

        /// <summary>
        /// 数据初始化之前
        /// </summary>
        protected virtual void preInit()
        {
            this.objectUID = TezObjectUID.generateID();
        }

        /// <summary>
        /// 数据初始化之后
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            TezObjectUID.recycleID(this.objectUID);
            this.NID = null;
        }

        #region Override
        public override int GetHashCode()
        {
            return this.objectUID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezDataComponent)other);
        }

        public bool Equals(TezDataComponent other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.objectUID == other.objectUID;
        }

        public static bool operator ==(TezDataComponent a, TezDataComponent b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezDataComponent a, TezDataComponent b)
        {
            return !(a == b);
        }
        #endregion
    }
}