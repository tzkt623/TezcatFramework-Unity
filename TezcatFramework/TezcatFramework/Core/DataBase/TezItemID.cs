using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace tezcat.Framework.Core
{
    /// <para>
    /// 每一个对象都有可能成为一个原型
    /// 
    /// 但是不是每一个对象都可以是一个物品
    /// 
    /// 以eve来说
    /// 每一艘舰船都可以是一个市场里的物品
    /// 但是他同时也是一艘舰船
    /// (怀疑拆卸装箱功能就是变为proto信息对象本身)
    /// 所以
    /// 1.存在两种类对象,一种是物品包装对象,一种是舰船实体
    ///   当舰船保存在物品栏中的时候,操作的是物品包装对象
    ///   可以通过让物品对象成为实体对象的wrapper来操作
    ///   这样来说实体本身就应该包含作为物品的元信息
    ///   
    /// 2.只存在一种实体对象,他同时也是物品对象
    /// </para>
    /// 
    /// <para>
    /// 一个Proto
    /// 1.ProtoID
    ///  a.TypeID 用class名称来生成
    ///  b.IndexID 人为赋予
    /// </para>
    /// 
    /// <para>
    /// 一个Item
    /// 1.ItemID
    ///  a.TypeID
    ///  b.IndexID
    /// 2.stackCount
    /// </para>
    /// 
    /// <para>
    /// RuntimeID
    /// 
    /// 
    /// </para>
    [StructLayout(LayoutKind.Explicit)]
    public class TezProtoID
        : ITezNonCloseable
        , IEquatable<TezProtoID>
    {
        [FieldOffset(0)]
        int mID = -1;
        [FieldOffset(0)]
        ushort mTypeID;
        [FieldOffset(2)]
        ushort mIndexID;

        public int typeID => mTypeID;
        public int indexID => mIndexID;

        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezProtoID)obj);
        }

        public bool Equals(TezProtoID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mID == other.mID;
        }

        public static bool operator ==(TezProtoID a, TezProtoID b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.mID == b.mID;
        }

        public static bool operator !=(TezProtoID a, TezProtoID b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return true;
            }

            return a.mID != b.mID;
        }


        #region Tool
        static List<string> sTypeList = new List<string>();
        static Dictionary<string, ushort> sTypeDict = new Dictionary<string, ushort>();

        public static void loadConfigFile(TezReader reader)
        {
            foreach (var key in reader.getKeys())
            {
                registerTypeID(key, reader.readInt(key));
            }
        }

        public static int getTypeID(string name)
        {
            return sTypeDict[name];
        }

        public static void registerTypeID(string typeName, int typeID)
        {
            while (sTypeList.Count <= typeID)
            {
                sTypeList.Add(null);
            }

            sTypeList[typeID] = typeName;
            sTypeDict.Add(typeName, (ushort)typeID);
        }
        #endregion
    }

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
    /// 
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
            result.mTypeID = TID;
            result.mIndexID = UID;
            result.mRTID = sRTIDPool.Count > 0 ? sRTIDPool.Dequeue() : sRTID++;
            return result;
        }

        public static TezItemID create(ushort TID, ushort UID, int RTID)
        {
            TezItemID result = sPool.Count > 0 ? sPool.Dequeue() : new TezItemID();
            result.mTypeID = TID;
            result.mIndexID = UID;
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
        ushort mIndexID = 0;
        /// <summary>
        /// 唯一ID
        /// </summary>
        public ushort IID => mIndexID;

        [FieldOffset(2)]
        ushort mTypeID = 0;
        /// <summary>
        /// 类型ID
        /// </summary>
        public ushort TID => mTypeID;

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

            return mID == other.mID;
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

            return a.mID == b.mID;
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator !=(TezItemID a, TezItemID b)
        {
            return !(a == b);
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