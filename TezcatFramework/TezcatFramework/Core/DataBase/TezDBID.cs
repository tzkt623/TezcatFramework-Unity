using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 数据库ID
    /// 对应数据库中的存储位置
    /// </summary>
    public class TezDBID
        : ITezNonCloseable
        , IEquatable<TezDBID>
    {
        int mUID;
        /// <summary>
        /// 全局唯一ID
        /// </summary>
        public int UID => mUID;

        /// <summary>
        /// 多类型数据库存储模式下才会生效
        /// </summary>
        public int managerID
        {
            get { return mUID >> 16; }
        }

        /// <summary>
        /// 多类型数据库存储模式下才会生效
        /// </summary>
        public int indexID
        {
            get { return mUID & 0x0000_ffff; }
        }

        /// <summary>
        /// 单数据库存储方式
        /// </summary>
        /// <param name="uid">32bit UID</param>
        public TezDBID(int uid)
        {
            mUID = uid;
        }

        /// <summary>
        /// 多类型数据库存储方式
        /// </summary>
        /// <param name="managerID">first 16bit ID</param>
        /// <param name="UID">last 16bit ID</param>
        public TezDBID(int managerID, int UID)
        {
            mUID = (managerID << 16) | UID;
        }

        public override int GetHashCode()
        {
            return mUID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezDBID)obj);
        }

        public bool Equals(TezDBID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.sameAs(other);
        }

        /// <summary>
        /// 快速比较
        /// 保证输入参数不为空
        /// </summary>
        public bool sameAs(TezDBID other)
        {
            return mUID == other.mUID;
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator ==(TezDBID a, TezDBID b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.sameAs(b);
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator !=(TezDBID a, TezDBID b)
        {
            return !(a == b);
        }
    }
}