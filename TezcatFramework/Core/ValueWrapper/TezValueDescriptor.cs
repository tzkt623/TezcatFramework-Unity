using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezValueDescriptor
        : IEquatable<ITezValueDescriptor>
        , IComparable<ITezValueDescriptor>
    {
        int ID { get; }
        int typeID { get; }
        int indexID { get; }
        string name { get; }
    }

    /// <summary>
    /// 值系统的描述类
    /// 用于描述这个值的特征信息
    /// 这种描述整个程序里面只应该生成一份用于共享
    /// 
    /// 
    /// 这里的模板参数用于产生多种不同归属的描述信息
    /// </summary>
    public sealed class TezValueDescriptor
        : ITezValueDescriptor
    {
        [StructLayout(LayoutKind.Explicit)]
        struct IDData
        {
            [FieldOffset(0)]
            public int ID;
            [FieldOffset(0)]
            public ushort indexID;
            [FieldOffset(2)]
            public short typeID;
        }

        IDData mData = new IDData();
        public string mName = null;

        public int ID => mData.ID;
        public int typeID => mData.typeID;
        public int indexID => mData.indexID;
        public ushort listIndex { get; }
        public string name => mName;

        TezValueDescriptor(short typeID, ushort indexID, string name, ushort parentCount)
        {
            mData.typeID = typeID;
            mData.indexID = (ushort)(indexID + parentCount);
            mName = name;
            this.listIndex = indexID;
        }

        public bool Equals(ITezValueDescriptor other)
        {
            return mData.ID == other.ID;
        }

        public int CompareTo(ITezValueDescriptor other)
        {
            return mData.ID.CompareTo(other.ID);
        }

        public override int GetHashCode()
        {
            return mData.ID.GetHashCode();
        }

        #region 注册
        class TypeData
        {
            internal ushort parentCount = 0;
            internal short ID = 0;

            internal string Name;
            internal Dictionary<string, ITezValueDescriptor> dict = new Dictionary<string, ITezValueDescriptor>();
            internal List<ITezValueDescriptor> list = new List<ITezValueDescriptor>();
        }

        static List<TypeData> mList = new List<TypeData>();
        static Dictionary<string, TypeData> mDict = new Dictionary<string, TypeData>();

        /// <summary>
        /// 
        /// 生成大类ID
        /// 
        /// 例如：舰船属性, 炮台属性等等
        /// 游戏中有几种不同的单位,就要生成几种不同的大类ID
        /// 
        /// 但是每一个单位都有血量这个属性,所以血量这个属性是共享的
        /// 所以需要做出属性ID继承体系
        ///
        /// 
        /// </summary>
        public static short generateTypeID(string name)
        {
            var cell = new TypeData()
            {
                parentCount = 0,
                ID = (short)mList.Count,
                Name = name
            };

            mList.Add(cell);
            mDict.Add(name, cell);
            return cell.ID;
        }

        public static short generateTypeID(short parentID, string name)
        {
            var cell = new TypeData()
            {
                parentCount = (ushort)mList[parentID].list.Count,
                ID = (short)mList.Count,
                Name = name
            };

            mList.Add(cell);
            mDict.Add(name, cell);
            return cell.ID;
        }

        public static int getTypeCapacity(short id)
        {
            return mList[id].list.Count;
        }

        public static int getTypeCount()
        {
            return mList.Count;
        }

        public static ITezValueDescriptor register(short typeID, string name)
        {
            var cell = mList[typeID];
            if (cell.dict.TryGetValue(name, out var descriptor))
            {
                throw new ArgumentException($"Name Registered {descriptor.name}, Type ID {typeID}");
            }
            else
            {
                descriptor = new TezValueDescriptor(typeID, (ushort)cell.list.Count, name, mList[typeID].parentCount);
                cell.dict.Add(name, descriptor);
                cell.list.Add(descriptor);
            }

            return descriptor;
        }

        public static ITezValueDescriptor get(short typeID, string name)
        {
            var cell = mList[typeID];
            if (!cell.dict.TryGetValue(name, out ITezValueDescriptor descriptor))
            {
                throw new Exception($"This Value[{name}] is not registered!!");
            }

            return descriptor;
        }

        public static ITezValueDescriptor get(short typeID, short indexID)
        {
            return mList[typeID].list[indexID];
        }

        public static ITezValueDescriptor get(string typeName, string name)
        {
            if (mDict.TryGetValue(typeName, out var data))
            {
                if (data.dict.TryGetValue(name, out var descriptor))
                {
                    return descriptor;
                }
            }

            throw new Exception($"This Value[{name}] is not registered!!");
        }

        public static void foreachName(TezEventExtension.Action<string, short> onTypeBegin, TezEventExtension.Action<ITezValueDescriptor> onDescriptor)
        {
            foreach (var cell in mList)
            {
                onTypeBegin(cell.Name, cell.ID);
                foreach (var item in cell.list)
                {
                    onDescriptor(item);
                }
            }
        }
        #endregion
    }
}