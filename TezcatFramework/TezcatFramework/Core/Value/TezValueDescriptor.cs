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
    /// <para></para>
    /// 
    /// 这里的模板参数用于产生多种不同归属的描述信息
    /// </summary>
    public sealed class TezValueDescriptor
        : ITezValueDescriptor
    {
        [StructLayout(LayoutKind.Explicit)]
        struct Data
        {
            [FieldOffset(0)]
            public int ID;
            [FieldOffset(0)]
            public short indexID;
            [FieldOffset(2)]
            public short typeID;
        }

        Data mData = new Data();
        public string mName = null;

        public int ID => mData.ID;
        public int typeID => mData.typeID;
        public int indexID => mData.indexID;
        public string name => mName;

        TezValueDescriptor(short typeID, short indexID, string name)
        {
            mData.typeID = typeID;
            mData.indexID = indexID;
            mName = name;
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
            public short ID = 0;
            public string Name;
            public Dictionary<string, ITezValueDescriptor> dict = new Dictionary<string, ITezValueDescriptor>();
            public List<ITezValueDescriptor> list = new List<ITezValueDescriptor>();
        }

        static List<TypeData> mList = new List<TypeData>();
        static Dictionary<string, TypeData> mDict = new Dictionary<string, TypeData>();

        public static short generateTypeID(string name)
        {
            var cell = new TypeData()
            {
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

        public static ITezValueDescriptor register(short typeID, string name)
        {
            //             while (mList.Count <= typeID)
            //             {
            //                 mList.Add(new Cell());
            //             }

            var cell = mList[typeID];
            cell.ID = typeID;
            if (cell.dict.TryGetValue(name, out var descriptor))
            {
                throw new ArgumentException(descriptor.name);
            }
            else
            {
                descriptor = new TezValueDescriptor(typeID, (short)cell.list.Count, name);
                cell.dict.Add(name, descriptor);
                cell.list.Add(descriptor);
            }

            return descriptor;
        }

        public static ITezValueDescriptor get(short typeID, string name)
        {
            var cell = mList[typeID];

            ITezValueDescriptor descriptor;
            if (!cell.dict.TryGetValue(name, out descriptor))
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