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

    public class TezFixedDatabase
    {
        protected List<TezGameItemInfo> mFixedList = new List<TezGameItemInfo>();
        protected Dictionary<string, TezGameItemInfo> mFixedDict = new Dictionary<string, TezGameItemInfo>();


        public void load(string path)
        {
            TezFileReader reader = new TezJsonReader();
            var files = TezFilePath.getFiles(path, true);
            for (int j = 0; j < files.Length; j++)
            {
                if (reader.load(files[j]))
                {
                    var CID = reader.readString(TezReadOnlyString.ClassID);
                    var item = TezcatFramework.classFactory.create<TezItemableObject>(CID);
                    item.deserialize(reader);
                    this.registerItem(item);
                }
                else
                {
                    throw new Exception(files[j]);
                }
            }

            reader.close();
        }

        public void registerItem(TezItemableObject gameObject)
        {
            //数据库信息应该由数据库文件定义
            //这里只需要读取保存好的文件然后放到指定的位置中即可
            //也可以在这里做更复杂的分类
            var info = (TezGameItemInfo)gameObject.itemInfo;
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
        }

        public TezGameItemInfo getItem(int fixedID)
        {
            return mFixedList[fixedID];
        }

        public bool tryGetItem(int fixedID, out TezGameItemInfo info)
        {
            if (fixedID < 0 || fixedID > mFixedList.Count)
            {
                info = null;
                return false;
            }

            info = mFixedList[fixedID];
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

            info = mFixedList[0];
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

        public bool registerItem(TezGameItemInfo info)
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

        public TezGameItemInfo getItem(int modifiedID)
        {
            var info = mItemList[modifiedID];
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