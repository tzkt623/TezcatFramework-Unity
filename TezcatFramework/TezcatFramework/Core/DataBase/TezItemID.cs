using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 
    /// 
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
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct TezItemID
        : IEquatable<TezItemID>
    {
        #region Pool
//        static Queue<TezItemID> sPool = new Queue<TezItemID>();
        static Queue<uint> sPoolFreeRedefineID = new Queue<uint>();

        static uint sRedefineIDGenerator = 1;

//         public static TezItemID create(uint DBID, int RTID)
//         {
//             TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
//             result.mDBID = DBID;
//             result.mRedefineID = RTID;
//             return result;
//         }
// 
//         public static TezItemID create(uint DBID)
//         {
//             TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
//             result.mDBID = DBID;
//             result.mRedefineID = sRTIDPool.Count > 0 ? sRTIDPool.Dequeue() : sRTID++;
//             return result;
//         }
// 
//         public static TezItemID create(ushort TID, ushort UID)
//         {
//             TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
//             result.mTypeID = TID;
//             result.mIndexID = UID;
//             result.mRedefineID = sRTIDPool.Count > 0 ? sRTIDPool.Dequeue() : sRTID++;
//             return result;
//         }
// 
//         public static TezItemID create(ushort TID, ushort UID, int RTID)
//         {
//             TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
//             result.mTypeID = TID;
//             result.mIndexID = UID;
//             result.mRedefineID = RTID;
//             return result;
//         }
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

        public static void loadConfigFile(TezReader reader)
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

        [FieldOffset(0)]
        ulong mID;
        public ulong ID => mID;

        [FieldOffset(0)]
        uint mRedefineID;
        /// <summary>
        /// 
        /// Runtime ID
        /// 用于在游戏运行时赋予动态生成的物品
        /// 也就是并没有记录在内建数据库中的物品ID
        /// 
        /// 此ID用于标记在运行时数据库存在的物品数据
        /// </summary>
        /// 
        /// <remarks>
        /// 例如
        /// RPG游戏里面的一个模版装备
        /// 坚固盔甲的TID = 2, IID = 1, RTID = -1
        /// 
        /// 爆了两个圣神之强化的坚固盔甲
        /// 第一个DBID = 2, RTID = 1
        /// 第二个DBID = 2, RTID = 2
        /// 
        /// </remarks>
        /// 
        
        ///重定义ID,用于生成新物品
        public uint RedefineID => mRedefineID;

        [FieldOffset(4)]
        uint mDBID;
        /// <summary>
        /// Database ID
        /// 存在于数据库中的物品的ID
        /// </summary>
        public uint DBID => mDBID;

        [FieldOffset(4)]
        ushort mIndexID;
        /// <summary>
        /// 索引ID
        /// </summary>
        public ushort IID => mIndexID;

        [FieldOffset(6)]
        ushort mTypeID;
        /// <summary>
        /// 类型ID
        /// 由类的Class类型决定
        /// </summary>
        public ushort TID => mTypeID;

        /// <summary>
        /// 是否运行存储
        /// </summary>
        public bool isEmpty => mID == 0;

        public void setID(ulong id)
        {
            mID = id;
        }

        public void setDBID(ushort type, ushort index)
        {
            mTypeID = type;
            mIndexID = index;
        }

        //void ITezCloseable.closeThis()
        //{
        //    if (mRTID > -1)
        //    {
        //        sRTIDPool.Enqueue(mRTID);
        //    }
        //
        //    mID = 0;
        //    sPool.Enqueue(this);
        //}

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
            return mID == other.mID;
        }

//         internal void setRedefineID(int v)
//         {
//             mRedefineID = v;
//         }

        internal void close()
        {
            if (mRedefineID > 0)
            {
                sPoolFreeRedefineID.Enqueue(mRedefineID);
            }
        }

        internal void generateRedefineID()
        {
            if(sPoolFreeRedefineID.Count > 0)
            {
                mRedefineID = sPoolFreeRedefineID.Dequeue();
            }
            else
            {
                mRedefineID = sRedefineIDGenerator++;
            }
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator ==(TezItemID a, TezItemID b)
        {
            return a.mID == b.mID;
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator !=(TezItemID a, TezItemID b)
        {
            return a.mID != b.mID;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TezPrototypeRegisterAttribute : Attribute
    {
        public string name;
        public int index;

        public TezPrototypeRegisterAttribute(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
    }
}