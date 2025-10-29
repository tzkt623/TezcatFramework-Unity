using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 
    /// 原型对象信息包装器
    /// 
    /// 用于记录物品原型的摘要数据信息
    /// 用于对象的生成,比较,分类等
    /// 并在各个原型对象之间共享数据
    /// 
    /// </summary>
    public class TezProtoInfoWrapper
        : ITezObjectPoolItem
        , ITezProtoLoader
        , IEquatable<TezProtoInfoWrapper>
    {
        /// <summary>
        /// ID元数据
        /// 用于在各个Info之间共享数据
        /// 是整个iteminfo的核心数据
        /// 
        /// 在复制和共享时,此数据不会被复制
        /// 只会增加引用计数
        /// 
        /// 在产生新物品时,此数据才会被复制作为新物品的元数据
        /// 
        /// 此数据的生命周期归持有它的对象管理
        /// 当最后一个使用此数据的对象销毁时,此数据销毁
        /// 因为ProtoDB中始终有一个原型数据
        /// 所以原型数据在运行时永远不会被销毁
        /// 
        /// </summary>
        class ProtoInfo
        {
            public Action onClear = null;

            /// <summary>
            /// 引用数量
            /// 用于记录产生了多少个共享对象
            /// </summary>
            public int refCount = -1;
            /// <summary>
            /// 物品ID
            /// </summary>
            public TezItemID itemID = new TezItemID();
            /// <summary>
            /// 物品的名称ID
            /// 并不是真实名称,因为有本地化的原因
            /// </summary>
            public string NID = null;
            /// <summary>
            /// 可堆叠数量
            /// >1 : 可堆叠物品
            /// =1 : 不可堆叠物品
            /// -1 : 初始化
            /// </summary>
            public int stackCount = -1;
            /// <summary>
            /// 是否可自定化对象
            /// 可自定义的对象在保存时需要保存完整数据
            /// 不可自定义对象在保存时只需要保存数据库ID
            /// </summary>
            public bool copyType = false;
            /// <summary>
            /// 原型对象
            /// </summary>
            public TezProtoObject proto = null;

            /// <summary>
            /// 从另一个元数据中复制数据
            /// 并且设置引用计数为1
            /// </summary>
            public void copyDataFrom(ProtoInfo other)
            {
                this.NID = other.NID;
                this.stackCount = other.stackCount;
                this.copyType = other.copyType;
                this.itemID = other.itemID;
                this.refCount = 1; // 复制时引用计数重置为1
            }

            private void clear()
            {
                this.onClear?.Invoke();

                //this.itemID.close();

                this.refCount = -1;
                this.NID = null;
                this.stackCount = -1;
                this.proto = null;
            }

            public bool release()
            {
                if ((--this.refCount) == 0)
                {
                    this.refCount = -1;
                    this.clear();
                    return true;
                }

                return false;
            }
        }

        private ProtoInfo mMetaData = null;

        /// <summary>
        /// 物品ID
        /// </summary>
        public TezItemID itemID => mMetaData.itemID;

        public void setRTID(int id)
        {
            mMetaData.itemID.setRTID(id);
        }

        public void setDBID(ushort type, ushort index)
        {
            mMetaData.itemID.setDBID(type, index);
        }

        /// <summary>
        /// 物品的名称ID
        /// 并不是真实名称,因为有本地化的原因
        /// </summary>
        public string NID
        {
            get { return mMetaData.NID; }
            set { mMetaData.NID = value; }
        }

        /// <summary>
        /// 堆叠数量
        /// 为了匹配物品栏系统系统
        /// 0表示没有物品
        /// 1表示有1个物品
        /// 大于1表示有多个物品
        /// 
        /// >1 : 可堆叠物品
        /// =1 : 不可堆叠物品
        /// -1 : 初始化
        /// </summary>
        public int stackCount => mMetaData.stackCount;

        /// <summary>
        /// 是否是可自定义对象
        /// 可自定义对象保存时必须保存全部数据
        /// 不可自定义对象保存时只需要保存ItemID和数量
        /// </summary>
        public bool isSharedType => mMetaData.copyType;

        /// <summary>
        /// 此对象是否有效
        /// 无效对象会被对象池回收
        /// </summary>
        public bool invalid => mMetaData == null;

        ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

        /// <summary>
        /// 从数据库中加载物品元信息
        /// </summary>
        public void init(string NID, int stackCount, bool customizable, ushort TID, ushort UID)
        {
            mMetaData = new ProtoInfo()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                copyType = customizable
            };

            mMetaData.itemID.setDBID(TID, UID);
        }

        /// <summary>
        /// 从保存的数据中加载物品元信息
        /// </summary>
        public void init(string NID, int stackCount, bool customizable, ushort TID, ushort UID, int RTID)
        {
            mMetaData = new ProtoInfo()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                copyType = customizable
            };

            mMetaData.itemID.setDBID(TID, UID);
            mMetaData.itemID.setRTID(RTID);
        }

        /// <summary>
        /// 从另一个元数据中共享数据
        /// </summary>
        public void sharedFrom(TezProtoInfoWrapper itemInfo)
        {
            mMetaData?.release();

            mMetaData = itemInfo.mMetaData;
            mMetaData.refCount++;
        }

        public void changeToRuntimeInfo()
        {
            var old_info = mMetaData;
            mMetaData = new ProtoInfo();
            //复制旧的元数据
            mMetaData.copyDataFrom(old_info);
            //清除旧的元数据
            old_info.release();
        }

        /// <summary>
        /// 根据模版重定义数据
        /// </summary>
        public void remodifyFrom(TezProtoInfoWrapper protoInfo)
        {
            this.init(protoInfo.NID, protoInfo.stackCount, protoInfo.isSharedType, protoInfo.itemID.TID, protoInfo.itemID.IID);
        }

        public override int GetHashCode()
        {
            return mMetaData.itemID.GetHashCode();
        }

        public void setOnClear(Action action)
        {
            mMetaData.onClear = action;
        }

        public void setProto(TezProtoObject proto)
        {
            mMetaData.proto = proto;
        }

        public TezProtoObject getProto()
        {
            return mMetaData.proto;
        }

        public TezProtoObject shardeProto()
        {
            mMetaData.refCount++;
            return mMetaData.proto;
        }

        public void retain()
        {
            mMetaData.refCount++;
        }

        public TezProtoInfoWrapper share()
        {
            mMetaData.refCount++;
            return this;
        }

        public static bool isError(TezProtoInfoWrapper info)
        {
            return info.itemID.DBID == 0;
        }

        /// <summary>
        /// 如果是数据库物品
        /// 只需要保存数据库ID和数量,不需要保存具体数据
        /// 
        /// 如果是实时生成的新物品
        /// 需要保存所有数据
        /// </summary>
        public void serialize(TezSaveController.Writer writer)
        {
            writer.enterObject(TezBuildInName.SaveChunkName.ProtoInfo);

            //writer.write(TezBuildInName.Type, TezItemID.getTypeName(this.itemID.TID));

            //必须保存物品IID
            //物品的TypeID跟ClassID绑定
            writer.write(TezBuildInName.ProtoInfo.IID, this.itemID.IID);

            //如果是实时生成的新物品
            //需要保存所有数据
            if (this.itemID.RTID > -1)
            {
                writer.write(TezBuildInName.ProtoInfo.SharedType, this.isSharedType);
                writer.write(TezBuildInName.ProtoInfo.RTID, this.itemID.RTID);
                writer.write(TezBuildInName.ProtoInfo.Name, this.NID);
                writer.write(TezBuildInName.ProtoInfo.StackCount, this.stackCount);
            }

            writer.exitObject(TezBuildInName.SaveChunkName.ProtoInfo);
        }

        public void loadProtoData(TezSaveController.Reader reader)
        {
            var CID = reader.readString(TezBuildInName.CID);

            reader.enterObject(TezBuildInName.SaveChunkName.ProtoInfo);

            if (!reader.tryRead(TezBuildInName.ProtoInfo.RTID, out int runtime_id))
            {
                runtime_id = -1;
            }

            if (!reader.tryRead(TezBuildInName.ProtoInfo.StackCount, out int stack_count))
            {
                stack_count = 1;
            }

            if (!reader.tryRead(TezBuildInName.ProtoInfo.SharedType, out bool shared_type))
            {
                shared_type = false;
            }

            this.init(reader.readString(TezBuildInName.ProtoInfo.Name)
                , stack_count
                , shared_type
                , (ushort)TezcatFramework.protoDB.getTypeID(reader.readString(TezBuildInName.ProtoInfo.TID))
                , (ushort)reader.readInt(TezBuildInName.ProtoInfo.IID)
                , runtime_id);

            reader.exitObject(TezBuildInName.SaveChunkName.ProtoInfo);
        }

        bool ITezObjectPoolItem.tryRecycleThis()
        {
            if (mMetaData.release())
            {
                mMetaData = null;
                return true;
            }

            return false;
        }

        void ITezObjectPoolItem.onDestroyThis()
        {

        }

        public bool Equals(TezProtoInfoWrapper other)
        {
            return itemID == other.itemID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezProtoInfoWrapper)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemInfo"></param>
        public void copyFrom(TezProtoInfoWrapper itemInfo)
        {
            mMetaData?.release();
            mMetaData = new ProtoInfo();
            mMetaData.copyDataFrom(itemInfo.mMetaData);
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator ==(TezProtoInfoWrapper a, TezProtoInfoWrapper b)
        {
            return a.itemID == b.itemID;
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator !=(TezProtoInfoWrapper a, TezProtoInfoWrapper b)
        {
            return a.itemID != b.itemID;
        }
    }
}
