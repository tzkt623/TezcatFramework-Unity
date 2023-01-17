using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 物品ID
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class TezItemID
        : ITezCloseable
        , IEquatable<TezItemID>
    {
        #region Pool
        public static readonly TezItemID EmptyID = new TezItemID();
        static Queue<TezItemID> sPool = new Queue<TezItemID>();

        public static bool create(ref TezItemID itemID)
        {
            itemID?.close();

            if (sPool.Count > 0)
            {
                itemID = sPool.Dequeue();
                return false;
            }

            itemID = new TezItemID();
            return true;
        }

        /// <summary>
        /// 返回True表示新建了新的ID对象
        /// 返回False表示使用了回收的对象
        /// </summary>
        public static bool create(ref TezItemID itemID, int DBID, int modifiedID = -1)
        {
            itemID?.close();

            if (sPool.Count > 0)
            {
                itemID = sPool.Dequeue();
                itemID.mFixedID = DBID;
                itemID.mModifiedID = modifiedID;
                return false;
            }

            itemID = new TezItemID();
            itemID.mFixedID = DBID;
            itemID.mModifiedID = modifiedID;
            return true;
        }


        /// <summary>
        /// 返回True表示新建了新的ID对象
        /// 返回False表示使用了回收的对象
        /// </summary>
        public static void create(ref TezItemID itemID, TezItemID source)
        {
            itemID?.close();

            if (sPool.Count > 0)
            {
                itemID = sPool.Dequeue();
            }
            else
            {
                itemID = new TezItemID();
            }

            itemID.mID = source.mID;
        }


        public static TezItemID create(int FDID, int MDID = -1)
        {
            TezItemID item_id = null;
            if (sPool.Count > 0)
            {
                item_id = sPool.Dequeue();
            }
            else
            {
                item_id = new TezItemID();
            }

            item_id.mFixedID = FDID;
            item_id.mModifiedID = MDID;
            return item_id;
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
        /// 范围值为[0, int32.Max]
        /// </summary>
        public int fixedID => mFixedID;


        [FieldOffset(4)]
        int mModifiedID = -1;
        /// <summary>
        /// 重定义ID
        /// 用于当物品被修改时用于确定物品是否相同
        /// ID范围值为[int32.Max+1, int64.Max]
        /// </summary>
        public int modifiedID => mModifiedID;

        private TezItemID() { }

        public void close()
        {
            if (mFixedID == -1)
            {
                return;
            }

            mID = -1;
            sPool.Enqueue(this);
        }

        public bool sameAs(TezItemID other)
        {
            if (mFixedID == -1 || other.mFixedID == -1)
            {
                return false;
            }

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