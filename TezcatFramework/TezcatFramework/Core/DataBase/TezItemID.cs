using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 物品ID
    /// 用于判断两个物品是不是同一个
    /// 此类应该是共享数据类
    /// 不需要存在两份相同的数据
    /// 
    /// <para>
    /// 此类由两个ID组成
    /// </para>
    /// 
    /// <para>
    /// Database ID
    /// 存在于数据库中的物品的ID 
    /// 范围值为[0, int32.Max]
    /// </para>
    /// 
    /// <para>
    /// Runtime ID
    /// 用于在游戏运行时赋予动态生成的物品
    /// ID范围值为[int32.Max+1, int64.Max]
    /// </para>
    /// 
    /// <para>
    /// 例如
    /// RPG游戏里面掉落的一个不可堆叠装备
    /// 坚固盔甲的DBID = 2
    /// 
    /// 爆了两个圣神之强化的坚固盔甲
    /// 第一个DBID = 2, RTID = 1
    /// 第二个DBID = 2, RTID = 2
    /// </para>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class TezItemID
        : ITezCloseable
        , IEquatable<TezItemID>
    {
        #region Pool
        static Queue<TezItemID> sPool = new Queue<TezItemID>();
        static Queue<int> sRTIDPool = new Queue<int>();

        static int sRTID = 0;

        public static TezItemID create(uint DBID, int RTID)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mDBID = DBID;
            result.mRTID = RTID;
            return result;
        }

        public static TezItemID create(uint DBID)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mDBID = DBID;
            result.mRTID = sRTIDPool.Count > 0 ? sRTIDPool.Dequeue() : sRTID++;
            return result;
        }

        public static TezItemID create(ushort TID, ushort UID)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mTID = TID;
            result.mUID = UID;
            result.mRTID = sRTIDPool.Count > 0 ? sRTIDPool.Dequeue() : sRTID++;
            return result;
        }

        public static TezItemID create(ushort TID, ushort UID, int RTID)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mTID = TID;
            result.mUID = UID;
            result.mRTID = RTID;
            return result;
        }
        #endregion


        #region Tool
        static List<string> sTypeList = new List<string>();
        static Dictionary<string, ushort> sTypeDic = new Dictionary<string, ushort>();

        public static string getTypeName(int index)
        {
            return sTypeList[index];
        }

        public static ushort getTypeID(string name)
        {
            return sTypeDic[name];
        }

        public static void loadIDFrom(TezReader reader)
        {
            foreach (var key in reader.getKeys())
            {
                registerTypeID(key, reader.readInt(key));
            }
        }

        public static void registerTypeID(string typeName, int typeID)
        {
            while (sTypeList.Count <= typeID)
            {
                sTypeList.Add(null);
            }

            sTypeList[typeID] = typeName;
            sTypeDic.Add(typeName, (ushort)typeID);
        }
        #endregion

        /// <summary>
        /// 是否运行存储
        /// </summary>
        public bool isEmpty => mID == 0;

        [FieldOffset(0)]
        long mID;
        public long ID => mID;

        [FieldOffset(0)]
        uint mDBID = 0;
        /// <summary>
        /// Database ID
        /// 存在于数据库中的物品的ID
        /// 范围值为[0, int32.Max]
        /// </summary>
        public uint DBID => mDBID;

        [FieldOffset(0)]
        ushort mUID = 0;
        /// <summary>
        /// 唯一ID
        /// </summary>
        public ushort UID => mUID;

        [FieldOffset(2)]
        ushort mTID = 0;
        /// <summary>
        /// 类型ID
        /// </summary>
        public ushort TID => mTID;

        [FieldOffset(4)]
        int mRTID = -1;
        /// <summary>
        /// Runtime ID
        /// 用于在游戏运行时赋予动态生成的物品
        /// ID范围值为[int32.Max+1, int64.Max]
        /// 
        /// <para>
        /// 例如
        /// RPG游戏里面掉落的一个装备
        /// 
        /// 坚固盔甲的TID = 2, UID = 1, RTID = -1
        /// 爆了两个圣神之强化的坚固盔甲
        /// 第一个DBID = 2, RTID = 1
        /// 第二个DBID = 2, RTID = 2
        /// </para>
        /// 
        /// </summary>
        public int RTID => mRTID;

        private TezItemID() { }

        public void close()
        {
            if (mRTID > -1)
            {
                sRTIDPool.Enqueue(mRTID);
            }

            mID = 0;
            sPool.Enqueue(this);
        }

        public TezItemID copy()
        {
            return create(mDBID, mRTID);
        }

        public bool sameAs(TezItemID other)
        {
            //             if (mDBID == -1 || other.mDBID == -1)
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