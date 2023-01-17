using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public interface ITezDatabase
    {
#if false
        void registerItem(TezDatabaseGameItem item);
        TezDatabaseGameItem getItem(int uid);
        TezDatabaseGameItem getItem(string nid);
#else
        void registerItem(TezGameObject gameObject);
        TezGameObject getItem(int uid);
        TezGameObject getItem(string nid);
#endif

    }

    public class TezFileDatabase
    {
        protected Dictionary<string, TezReader> mFileData = new Dictionary<string, TezReader>();

        public void loadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("FileData path Error!!!");
            }

            try
            {
                string content = File.ReadAllText(path);
                var json_root = JsonMapper.ToObject(content);
                if (json_root.IsArray)
                {
                    for (int i = 0; i < json_root.Count; i++)
                    {
                        var reader = new TezJsonObjectReader(json_root[i]);
                        mFileData.Add(reader.readString(TezReadOnlyString.NID), reader);
                    }
                }
                else if (json_root.IsObject)
                {
                    var keys = json_root.Keys;
                    foreach (var key in keys)
                    {
                        var reader = new TezJsonObjectReader(json_root[key]);
                        mFileData.Add(key, reader);
                    }
                }

                json_root.Clear();
            }
            catch (Exception e)
            {
                //e.Message;
            }
        }

        /// <summary>
        /// 尝试取得文件数据
        /// </summary>
        public bool tryGetItemData(string name, out TezReader data)
        {
            return mFileData.TryGetValue(name, out data);
        }

        /// <summary>
        /// 取得文件数据
        /// </summary>
        public TezReader getItemData(string name)
        {
            return mFileData[name];
        }
    }

    /// <summary>
    /// 独立主数据库
    /// 
    /// 数据库Json文件应该是一个Array的外层
    /// 物品的DBID来自于数据库文件
    /// [
    ///     {
    ///         "CID": "xxx"        =>类类型
    ///         "NID": "xxxxxx"     =>物品名称
    ///         "fixedID": 0           =>物品数据库ID
    ///         "MID": -1           =>重定义ID
    ///         "CTG": "xx"         =>物品类型
    ///     
    ///     }
    /// ]
    /// 
    /// 
    /// </summary>
    public class TezMainDatabase
    {
#if false
        protected List<TezDatabaseGameItem> mItemList = new List<TezDatabaseGameItem>();
        protected Dictionary<string, TezDatabaseGameItem> mItemDict = new Dictionary<string, TezDatabaseGameItem>();

        public void registerItem(TezDatabaseGameItem item)
        {
            if (mItemDict.ContainsKey(item.NID))
            {
                throw new Exception(string.Format("{0} : Item {1} had registered!!", this.GetType().Name, item.NID));
            }

            item.onRegister(mItemList.Count);
            mItemList.Add(item);
            mItemDict.Add(item.NID, item);
        }

        public TezDatabaseGameItem getItem(int uid)
        {
            return mItemList[uid];
        }

        public TezDatabaseGameItem getItem(string nid)
        {
            return mItemDict[nid];
        }
#else
        public TezMainDatabase()
        {
            mFixedList.Add(TezcatFramework.emptyItemInfo);
            mFixedDict.Add(TezcatFramework.emptyItemInfo.NID, TezcatFramework.emptyItemInfo);
        }



        #region ItemData
        protected List<TezBaseItemInfo> mFixedList = new List<TezBaseItemInfo>();
        protected Dictionary<string, TezBaseItemInfo> mFixedDict = new Dictionary<string, TezBaseItemInfo>();

        public void registerItem(TezItemableObject gameObject)
        {
            //数据库信息应该由数据库文件定义
            //这里只需要读取保存好的文件然后放到指定的位置中即可
            //也可以在这里做更复杂的分类
            var info = (TezItemInfo)gameObject.itemInfo;
            var item_id = info.itemID;

            while (item_id.fixedID >= mFixedList.Count)
            {
                mFixedList.Add(null);
            }

            if (mFixedList[item_id.fixedID] != null)
            {
                throw new Exception($"This item slot has registered! [{info.NID}: {item_id.fixedID}]");
            }

            mFixedList[item_id.fixedID] = info;
            mFixedDict.Add(info.NID, info);

            //             var info = gameObject.itemInfo;
            //             if (mFixedDict.ContainsKey(info.NID))
            //             {
            //                 throw new Exception($"{this.GetType().Name} : Item {info.NID} had registered!!");
            //             }
            // 
            //             gameObject.onDBRegister(mFixedList.Count);
            //             mFixedList.Add(info);
            //             mFixedDict.Add(info.NID, info);
        }

        public TezItemInfo getItem(int dbid)
        {
            return (TezItemInfo)mFixedList[dbid];
        }

        public bool tryGetItem(int dbid, out TezItemInfo info)
        {
            if (dbid < 0 || dbid > mFixedList.Count)
            {
                info = (TezItemInfo)mFixedList[0];
                return false;
            }

            info = (TezItemInfo)mFixedList[dbid];
            return true;
        }

        public TezItemInfo getItem(string nid)
        {
            return (TezItemInfo)mFixedDict[nid];
        }

        public bool tryGetItem(string nid, out TezItemInfo info)
        {
            if (mFixedDict.TryGetValue(nid, out var temp))
            {
                info = (TezItemInfo)temp;
                return true;
            }

            info = (TezItemInfo)mFixedList[0];
            return false;
        }

        public void serialize(TezWriter writer)
        {

        }

        public void deserialize(TezReader reader)
        {

        }
        #endregion

#endif
    }

    /// <summary>
    /// 运行时数据库
    /// 用于保存运行时利用模板数据新生成的物品数据
    /// 使数据得以共享
    /// 如果一个数据的索引值为0
    /// 那么它将被删除
    /// </summary>
    public class TezRunTimeDatabase
    {
#if false
        protected List<TezDatabaseGameItem> mItemList = new List<TezDatabaseGameItem>();
        private Queue<int> mFreeIndex = new Queue<int>();

        public int generateID()
        {
            int index;
            if (mFreeIndex.Count > 0)
            {
                index = mFreeIndex.Dequeue();
            }
            else
            {
                index = mItemList.Count;
                mItemList.Add(null);
            }

            return index;
        }

        public bool registerItem(TezItemID itemID, TezDatabaseGameItem gameItem)
        {
            var modified_id = itemID.ModifiedID;
            if (mItemList[modified_id] != null)
            {
                return false;
            }

            gameItem.onRuntimeRegister(itemID.DBID, modified_id);
            mItemList[modified_id].retainModifiedItem();
            mItemList[modified_id] = gameItem;
            return true;
        }

        public bool unregisterItem(int modifiedID)
        {
            if (mItemList[modifiedID].releaseModifiedItem())
            {
                mFreeIndex.Enqueue(modifiedID);
                mItemList[modifiedID].close();
                mItemList[modifiedID] = null;
                return true;
            }

            return false;
        }

        public TezDatabaseGameItem getItem(int modifiedID)
        {
            var item = mItemList[modifiedID];
            item.retainModifiedItem();
            return item;
        }

        public void serialize(TezWriter writer)
        {

        }

        public void deserialize(TezReader reader)
        {

        }

        public void close()
        {

        }

        public bool isModified(int modifiedID)
        {
            return mItemList[modifiedID] != null;
        }
#else
        protected List<TezMItemInfo> mItemList = new List<TezMItemInfo>();
        private Queue<int> mFreeIndex = new Queue<int>();

        public int generateID()
        {
            int index;
            if (mFreeIndex.Count > 0)
            {
                index = mFreeIndex.Dequeue();
            }
            else
            {
                index = mItemList.Count;
                mItemList.Add(null);
            }

            return index;
        }

        public bool registerItem(TezMItemInfo info)
        {
            var modified_id = info.itemID.modifiedID;
            if (mItemList[modified_id] != null)
            {
                return false;
            }

            mItemList[modified_id] = info;
            return true;
        }

        public void unregisterItem(int modifiedID)
        {
            mFreeIndex.Enqueue(modifiedID);
            mItemList[modifiedID] = null;
        }

        public TezMItemInfo getItem(int modifiedID)
        {
            var info = mItemList[modifiedID];
            return info;
        }

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
#endif
    }

    public class TezMultiDatabase : ITezNonCloseable
    {
        List<TezCellDatabase> mDatabaseList = new List<TezCellDatabase>();

        public void register(TezCellDatabase db)
        {
            while (mDatabaseList.Count <= db.UID)
            {
                mDatabaseList.Add(null);
            }

            if (mDatabaseList[db.UID] != null)
            {
                throw new Exception("This Database ID already existed");
            }

            mDatabaseList[db.UID] = db;
        }

        public TezCellDatabase get(int id)
        {
            return mDatabaseList[id];
        }

        public TezDatabaseGameItem getItem(int itemID)
        {
            return mDatabaseList[TezDBIDGenerator.getCellID(itemID)].getItem(TezDBIDGenerator.getTypeID(itemID));
        }
    }

    /// <summary>
    /// 细胞数据库
    /// </summary>
    public class TezCellDatabase
    {
        protected List<TezDatabaseGameItem> mItemList = new List<TezDatabaseGameItem>();
        protected Dictionary<string, TezDatabaseGameItem> mItemDict = new Dictionary<string, TezDatabaseGameItem>();

        public int UID { get; } = 0;

        /// <summary>
        /// 多数据库模式
        /// 可以使用TezDatabaseManager来获得所有分体式数据库
        /// </summary>
        public TezCellDatabase(int uid)
        {
            this.UID = uid;
            TezcatFramework.multiDB.register(this);
        }

        public void registerItem(TezDatabaseGameItem item)
        {
            if (mItemDict.ContainsKey(item.NID))
            {
                throw new Exception($"{this.GetType().Name} : Item {item.NID} had registered!!");
            }

            item.onRegister(TezDBIDGenerator.generateID(this.UID, mItemList.Count));
            mItemList.Add(item);
            mItemDict.Add(item.NID, item);
        }

        public TezDatabaseGameItem getItem(int uid)
        {
            return mItemList[uid];
        }

        public TezDatabaseGameItem getItem(string nid)
        {
            return mItemDict[nid];
        }
    }

}