using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace tezcat.Framework.Database
{
    /*
     * #数据库操作流程
     * 
     * 从本地文件中读取所有物品文件数据添加到FileDatabase中
     * =>读取过程中,如果读取到只读对象物品,就初始化一个对象并添加到共享物品数
     * =>读取过程中,如果读取到唯一对象物品,就只记录信息并不初始化对象到缓存中
     * ==>
     * 
     * 
     * 数据库格式
     * 
     * 多个数组模式
     * array:
     * [
     *  {
     *    "CID": "xxx",
     *    "NID": "xxx",
     *    "FDID: 1,
     *    "MDID": 1
     *  }
     * ]
     * 
     * 单个文件模式
     * object:
     * {
     *   "CID": "xxx",
     *   "NID": "xxx"
     *   "FDID: 1,
     *   "MDID": 1
     * }
     * 
     */
    public class TezFileDatabase
    {
        protected Dictionary<string, TezBaseItemInfo> mFileData = new Dictionary<string, TezBaseItemInfo>();
        protected List<TezBaseItemInfo> mFixedList = new List<TezBaseItemInfo>();
        protected List<TezBaseItemInfo> mModifiedList = new List<TezBaseItemInfo>();
        private Queue<int> mFreeIndex = new Queue<int>();


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

                switch (json_root.GetJsonType())
                {
                    case JsonType.Array:
                        {
                            for (int i = 0; i < json_root.Count; i++)
                            {
                                var item = json_root[i];
                                if (item.IsObject)
                                {
                                    this.add(item, path);
                                }
                                else
                                {
                                    throw new Exception($"FileData must a JsonObject");
                                }
                            }

                            //关掉数组头
                            json_root.Clear();
                        }
                        break;
                    case JsonType.Object:
                        {
                            this.add(json_root, path);
                        }
                        break;
                    default:
                        throw new Exception($"TezFileDatabase=> Error FileData Type [{path}]");
                }
            }
            catch (Exception e)
            {

            }
        }

        private void add(JsonData jsonData, string path)
        {
            var reader = new TezJsonObjectReader(jsonData);
            var info = TezBaseItemInfo.createItemInfo(reader);

            var fixed_id = info.itemID.fixedID;

            while (fixed_id >= mFixedList.Count)
            {
                mFixedList.Add(null);
            }

            if (mFixedList[fixed_id] != null)
            {
                throw new Exception($"This item slot has registered! [{info.NID}: {fixed_id}] ==> {path}");
            }

            mFixedList[fixed_id] = info;
            mFileData.Add(info.NID, info);
        }

        /// <summary>
        /// 取得文件数据
        /// </summary>
        public TezReader getItemReader(string name)
        {
            return mFileData[name].dataReader;
        }

        public TezReader getItemReader(TezItemID itemID)
        {
            return this.getItemInfo(itemID).dataReader;
        }

        /// <summary>
        /// 尝试取得文件数据
        /// </summary>
        public bool tryGetItemReader(string name, out TezReader dataReader)
        {
            if (mFileData.TryGetValue(name, out var wrapper))
            {
                dataReader = wrapper.dataReader;
                return true;
            }

            dataReader = null;
            return false;
        }

        public TezFixedItemInfo getItemInfoByFDID(int FDID)
        {
            return (TezFixedItemInfo)mFixedList[FDID];
        }

        /// <summary>
        /// 获得信息
        /// </summary>
        public TezBaseItemInfo getItemInfo(TezItemID itemID)
        {
            return this.getItemInfo(itemID.fixedID, itemID.modifiedID);
        }

        public TezBaseItemInfo getItemInfo(int FDID, int MDID = -1)
        {
            if (MDID >= 0)
            {
                return mModifiedList[MDID];
            }

            return mFixedList[FDID];
        }

        public TezBaseItemInfo getItemInfo(string name)
        {
            return mFileData[name];
        }

        public bool tryGetItemInfo(string name, out TezBaseItemInfo itemInfo)
        {
            return mFileData.TryGetValue(name, out itemInfo);
        }


        #region Modified
        public int generateMDID()
        {
            if (mFreeIndex.Count > 0)
            {
                return mFreeIndex.Dequeue();
            }
            else
            {
                int index = mModifiedList.Count;
                mModifiedList.Add(null);
                return index;
            }
        }

        public bool registerItem(TezRuntimeItemInfo info)
        {
            var modified_id = info.itemID.modifiedID;
            if (mModifiedList[modified_id] != null)
            {
                return false;
            }

            mModifiedList[modified_id] = info;
            return true;
        }

        public void unregisterItem(int modifiedID)
        {
            mFreeIndex.Enqueue(modifiedID);
            mModifiedList[modifiedID] = null;
        }
        #endregion
    }
}