﻿using System;
using LitJson;
using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezBaseObject
        : ITezCloseable
        , ITezSerializable
        , IEquatable<TezBaseObject>
    {
        public virtual string CID
        {
            get { return this.GetType().Name; }
        }

        private uint mObjectUID = TezObjectUID.generateID();
        /// <summary>
        /// UID
        /// 为0则表示没有分配
        /// </summary>
        public uint objectUID => mObjectUID;

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public virtual void close()
        {
            TezObjectUID.recycleID(mObjectUID);
            mObjectUID = 0;
        }

        public abstract void serialize(TezWriter writer);
        public abstract void deserialize(TezReader reader);


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
        /// 如需比较内存地址请使用
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
    }
}