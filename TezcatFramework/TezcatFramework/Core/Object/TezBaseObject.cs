using System;
using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezBaseObject : ITezCloseable
    {
        public string CID
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public abstract void close();
    }


    /// <summary>
    /// 游戏对象
    /// </summary>
    public abstract class TezGameObject
        : TezBaseObject
        , ITezSerializable
        , IEquatable<TezGameObject>
    {
        private uint mObjectUID = 0;

        /// <summary>
        /// UID
        /// 为0则表示没有分配
        /// </summary>
        public uint objectUID => mObjectUID;


        public void init()
        {
            mObjectUID = TezObjectUID.generateID();
            this.preInit();
            this.initNew();
            this.postInit();
        }

        public void init(ITezSerializable item)
        {
            mObjectUID = TezObjectUID.generateID();
            this.preInit();
            this.initWithItem(item);
            this.postInit();
        }

        protected virtual void preInit()
        {

        }

        protected virtual void postInit()
        {

        }

        protected virtual void initNew()
        {

        }

        protected virtual void initWithItem(ITezSerializable item)
        {

        }

        public override void close()
        {
            TezObjectUID.recycleID(mObjectUID);
        }

        public virtual void serialize(TezWriter writer)
        {

        }

        public virtual void deserialize(TezReader reader)
        {

        }

        #region Override
        public override int GetHashCode()
        {
            return mObjectUID.GetHashCode();
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals((TezGameObject)other);
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public bool Equals(TezGameObject other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mObjectUID == other.mObjectUID;
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public static bool operator ==(TezGameObject a, TezGameObject b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public static bool operator !=(TezGameObject a, TezGameObject b)
        {
            return !(a == b);
        }
        #endregion
    }
}