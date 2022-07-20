using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 游戏对象Item的数据
    /// 比如 炮 飞船 技能 天赋等等
    /// </summary>
    public abstract class TezDatabaseGameItem
        : IEquatable<TezDatabaseGameItem>
        , ITezSerializableItem
        , ITezCloseable
    {
        /// <summary>
        /// 数据库ID
        /// 如果为-1则表示此类型没有分类类型
        /// </summary>
        public int itemTypeID { get; private set; }

        /// <summary>
        /// 是否是模板物品
        /// </summary>
        public bool isTemplate
        {
            get { return this.itemTypeID >= 0; }
        }

        /// <summary>
        /// Class ID
        /// </summary>
        public string CID { get; private set; }

        /// <summary>
        /// Name ID
        /// </summary>
        public string NID { get; set; }

        /// <summary>
        /// 分类系统
        /// </summary>
        public TezCategory category { get; protected set; }

        /// <summary>
        /// Tags用于对Item进行分类
        /// 如果为空
        /// 则表示不需要分类
        /// </summary>
        public string[] TAGS = null;

        /// <summary>
        /// 可堆叠数量
        /// 大于0表示可以堆叠
        /// 等于0表示不允许堆叠
        /// 等于-1表示没有这个属性
        /// </summary>
        public int stackCount { get; private set; } = -1;

        /// <summary>
        /// 单数据库存储方式
        /// </summary>
        public void onRegister(int typeID)
        {
            this.itemTypeID = typeID;
            this.customRegister();
        }

        /// <summary>
        /// 多数据库存储方式
        /// manager_id为一级分类
        /// type_id为二级分类
        /// </summary>
        public void onRegister(int managerID, int typeID)
        {
            this.itemTypeID = TezItemTypeID.generateID(managerID, typeID);
            this.customRegister();
        }

        protected virtual void customRegister()
        {

        }

        public void serialize(TezWriter writer)
        {
            this.onSerialize(writer);
        }

        public void deserialize(TezReader reader)
        {
            this.onDeserialize(reader);
        }

        public virtual void close()
        {
            this.category = null;
            this.NID = null;
            this.CID = null;
            this.TAGS = null;
        }

        protected virtual void onSerialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);

            if (this.itemTypeID >= 0)
            {
                writer.beginObject("ItemTypeID");
                writer.write("MID", TezItemTypeID.getManagerID(this.itemTypeID));
                writer.write("TID", TezItemTypeID.getTypeID(this.itemTypeID));
                writer.endObject("ItemTypeID");
            }

            if (this.stackCount >= 0)
            {
                writer.write("StackCount", this.stackCount);
            }
        }

        protected virtual void onDeserialize(TezReader reader)
        {
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);

            if (reader.tryBeginObject("ItemTypeID"))
            {
                this.itemTypeID = TezItemTypeID.generateID(reader.readInt("MID"), reader.readInt("TID"));
                reader.endObject("ItemTypeID");
            }


            if (reader.tryRead("StackCount", out int stack_count))
            {
                this.stackCount = stackCount;
            }
            else
            {
                this.stackCount = -1;
            }
        }

        public TezEntity createEntity()
        {
            var entity = TezEntity.create();

            var info = this.createInfoComponent();
            info.loadItemData(this);
            entity.addComponent(info);

            var data = this.createDataComponent();
            data.initWithData(this);
            entity.addComponent(data);

            return entity;
        }

        protected virtual TezDataComponent createDataComponent()
        {
            throw new Exception(string.Format("{0} : Please override this method!!!", this.GetType().Name));
        }

        protected virtual TezInfoComponent createInfoComponent()
        {
            return new TezInfoComponent();
        }

        #region 重写
        public override int GetHashCode()
        {
            return this.itemTypeID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezDatabaseGameItem)other);
        }

        public bool Equals(TezDatabaseGameItem other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return itemTypeID.Equals(other.itemTypeID);
        }

        public static bool operator ==(TezDatabaseGameItem a, TezDatabaseGameItem b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezDatabaseGameItem a, TezDatabaseGameItem b)
        {
            return !(a == b);
        }
        #endregion
    }
}