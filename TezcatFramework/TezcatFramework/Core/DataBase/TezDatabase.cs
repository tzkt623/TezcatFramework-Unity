using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public class TezProtoCreator
    {
        public string name { get; }
        public int typeID { get; }
        public int indexID { get; }

        ITezProtoObject mProtoObject = null;

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

    /*
     * 原型数据库
     * 
     * 用于读取原型文件
     * 并创建原型对象用于实时生成
     * 
     */
    public class TezProtoDatabase
    {
        static class TypeIDGetter<T> where T : ITezProtoObject
        {
            static TypeIDGetter()
            {
                if (ID < 0)
                {
                    ID = TezProtoID.getTypeID(typeof(T).Name);

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
                    int type_id = TezProtoID.getTypeID(CID);

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

    /// <summary>
    /// 运行时数据库
    /// 用于保存运行时利用模板数据新生成的物品数据
    /// 使数据得以共享
    /// 如果一个数据的索引值为0
    /// 那么它将被删除
    /// </summary>
    [Obsolete("Don`t use this", true)]
    public class TezRunTimeDatabase
    {
        protected List<TezGameItemInfo> mItemList = new List<TezGameItemInfo>();

        public bool registerItem(TezGameItemInfo info)
        {
            var modified_id = info.itemID.RTID;
            while (modified_id >= mItemList.Count)
            {
                mItemList.Add(null);
            }

            if (mItemList[modified_id] != null)
            {
                return false;
            }

            mItemList[modified_id] = info;
            return true;
        }

        public void unregisterItem(int RTID)
        {
            mItemList[RTID] = null;
        }

        public TezGameItemInfo getItem(int RTID)
        {
            var info = mItemList[RTID];
            return info;
        }

        /*
         * 记得保存剩余ID
         */
        /// <summary>
        /// 
        /// </summary>
        public void serialize(TezWriter writer)
        {

        }

        public void deserialize(TezReader reader)
        {

        }

        public bool isModified(int modifiedID)
        {
            return !object.ReferenceEquals(mItemList[modifiedID], null);
        }
    }
}