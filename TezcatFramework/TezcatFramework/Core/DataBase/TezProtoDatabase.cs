using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /*
    ProtoConfig.json(item config)
    {
        "NewUnit":0
    }

    L7.json(item)
    {
        "CID": "NewUnitData",
        "ProtoInfo": {
            "TID": "NewUnit",
            "IID": 6,
            "Name": "L7",
            "StackCount": -1,
            "CopyType": true
        },
        "ObjectData": {
            "Health": 250,
            "Attack": 30,
            "Defense": 30,
            "Damage": 50,
            "Speed": 18,
            "WoundedRate": 80,
            "Spawn": 1
        }
     }

    class NewUnitData : TezProtoObjectData

    class NewUnit : TezProtoObject
     */



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
            public List<TezProtoObjectData> list = new List<TezProtoObjectData>();
            public Dictionary<string, TezProtoObjectData> dict = new Dictionary<string, TezProtoObjectData>();

            public void memoryCut()
            {
                list.TrimExcess();
            }
        }

        private List<Cell> mCellList = new List<Cell>();
        protected Dictionary<string, TezProtoObjectData> mFixedDict = new Dictionary<string, TezProtoObjectData>();

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
                    var item = TezcatFramework.classFactory.create<TezProtoObjectData>(reader.readString(TezBuildInName.CID));
                    item.deserialize(reader);

                    this.register(item);

                    reader.close();
                }
                else
                {
                    throw new Exception(files[j]);
                }
            }


            foreach (var item in mCellList)
            {
                item.memoryCut();
            }
        }

        private void register(TezProtoObjectData protoData)
        {
            var typeID = protoData.itemInfo.itemID.TID;
            var indexID = protoData.itemInfo.itemID.IID;

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
                throw new Exception($"This item slot[{cell.list[indexID].itemInfo.NID}:{cell.list[indexID].itemInfo.itemID.TID}|{cell.list[indexID].itemInfo.itemID.IID}] has registered! You want[{protoData.itemInfo.NID}:{typeID}|{indexID}]");
            }

            cell.list[indexID] = protoData;
            cell.dict.Add(protoData.itemInfo.NID, protoData);
            mFixedDict.Add(protoData.itemInfo.NID, protoData);
        }

        public TezProtoObject createObject(string nid)
        {
            return mFixedDict[nid].createObject();
        }

        public TezProtoObject createObject(int typeID, int indexID)
        {
            return mCellList[typeID].list[indexID].createObject();
        }

        public T createObject<T>(int indexID) where T : TezProtoObject
        {
            return (T)mCellList[TypeIDGetter<T>.ID].list[indexID].createObject();
        }

        public T createObject<T>(string name) where T : TezProtoObject
        {
            return (T)mCellList[TypeIDGetter<T>.ID].dict[name].createObject();
        }

        public bool tryCreateObject(ushort TID, ushort UID, out TezProtoObject protoObject)
        {
            var list = mCellList[TID].list;
            if (UID < 0 || UID > list.Count)
            {
                protoObject = null;
                return false;
            }

            protoObject = list[UID].createObject();
            return true;
        }

        public bool tryCreateObject(string NID, out TezProtoObject protoObject)
        {
            if (mFixedDict.TryGetValue(NID, out var temp))
            {
                protoObject = temp.createObject();
                return true;
            }

            protoObject = null;
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