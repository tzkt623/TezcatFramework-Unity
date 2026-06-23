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
    /// Redefine ID
    /// 用于在游戏运行时赋予动态生成的物品
    /// ID范围值为[0, int32.Max]
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
        static Queue<uint> sPoolFreeID = new Queue<uint>();
        static uint sIDGenerator = 1;
        #endregion

        #region Tool
        static List<Queue<uint>> sFreeIDPool = new List<Queue<uint>>();
        static List<uint> sIDPool = new List<uint>();

        static List<string> sTypeList = new List<string>();
        static Dictionary<string, ushort> sTypeDic = new Dictionary<string, ushort>();

        public static string getTypeName(int index)
        {
            return sTypeList[index];
        }

        public static ushort getTypeID(string name)
        {
            if (sTypeDic.TryGetValue(name, out ushort typeID))
            {
                return typeID;
            }

            throw new Exception($"Type {name} not found in type dictionary.");
        }

        public static void loadConfigFile(string path)
        {
            TezSaveController.Reader reader = new TezSaveController.Reader();
            if (reader.beginRead(path))
            {
                if (reader.isObject)
                {
                    foreach (var key in reader.keys)
                    {
                        registerTypeID(key, reader.readInt(key));
                    }
                }
                else
                {
                    throw new Exception($"TezItemID config file {path} is not a valid object.");
                }
            }
            reader.endRead();
        }

        public static void registerTypeID(string typeName, int typeID)
        {
            while (sTypeList.Count <= typeID)
            {
                sTypeList.Add(null);
                sFreeIDPool.Add(null);
                sIDPool.Add(1);
            }

            sTypeList[typeID] = typeName;
            sTypeDic.Add(typeName, (ushort)typeID);
            sFreeIDPool[(ushort)typeID] = new Queue<uint>();
        }
        #endregion

        [FieldOffset(0)]
        ulong mID;
        public ulong ID => mID;

        [FieldOffset(0)]
        uint mRedefineID;

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

        internal void close()
        {
            if (mRedefineID > 0)
            {
                sFreeIDPool[mTypeID].Enqueue(mRedefineID);
                mRedefineID = 0;
            }

            //             if (mRedefineID > 0)
            //             {
            //                 sPoolFreeID.Enqueue(mRedefineID);
            //             }
        }

        internal void generateID()
        {
            if (sFreeIDPool[mTypeID].Count > 0)
            {
                mRedefineID = sFreeIDPool[mTypeID].Dequeue();
            }
            else
            {
                mRedefineID = sIDPool[mTypeID]++;
            }

            //             if(sPoolFreeID.Count > 0)
            //             {
            //                 mRedefineID = sPoolFreeID.Dequeue();
            //             }
            //             else
            //             {
            //                 mRedefineID = sIDGenerator++;
            //             }
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