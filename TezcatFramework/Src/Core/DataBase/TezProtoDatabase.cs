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
        public void loadConfigFile(string configFilePath)
        {
            TezItemID.loadConfigFile(configFilePath);
        }

        public int getTypeID(string name)
        {
            return TezItemID.getTypeID(name);
        }
        #endregion

        static class TypeIDGetter<T> where T : TezProtoObjectData
        {
            static TypeIDGetter()
            {
                if (ID < 0)
                {
                    ID = TezcatFramework.protoDB.getTypeID(typeof(T).Name);
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
        public TezProtoObjectData createObjectData(string nid, TezProtoObjectCreateMode mode)
        {
            return mFixedDict[nid].createDataWhitMe(mode);
        }

        public TezProtoObjectData createObjectData(int typeID, int indexID, TezProtoObjectCreateMode mode)
        {
            return mCellList[typeID].list[indexID].createDataWhitMe(mode);
        }

        public TezProtoObjectData createObjectData(int typeID, string name, TezProtoObjectCreateMode mode)
        {
            return mCellList[typeID].dict[name].createDataWhitMe(mode);
        }

        public TezProtoObjectData createObjectData(string CID, string name, TezProtoObjectCreateMode mode)
        {
            return mCellList[getTypeID(CID)].dict[name].createDataWhitMe(mode);
        }

        public ProtoData createObjectData<ProtoData>(int indexID, TezProtoObjectCreateMode mode) where ProtoData : TezProtoObjectData
        {
            return (ProtoData)mCellList[TypeIDGetter<ProtoData>.ID].list[indexID].createDataWhitMe(mode);
        }

        public ProtoData createObjectData<ProtoData>(string name, TezProtoObjectCreateMode mode) where ProtoData : TezProtoObjectData
        {
            return (ProtoData)mCellList[TypeIDGetter<ProtoData>.ID].dict[name].createDataWhitMe(mode);
        }

        public bool tryCreateObjectData(ushort TID, ushort UID, TezProtoObjectCreateMode mode, out TezProtoObjectData protoObject)
        {
            var list = mCellList[TID].list;
            if (UID < 0 || UID > list.Count)
            {
                protoObject = null;
                return false;
            }

            protoObject = list[UID].createDataWhitMe(mode);
            return true;
        }

        public bool tryCreateObjectData(string NID, TezProtoObjectCreateMode mode, out TezProtoObjectData protoObject)
        {
            if (mFixedDict.TryGetValue(NID, out var temp))
            {
                protoObject = temp.createDataWhitMe(mode);
                return true;
            }

            protoObject = null;
            return false;
        }
        #endregion


        #region Entity

//         public TezWorld.Entity createEntity<ProtoData>(string name, TezProtoObjectCreateMode mode)
//             where ProtoData : TezEntityProtoObjectData
//         {
//             int id = TypeIDGetter<ProtoData>.ID;
//             return mCellList[id].dict[name].createEntity(mode);
//         }
// 
//         public TezWorld.Entity createEntity<ProtoData>(int index, TezProtoObjectCreateMode mode)
//             where ProtoData : TezEntityProtoObjectData
//         {
//             int id = TypeIDGetter<ProtoData>.ID;
//             return mCellList[id].list[index].createEntity(mode);
//         }
// 
//         public TezWorld.Entity createEntity<ProtoData>(string CID, string name, TezProtoObjectCreateMode mode)
//             where ProtoData : TezEntityProtoObjectData
//         {
//             var id = getTypeID(CID);
//             return mCellList[id].dict[name].createEntity(mode);
//         }

        #endregion


        public void serialize(TezSaveController.Writer writer)
        {

        }

        public void deserialize(TezSaveController.Reader reader)
        {

        }
    }
}