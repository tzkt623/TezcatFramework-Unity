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
        public TezDBID DBID { get; private set; }

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
        /// </summary>
        public int stackCount { get; private set; } = 0;

        public void onRegister(int dbUID, int itemID)
        {
            this.DBID = new TezDBID(dbUID, itemID);
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
            DBID.close();
            DBID = null;

            this.category = null;
            this.NID = null;
            this.CID = null;
            this.TAGS = null;
        }

        protected virtual void onSerialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);
            if (this.category != null)
            {
                writer.write(TezReadOnlyString.CTG_FT, this.category.finalToken.toName);
            }
        }

        protected virtual void onDeserialize(TezReader reader)
        {
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);
            if (reader.tryRead(TezReadOnlyString.CTG_FT, out string final_token_name))
            {
                this.category = TezCategorySystem.getCategory(final_token_name);
            }
        }

        public TezEntity createEntity()
        {
            var obj = this.onCreateObject();
            obj.initWithData(this);

            var entity = TezEntity.create();
            entity.addComponent(obj);
            return entity;
        }

        protected virtual TezComData onCreateObject()
        {
            throw new Exception(string.Format("Please override this method for {0}", this.GetType().Name));
        }

        #region 重写
        public override int GetHashCode()
        {
            return this.DBID.GetHashCode();
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

            return DBID.Equals(other.DBID);
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