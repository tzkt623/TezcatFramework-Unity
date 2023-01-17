using System;
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
        , ITezSerializable
        , ITezCloseable
    {
        /// <summary>
        /// Class ID
        /// </summary>
        public string CID { get; private set; }

        /// <summary>
        /// Name ID
        /// </summary>
        public string NID { get; set; }

        /// <summary>
        /// 数据库ID
        /// 用于固定数据库
        /// </summary>
        public int DBID => mItemID.fixedID;

        /// <summary>
        /// 重定义ID
        /// 用于运行时数据库
        /// </summary>
        public int ModifiedID => mItemID.modifiedID;

        TezItemID mItemID = null;
        /// <summary>
        /// 物品ID
        /// </summary>
        public TezItemID itemID => mItemID;

        protected TezCategory mCategory = null;
        /// <summary>
        /// 分类系统
        /// </summary>
        public TezCategory category => mCategory;

        /// <summary>
        /// Tags用于对Item进行分类
        /// 如果为空
        /// 则表示不需要分类
        /// </summary>
        public string[] TAGS = null;

        protected int mStackCount = -1;
        /// <summary>
        /// 可堆叠数量
        /// 大于0表示可以堆叠
        /// 等于0表示不允许堆叠
        /// 等于-1表示没有这个属性
        /// </summary>
        public int stackCount => mStackCount;

        int mModifiedRefCount = 0;
        /// <summary>
        /// 重定义引用
        /// </summary>
        public int refModifiedCount => mModifiedRefCount;

        public void retainModifiedItem()
        {
            mModifiedRefCount++;
        }

        public bool releaseModifiedItem()
        {
            mModifiedRefCount--;
            if (mModifiedRefCount == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 注册数据库ID
        /// </summary>
        public void onRegister(int dbID)
        {
            TezItemID.create(ref mItemID, dbID);
            this.postRegister();
        }

        /// <summary>
        /// 注册运行时数据库ID
        /// </summary>
        public void onRuntimeRegister(int dbID, int modifiedID)
        {
            TezItemID.create(ref mItemID, dbID, modifiedID);
            this.postRegister();
        }

        protected virtual void postRegister()
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
            mItemID.close();

            mItemID = null;
            mCategory = null;
            this.NID = null;
            this.CID = null;
            this.TAGS = null;
        }

        protected virtual void onSerialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);

            writer.beginObject(TezReadOnlyString.ItemID);
            writer.write(TezReadOnlyString.CellID, TezDBIDGenerator.getCellID(mItemID.fixedID));
            writer.write(TezReadOnlyString.TypeID, TezDBIDGenerator.getTypeID(mItemID.fixedID));
            writer.write(TezReadOnlyString.MDID, mItemID.modifiedID);
            writer.endObject(TezReadOnlyString.ItemID);

            writer.write(TezReadOnlyString.StackCount, mStackCount);

            if (object.ReferenceEquals(mCategory, null))
            {
                writer.write(TezReadOnlyString.Category, mCategory.finalToken.name);
            }
        }

        protected virtual void onDeserialize(TezReader reader)
        {
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);

            reader.beginObject(TezReadOnlyString.ItemID);
            var cell_id = reader.readInt(TezReadOnlyString.CellID);
            var type_id = reader.readInt(TezReadOnlyString.TypeID);
            TezItemID.create(ref mItemID, TezDBIDGenerator.generateID(cell_id, type_id), reader.readInt(TezReadOnlyString.MDID));
            reader.endObject(TezReadOnlyString.ItemID);

            mStackCount = reader.readInt(TezReadOnlyString.StackCount);

            if (reader.tryRead(TezReadOnlyString.Category, out string final_token))
            {
                mCategory = TezCategorySystem.getCategory(final_token);
            }
        }

        public TezEntity createEntity()
        {
            var entity = TezEntity.create();

            //             var info = this.createInfoComponent();
            //             info.loadItemData(this);
            //             entity.addComponent(info);

            var data = this.createDataComponent();
            entity.addComponent(data);

            return entity;
        }

        protected virtual TezDataComponent createDataComponent()
        {
            throw new Exception(string.Format("{0} : Please override this method[createDataComponent]", this.GetType().Name));
        }

        public virtual TezGameObject createGameObject()
        {
            throw new Exception(string.Format("{0} : Please override this method[createGameObject]", this.GetType().Name));
        }

        //         protected virtual TezInfoComponent createInfoComponent()
        //         {
        //             return new TezInfoComponent();
        //         }

        #region 重写
        public override int GetHashCode()
        {
            return mItemID.GetHashCode();
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

            return mItemID.sameAs(other.mItemID);
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