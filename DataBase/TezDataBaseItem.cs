using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 基础数据库Item类
    /// </summary>
    public abstract class TezDatabaseItem
        : IEquatable<TezDatabaseItem>
        , ITezSerializableItem
        , ITezCloseable
    {
        /// <summary>
        /// Name ID
        /// </summary>
        public string NID { get; set; } = null;

        public TezDBID DBID { get; private set; }

        public void onRegister(int db_id, int item_id)
        {
            this.DBID = new TezDBID(db_id, item_id);
        }

        public virtual void serialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        public virtual void deserialize(TezReader reader)
        {
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        public virtual void close()
        {
            DBID.close();
            DBID = null;
        }

        #region 重载
        public override bool Equals(object other)
        {
            return this.Equals((TezDatabaseItem)other);
        }

        public bool Equals(TezDatabaseItem other)
        {
            return other ? DBID.sameAs(other.DBID) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TezDatabaseItem a, TezDatabaseItem b)
        {
            var flagA = object.ReferenceEquals(a, null);
            var flagB = object.ReferenceEquals(b, null);

            ///(true && true) || (!true && !true && a == b)
            ///true || (?)
            ///
            ///(true && false) || (!true && !false && a== b)
            ///false || (false && ?)
            ///
            ///(false && false) || (!false && !false && a== b)
            ///false || (true && a == b)
            ///
            return (flagA && flagB) || (!flagA && !flagB && a.Equals(b));
        }

        public static bool operator !=(TezDatabaseItem a, TezDatabaseItem b)
        {
            var flagA = object.ReferenceEquals(a, null);
            var flagB = object.ReferenceEquals(b, null);

            ///(!true || !true) && (true || true || a != b)
            ///false && (?)
            ///
            ///(!true || !false) && (true || false || 
            ///true && (true || ?)
            ///
            ///(!false || !false) && (false || false) || (a != b)
            ///true && (false || a != b)
            ///
            return (!flagA || !flagB) && (flagA || flagB || !a.Equals(b));
        }

        public static bool operator true(TezDatabaseItem item)
        {
            return !object.ReferenceEquals(item, null);
        }

        public static bool operator false(TezDatabaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }

        public static bool operator !(TezDatabaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }
        #endregion
    }

    /// <summary>
    /// 图片文本Item
    /// </summary>
    public abstract class TezDataBaseAssetItem : TezDatabaseItem
    {

    }

    /// <summary>
    /// 游戏对象Item
    /// </summary>
    public abstract class TezDatabaseGameItem : TezDatabaseItem
    {
        /// <summary>
        /// Class ID
        /// </summary>
        public string CID { get; private set; }


        public TezCategory category { get; protected set; } = new TezCategory();

        /// <summary>
        /// 允许堆叠的数量
        /// </summary>
        public int stackCount { get; protected set; } = 0;


        public List<string> TAGS { get; private set; } = new List<string>();

        /// <summary>
        /// 建立Item的分类
        /// </summary>
        private List<ITezCategoryToken> buildCategory
        {
            get
            {
                List<ITezCategoryToken> list = new List<ITezCategoryToken>(4);
                this.onBuildCategory(ref list);
                return list;
            }
        }

        /// <summary>
        /// 使用Category系统
        /// 建立Item的分类路径
        /// </summary>
        protected abstract void onBuildCategory(ref List<ITezCategoryToken> list);

        public TezDatabaseGameItem()
        {
            this.category.setToken(buildCategory);
        }

        public override void close()
        {
            base.close();

            this.CID = null;

            this.TAGS.Clear();
            this.TAGS = null;
        }

        public TezEntity createObject()
        {
            var obj = this.onCreateObject();
            obj.initWithData(this);

            var entity = TezEntity.create();
            entity.addComponent(obj);
            return entity;
        }

        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        protected virtual TezGameObject onCreateObject()
        {
            throw new Exception(string.Format("Please override this method for {0}", this.GetType().Name));
        }
    }
}