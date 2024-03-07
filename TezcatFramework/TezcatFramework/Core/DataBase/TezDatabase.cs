using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /*
     * 文件数据库
     * 
     * 数据库应该给每一个Item一个对应的ID,用于区分不同的Item
     * ID分为两部分
     * 
     * 高位代表FixedID
     * FixedID表示此物品来源于设定数据库
     * 
     * 地位代表ModifiedID
     * ModifiedID表示此物品来源于运行时依赖设定数据库进行修改后的物品
     * 
     */
    public class TezItemDatabase
    {
        sealed class GetTypeID<T> where T : ITezItemObject
        {
            static GetTypeID()
            {
                if (sTID < 0)
                {
                    sTID = TezItemID.getTypeID(typeof(T).Name);
                }
            }

            static int sTID = -1;
            public static int TID => sTID;
        }

        class Cell
        {
            public List<TezGameItemInfo> list = new List<TezGameItemInfo>();
            public Dictionary<string, TezGameItemInfo> dict = new Dictionary<string, TezGameItemInfo>();

            public void memoryCut()
            {
                list.TrimExcess();
            }
        }

        protected List<TezGameItemInfo> mFixedList = new List<TezGameItemInfo>();

        private List<Cell> mCellList = new List<Cell>();

        protected Dictionary<string, TezGameItemInfo> mFixedDict = new Dictionary<string, TezGameItemInfo>();

        public void load(string path)
        {
            TezFileReader reader = new TezJsonReader();
            var files = TezFilePath.getFiles(path, true);
            for (int j = 0; j < files.Length; j++)
            {
                if (reader.load(files[j]))
                {
                    var CID = reader.readString(TezBuildInName.ClassID);
                    var item = TezcatFramework.classFactory.create<TezItemObject>(CID);
                    item.deserialize(reader);
                    item.itemInfo.setPrototype(item);
                    this.registerItem(item);
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

        public void registerItem(TezItemObject gameObject)
        {
            //数据库信息应该由数据库文件定义
            //这里只需要读取保存好的文件然后放到指定的位置中即可
            //也可以在这里做更复杂的分类
            var info = gameObject.itemInfo;
            var item_id = info.itemID;

            while (item_id.TID >= mCellList.Count)
            {
                mCellList.Add(new Cell());
            }

            var cell = mCellList[item_id.TID];
            while (item_id.UID >= cell.list.Count)
            {
                cell.list.Add(null);
            }

            if (cell.list[item_id.UID] != null)
            {
                throw new Exception($"This item slot[{cell.list[item_id.UID].NID}] has registered! [{info.NID}: {item_id.UID}]");
            }

            cell.list[item_id.UID] = info;
            cell.dict.Add(info.NID, info);

            //mFixedDict.Add(info.NID, info);
        }

        public TezGameItemInfo getItem(int TID, int UID)
        {
            return mCellList[TID].list[UID];
        }

        public TezGameItemInfo getItem<T>(int UID) where T : ITezItemObject
        {
            return mCellList[GetTypeID<T>.TID].list[UID];
        }

        public TezGameItemInfo getItem<T>(string NID) where T : ITezItemObject
        {
            return mCellList[GetTypeID<T>.TID].dict[NID];
        }

        public bool tryGetItem(ushort TID, ushort UID, out TezGameItemInfo info)
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

        public TezGameItemInfo getItem(string nid)
        {
            return mFixedDict[nid];
        }

        public bool tryGetItem(string nid, out TezGameItemInfo info)
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
    public class TezRunTimeDatabase
    {
        protected List<TezGameItemInfo> mItemList = new List<TezGameItemInfo>();

        public bool registerItem(TezGameItemInfo info)
        {
            var modified_id = info.itemID.RTID;
            while(modified_id >= mItemList.Count)
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