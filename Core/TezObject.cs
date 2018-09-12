using System;
using tezcat.DataBase;
using tezcat.String;

namespace tezcat.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezObject
        : ITezCloseable
    {
        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public abstract void close();


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

    /// <summary>
    /// 游戏Object
    /// </summary>
    public abstract class TezGameObject
        : TezObject
        , ITezSerializable
        , ITezTagSet
    {
        /// <summary>
        /// 全局唯一ID
        /// </summary>
        public TezUID UID { get; private set; } = null;

        /// <summary>
        /// 全局唯一名称ID
        /// </summary>
        public TezStaticString NID { get; private set; } = null;

        /// <summary>
        /// 标签
        /// </summary>
        public TezTagSet TAG { get; private set; } = null;

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initObject()
        {
            if (!this.UID)
            {
                this.UID = new TezUID();
                this.NID = new TezStaticString();
                this.TAG = new TezTagSet();
                this.onInitObject();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} >> This Object is Init Again", this.GetType().Name));
            }
        }

        protected virtual void onInitObject()
        {

        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            this.TAG.close();
            this.TAG = null;

            this.UID.close();
            this.UID = null;

            this.NID.close();
            this.NID = null;
        }

        public virtual void serialize(TezWriter writer)
        {
            writer.write("TID", this.GetType().Name);
            writer.write("NID", this.NID);
        }

        public virtual void deserialize(TezReader reader)
        {

        }
    }

    /// <summary>
    /// 工具Object
    /// </summary>
    public abstract class TezToolObject : TezObject
    {

    }
}

