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
        int m_UID;
        /// <summary>
        /// 全局唯一ID
        /// </summary>
        public int UID => m_UID;

        /// <summary>
        /// 在多类型数据库存储模式下才会生效
        /// </summary>
        public int managerID
        {
            get { return m_UID >> 16; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int indexID
        {
            get { return m_UID & 0x0000_ffff; }
        }

        /// <summary>
        /// 单数据库存储方式
        /// </summary>
        /// <param name="uid">32bit UID</param>
        public TezDBID(int uid)
        {
            m_UID = uid;
        }

        /// <summary>
        /// 多类型数据库存储方式
        /// </summary>
        /// <param name="manager_id">first 16bit ID</param>
        /// <param name="uid">last 16bit ID</param>
        public TezDBID(int manager_id, int uid)
        {
            m_UID = (manager_id << 16) | uid;
        }

        public override int GetHashCode()
        {
            return m_UID.GetHashCode();
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

            return m_UID == other.m_UID;
        }

        /// <summary>
        /// 快速比较
        /// 保证输入参数不为空
        /// </summary>
        public bool fastEquals(TezDBID other)
        {
            return m_UID == other.m_UID;
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

            return a.UID == b.UID;
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