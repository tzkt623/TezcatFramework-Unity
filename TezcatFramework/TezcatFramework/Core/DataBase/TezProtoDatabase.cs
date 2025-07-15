using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using tezcat.Framework.Game;

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
        #region ID Manager
        List<string> mTypeList = new List<string>();
        Dictionary<string, ushort> mTypeDict = new Dictionary<string, ushort>();

        private void loadConfigFile(string configFilePath)
        {
            TezJsonReader reader = new TezJsonReader();

            if (reader.load(configFilePath))
            {
                foreach (var key in reader.getKeys())
                {
                    registerTypeID(key, reader.readInt(key));
                }
            }

            reader.close();
        }

        public int getTypeID(string name)
        {
            if(mTypeDict.TryGetValue(name, out ushort typeID))
            {
                return typeID;
            }

            return -1;
        }

        private void registerTypeID(string typeName, int typeID)
        {
            while (mTypeList.Count <= typeID)
            {
                mTypeList.Add(null);
            }

            mTypeList[typeID] = typeName;
            mTypeDict.Add(typeName, (ushort)typeID);
        }
        #endregion

        static class TypeIDGetter<T> where T : ITezProtoObject
        {
            static TypeIDGetter()
            {
                if (ID < 0)
                {
                    ID = TezcatFramework.protoDB.getTypeID(typeof(T).Name);

                    //ID = TezItemID.getTypeID(typeof(T).Name);
                }
            }

            public static readonly int ID = -1;
        }

        class Cell
        {
            public List<TezProtoItemInfo> list = new List<TezProtoItemInfo>();
            public Dictionary<string, TezProtoItemInfo> dict = new Dictionary<string, TezProtoItemInfo>();

            public void memoryCut()
            {
                list.TrimExcess();
            }
        }

        private List<Cell> mCellList = new List<Cell>();
        protected Dictionary<string, TezProtoItemInfo> mFixedDict = new Dictionary<string, TezProtoItemInfo>();

        public void load(string configFilePath, string protoFilePath)
        {
            this.loadConfigFile(configFilePath);
            this.loadProtoFile(protoFilePath);
        }

        private void loadProtoFile(string protoFilePath)
        {
            TezSaveController.Reader reader = new TezSaveController.Reader();
            //TezFileReader reader = new TezJsonReader();
            var files = TezFilePath.getFiles(protoFilePath, true);
            for (int j = 0; j < files.Length; j++)
            {
                if (reader.load(files[j]))
                {
                    var item = TezcatFramework.classFactory.create<ITezProtoObject>(reader.readString(TezBuildInName.CID));
                    item.deserialize(reader);

                    this.register(item.itemInfo);
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

        private void register(TezProtoItemInfo itemInfo)
        {
            var typeID = itemInfo.itemID.TID;
            var indexID = itemInfo.itemID.IID;

            while (typeID >= mCellList.Count)
            {
                mCellList.Add(new Cell());
            }

            var cell = mCellList[typeID];
            while (indexID >= cell.list.Count)
            {
                cell.list.Add(null);
            }

            if (!(cell.list[indexID] is null))
            {
                throw new Exception($"This item slot[{cell.list[indexID].NID}:{cell.list[indexID].itemID.TID}|{cell.list[indexID].itemID.IID}] has registered! You want[{itemInfo.NID}:{typeID}|{indexID}]");
            }

            cell.list[indexID] = itemInfo;
            cell.dict.Add(itemInfo.NID, itemInfo);
            mFixedDict.Add(itemInfo.NID, itemInfo);
        }

        public TezProtoItemInfo getProto(string nid)
        {
            return mFixedDict[nid];
        }

        public TezProtoItemInfo getProto(int typeID, int indexID)
        {
            return mCellList[typeID].list[indexID];
        }

        public TezProtoItemInfo getProto<T>(int indexID) where T : ITezProtoObject
        {
            return mCellList[TypeIDGetter<T>.ID].list[indexID];
        }

        public TezProtoItemInfo getProto<T>(string name) where T : ITezProtoObject
        {
            return mCellList[TypeIDGetter<T>.ID].dict[name];
        }

        public bool tryGetProto(ushort TID, ushort UID, out TezProtoItemInfo info)
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

        public bool tryGetProto(string NID, out TezProtoItemInfo info)
        {
            if (mFixedDict.TryGetValue(NID, out var temp))
            {
                info = temp;
                return true;
            }

            info = null;
            return false;
        }

        public void serialize(TezSaveController.Writer writer)
        {

        }

        public void deserialize(TezSaveController.Reader reader)
        {

        }
    }
}