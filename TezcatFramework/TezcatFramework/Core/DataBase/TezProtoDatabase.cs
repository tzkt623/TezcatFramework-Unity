using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace tezcat.Framework.Core
{
    /*
     * 对象原型
     * 
     * 对每一个需要原型的叶子类对象生成一份原型数据并保存
     * 
     * 例如
     * 父类Weapon类下有叶子类Gun和Axe
     * 会对叶子类Gun和Axe分别生成一份原型数据
     * 但是不会把Gun和Axe生成在父类Weapon类型下
     * 
     * 即,用getProto<Gun>(index or name)来获取原型
     * 而不是用getProto<Weapon>(...)来获取原型
     * 
     */


    /// <para>
    /// 原型ID管理器
    /// 
    /// <para>
    /// 一个Proto
    /// 1.ProtoID
    ///  a.IndexID 人工设定
    ///  b.TypeID 人工设定
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
    /// </para>
    [StructLayout(LayoutKind.Explicit)]
    public class TezProtoIDManager
        : ITezNonCloseable
        , IEquatable<TezProtoIDManager>
    {
        [FieldOffset(0)]
        int mID = -1;
        [FieldOffset(0)]
        ushort mIndexID;
        [FieldOffset(2)]
        ushort mTypeID;

        public int typeID => mTypeID;
        public int indexID => mIndexID;

        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezProtoIDManager)obj);
        }

        public bool Equals(TezProtoIDManager other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mID == other.mID;
        }

        public static bool operator ==(TezProtoIDManager a, TezProtoIDManager b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.mID == b.mID;
        }

        public static bool operator !=(TezProtoIDManager a, TezProtoIDManager b)
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

    public class TezProtoCreator
    {
        public string name { get; }
        public int typeID { get; }
        public int indexID { get; }

        protected ITezProtoObject mProtoObject = null;

        public TezProtoCreator(string name, int typeID, int indexID, ITezProtoObject protoObject)
        {
            mProtoObject = protoObject;
            this.name = name;
            this.typeID = typeID;
            this.indexID = indexID;
        }

        public ITezProtoObject spawnObject()
        {
            return mProtoObject.spawnObject();
        }

        public T spawnObject<T>() where T : ITezProtoObject
        {
            return (T)mProtoObject.spawnObject();
        }
    }

    /// <summary>
    /// 原型数据库
    /// 
    /// 用于读取原型文件
    /// 并创建原型对象用于实时生成
    /// </summary>
    public class TezProtoDatabase
    {
        static class TypeIDGetter<T> where T : ITezProtoObject
        {
            static TypeIDGetter()
            {
                if (ID < 0)
                {
                    ID = TezProtoIDManager.getTypeID(typeof(T).Name);

                    //ID = TezItemID.getTypeID(typeof(T).Name);
                }
            }

            public static readonly int ID = -1;
        }

        class Cell
        {
            public List<TezProtoCreator> list = new List<TezProtoCreator>();
            public Dictionary<string, TezProtoCreator> dict = new Dictionary<string, TezProtoCreator>();

            public void memoryCut()
            {
                list.TrimExcess();
            }
        }

        private List<Cell> mCellList = new List<Cell>();
        protected List<TezProtoCreator> mFixedList = new List<TezProtoCreator>();
        protected Dictionary<string, TezProtoCreator> mFixedDict = new Dictionary<string, TezProtoCreator>();

        public void load(string path)
        {
            TezFileReader reader = new TezJsonReader();
            var files = TezFilePath.getFiles(path, true);
            for (int j = 0; j < files.Length; j++)
            {
                if (reader.load(files[j]))
                {
                    var CID = reader.readString(TezBuildInName.CID);
                    var item = TezcatFramework.classFactory.create<ITezProtoObject>(CID);
                    item.deserialize(reader);
                    int type_id = TezProtoIDManager.getTypeID(CID);

                    reader.beginObject(TezBuildInName.ProtoInfo);
                    var index_id = reader.readInt(TezBuildInName.IID);
                    string proto_name = reader.readString(TezBuildInName.Name);
                    reader.endObject(TezBuildInName.ProtoInfo);

                    this.register(proto_name, type_id, index_id, item);
                }
                else
                {
                    throw new Exception(files[j]);
                }
            }

            reader.close();

            foreach (var item in mCellList)
            {
                item.memoryCut();
            }
        }

        private void register(string protoName, int typeID, int indexID, ITezProtoObject protoObject)
        {
            while (typeID >= mCellList.Count)
            {
                mCellList.Add(new Cell());
            }

            var cell = mCellList[typeID];
            while (indexID >= cell.list.Count)
            {
                cell.list.Add(null);
            }

            if (cell.list[indexID] != null)
            {
                throw new Exception($"This item slot[{cell.list[indexID].name}:{cell.list[indexID].typeID}|{cell.list[indexID].indexID}] has registered! You want[{protoName}:{typeID}|{indexID}]");
            }

            var info = new TezProtoCreator(protoName, typeID, indexID, protoObject);
            cell.list[indexID] = info;
            cell.dict.Add(info.name, info);
        }

        public TezProtoCreator getProto(string nid)
        {
            return mFixedDict[nid];
        }

        public TezProtoCreator getProto(int typeID, int indexID)
        {
            return mCellList[typeID].list[indexID];
        }

        public TezProtoCreator getProto<T>(int indexID) where T : ITezProtoObject
        {
            return mCellList[TypeIDGetter<T>.ID].list[indexID];
        }

        public TezProtoCreator getProto<T>(string name) where T : ITezProtoObject
        {
            return mCellList[TypeIDGetter<T>.ID].dict[name];
        }

        public bool tryGetProto(ushort TID, ushort UID, out TezProtoCreator info)
        {
            var list = mCellList[TID].list;
            if (UID < 0 || UID > list.Count)
            {
                info = null;
                return false;
            }

            info = list[UID];
            return true;
        }

        public bool tryGetProto(string nid, out TezProtoCreator info)
        {
            if (mFixedDict.TryGetValue(nid, out var temp))
            {
                info = temp;
                return true;
            }

            info = null;
            return false;
        }

        public void serialize(TezWriter writer)
        {

        }

        public void deserialize(TezReader reader)
        {

        }
    }
}