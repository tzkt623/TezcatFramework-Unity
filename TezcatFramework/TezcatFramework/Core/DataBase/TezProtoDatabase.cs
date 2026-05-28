using System;
using System.Collections.Generic;
using tezcat.Framework.ArchetypeECS;

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
//         List<string> mTypeList = new List<string>();
//         Dictionary<string, ushort> mTypeDict = new Dictionary<string, ushort>();
//         public IReadOnlyDictionary<string, ushort> typeIDDict => mTypeDict;
//         public IReadOnlyList<string> typeIDList => mTypeList;

        public void loadConfigFile(string configFilePath)
        {
            TezItemID.loadConfigFile(configFilePath);

//             TezJsonReader reader = new TezJsonReader();
// 
//             if (reader.load(configFilePath))
//             {
//                 foreach (var key in reader.getKeys())
//                 {
//                     registerTypeID(key, reader.readInt(key));
//                 }
//             }
// 
//             reader.close();
        }

        public int getTypeID(string name)
        {
            return TezItemID.getTypeID(name);

//             if (mTypeDict.TryGetValue(name, out ushort typeID))
//             {
//                 return typeID;
//             }
// 
//             return -1;
        }

        private void registerTypeID(string typeName, int typeID)
        {
//             while (mTypeList.Count <= typeID)
//             {
//                 mTypeList.Add(null);
//             }
// 
//             mTypeList[typeID] = typeName;
//             mTypeDict.Add(typeName, (ushort)typeID);
        }
        #endregion

        static class TypeIDGetter<T> where T : TezProtoObjectData
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

        public event Action<string> evtDebugLoad;


        public void debug(Action<TezProtoObjectData> action, Action typeBegin, Action typeEnd)
        {
            foreach (var item in mCellList)
            {
                typeBegin();
                foreach (var protoData in item.list)
                {
                    action(protoData);
                }
                typeEnd();
            }
        }

        public void loadProtoFile(string protoFilePath)
        {
            TezSaveController.Reader reader = new TezSaveController.Reader();

            var files = TezFilePath.getFiles(protoFilePath, true);
            for (int j = 0; j < files.Length; j++)
            {
                if (reader.beginRead(files[j]))
                {
                    if(reader.isObject)
                    {
                        var item = TezcatFramework.classFactory.create<ITezProtoLoader>(reader.readString(TezBuildInName.CID));
                        item.loadProtoData(reader);
                        this.register((TezProtoObjectData)item);
                    }
                    
                    if(reader.isArray)
                    {
                        for (int i = 0; i < reader.count; i++)
                        {
                            reader.enterObject(i);
                            var item = TezcatFramework.classFactory.create<ITezProtoLoader>(reader.readString(TezBuildInName.CID));
                            item.loadProtoData(reader);
                            this.register((TezProtoObjectData)item);
                            reader.exitObject(i);
                        }
                    }
                }
                else
                {
                    throw new Exception(files[j]);
                }
                reader.endRead();
            }

            reader.close();

            foreach (var item in mCellList)
            {
                item.memoryCut();
            }
        }

        private void register(TezProtoObjectData protoData)
        {
            var typeID = protoData.protoInfo.itemID.TID;
            var indexID = protoData.protoInfo.itemID.IID;

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
                throw new Exception($"This item slot[{cell.list[indexID].protoInfo.NID}:{cell.list[indexID].protoInfo.itemID.TID}|{cell.list[indexID].protoInfo.itemID.IID}] has registered! You want[{protoData.protoInfo.NID}:{typeID}|{indexID}]");
            }

            cell.list[indexID] = protoData;
            cell.dict.Add(protoData.protoInfo.NID, protoData);
            mFixedDict.Add(protoData.protoInfo.NID, protoData);
        }

        #region Data
        public TezProtoObjectData createObjectData(string nid)
        {
            return mFixedDict[nid].copyOrShare();
        }

        public TezProtoObjectData createObjectData(int typeID, int indexID)
        {
            return mCellList[typeID].list[indexID].copyOrShare();
        }

        public TezProtoObjectData createObjectData(int typeID, string name)
        {
            return mCellList[typeID].dict[name].copyOrShare();
        }

        public TezProtoObjectData createObjectData(string CID, string name)
        {
            return mCellList[getTypeID(CID)].dict[name].copyOrShare();
        }

        public ProtoData createObjectData<ProtoData>(int indexID) where ProtoData : TezProtoObjectData
        {
            return (ProtoData)mCellList[TypeIDGetter<ProtoData>.ID].list[indexID].copyOrShare();
        }

        public ProtoData createObjectData<ProtoData>(string name) where ProtoData : TezProtoObjectData
        {
            return (ProtoData)mCellList[TypeIDGetter<ProtoData>.ID].dict[name].copyOrShare();
        }

        public bool tryCreateObjectData(ushort TID, ushort UID, out TezProtoObjectData protoObject)
        {
            var list = mCellList[TID].list;
            if (UID < 0 || UID > list.Count)
            {
                protoObject = null;
                return false;
            }

            protoObject = list[UID].copyOrShare();
            return true;
        }

        public bool tryCreateObjectData(string NID, out TezProtoObjectData protoObject)
        {
            if (mFixedDict.TryGetValue(NID, out var temp))
            {
                protoObject = temp.copyOrShare();
                return true;
            }

            protoObject = null;
            return false;
        }
        #endregion


        #region Entity

        public TezWorld.Entity createEntity<ProtoData>(string name, int whichClass = 0)
            where ProtoData : TezProtoObjectData
        {
            int id = TypeIDGetter<ProtoData>.ID;
            return mCellList[id].dict[name].createEntity();
        }

        public TezWorld.Entity createEntity<ProtoData>(int index, int whichClass = 0)
            where ProtoData : TezProtoObjectData
        {
            int id = TypeIDGetter<ProtoData>.ID;
            return mCellList[id].list[index].createEntity();
        }

        public TezWorld.Entity createEntity<ProtoData>(string CID, string name, int whichClass = 0)
            where ProtoData : TezProtoObjectData
        {
            var id = getTypeID(CID);
            return mCellList[id].dict[name].createEntity();
        }

        #endregion

        /*
        #region Object
        public TezProtoObject createObject(string nid, int whichClass = 0)
        {
            return mFixedDict[nid].createObjectByCopyMe(whichClass);
        }

        public TezProtoObject createObject(int typeID, int indexID, int whichClass = 0)
        {
            return mCellList[typeID].list[indexID].createObjectByCopyMe(whichClass);
        }

        public TezProtoObject createObject(int typeID, string name, int whichClass = 0)
        {
            return mCellList[typeID].dict[name].createObjectByCopyMe(whichClass);
        }

        public TezProtoObject createObject(string CID, string name, int whichClass = 0)
        {
            return mCellList[getTypeID(CID)].dict[name].createObjectByCopyMe(whichClass);
        }

        public ProtoObject createObject<ProtoData, ProtoObject>(int indexID, int whichClass = 0)
            where ProtoData : TezProtoObjectData
            where ProtoObject : TezProtoObject
        {
            return (ProtoObject)mCellList[TypeIDGetter<ProtoData>.ID].list[indexID].createObjectByCopyMe(whichClass);
        }

        public ProtoObject createObject<ProtoData, ProtoObject>(string name, int whichClass = 0)
            where ProtoData : TezProtoObjectData
            where ProtoObject : TezProtoObject
        {
            return (ProtoObject)mCellList[TypeIDGetter<ProtoData>.ID].dict[name].createObjectByCopyMe(whichClass);
        }

        public bool tryCreateObject(ushort TID, ushort UID, out TezProtoObject protoObject, int whichClass = 0)
        {
            var list = mCellList[TID].list;
            if (UID < 0 || UID > list.Count)
            {
                protoObject = null;
                return false;
            }

            protoObject = list[UID].createObjectByCopyMe(whichClass);
            return true;
        }

        public bool tryCreateObject(string NID, out TezProtoObject protoObject, int whichClass = 0)
        {
            if (mFixedDict.TryGetValue(NID, out var temp))
            {
                protoObject = temp.createObjectByCopyMe(whichClass);
                return true;
            }

            protoObject = null;
            return false;
        }
        #endregion
        */

        public void serialize(TezSaveController.Writer writer)
        {

        }

        public void deserialize(TezSaveController.Reader reader)
        {

        }
    }
}