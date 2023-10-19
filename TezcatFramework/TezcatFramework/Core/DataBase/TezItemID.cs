using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /*
     * 文件数据库
     * 
     * 游戏中并不是所有的对象都需要一个ItemID进行比较
     * 只有物品类的对象才需要一个ItemID
     * 
     * 数据库应该给每一个Item一个对应的ID,用于区分不同的Item
     * ID分为两部分
     * 低位代表FixedID
     * 高位代表ModifiedID
     * 
     * 
     */

    /// <summary>
    /// 物品ID
    /// 用于判断两个物品是不是同一个
    /// 此类应该是共享数据类
    /// 不需要存在两份相同的数据
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class TezItemID
        : ITezCloseable
        , IEquatable<TezItemID>
    {
        #region Pool
        static Queue<TezItemID> sPool = new Queue<TezItemID>();
        static Queue<int> sModifiedIDPool = new Queue<int>();

        public static TezItemID create(int fixedID, int modifiedID = -1)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mFixedID = fixedID;
            result.mModifiedID = modifiedID;
            return result;
        }

        #endregion
        /// <summary>
        /// 是否运行存储
        /// </summary>
        public bool isEmpty => mID == -1;

        [FieldOffset(0)]
        long mID;
        public long ID => mID;

        [FieldOffset(0)]
        int mFixedID = -1;
        /// <summary>
        /// 固定ID
        /// 存在于数据库中的物品的ID
        /// 范围值为[0, int32.Max]
        /// </summary>
        public int fixedID => mFixedID;

        [FieldOffset(4)]
        int mModifiedID = -1;
        /// <summary>
        /// 重定义ID
        /// 用于当物品被修改时用于确定物品是否相同
        /// ID范围值为[int32.Max+1, int64.Max]
        /// 
        /// <para>
        /// 例如
        /// RPG游戏里面掉落的一个装备
        /// 圣神之强化的坚固盔甲
        /// 虽然有不同的词缀,但是都是坚固盔甲
        /// </para>
        /// 
        /// </summary>
        public int modifiedID => mModifiedID;

        private TezItemID() { }

        public void close()
        {
            mID = -1;
            sPool.Enqueue(this);
        }

        public TezItemID copy()
        {
            return create(mFixedID, mModifiedID);
        }

        public bool sameAs(TezItemID other)
        {
            //             if (mFixedID == -1 || other.mFixedID == -1)
            //             {
            //                 return false;
            //             }

            return mID == other.mID;
        }

        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezItemID)obj);
        }

        public bool Equals(TezItemID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.sameAs(other);
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator ==(TezItemID a, TezItemID b)
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
        public static bool operator !=(TezItemID a, TezItemID b)
        {
            return !(a == b);
        }
    }
}