namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezBaseObject
        : ITezCloseable
        , ITezSerializable
        //, IEquatable<TezBaseObject>
    {
        public virtual string CID
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public abstract void close();

        /// <summary>
        /// 序列化对象
        /// </summary>
        public abstract void serialize(TezWriter writer);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        public abstract void deserialize(TezReader reader);

        /*
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
            return this.Equals((TezBaseObject)other);
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public bool Equals(TezBaseObject other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mObjectUID == other.mObjectUID;
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// 如需比较内存地址请使用object.ReferenceEquals
        /// </summary>
        public static bool operator ==(TezBaseObject a, TezBaseObject b)
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
        public static bool operator !=(TezBaseObject a, TezBaseObject b)
        {
            return !(a == b);
        }
        #endregion
        */
    }
}