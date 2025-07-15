using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 
    /// 原型对象信息
    /// 
    /// 用于记录物品原型的摘要数据信息
    /// 用于对象的生成,比较,分类等
    /// 
    /// </summary>
    public class TezProtoItemInfo
        : ITezObjectPoolItem
        , ITezSerializable
    {
        /// <summary>
        /// ID元数据
        /// 用于在各个Info之间共享数据
        /// </summary>
        class MetaData
        {
            /// <summary>
            /// 引用数量
            /// 用于记录产生了多少个共享对象
            /// </summary>
            public int refCount = -1;
            /// <summary>
            /// 物品ID
            /// </summary>
            public TezItemID itemID = null;
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
            public bool customizable = false;
            /// <summary>
            /// 原型对象
            /// </summary>
            public TezProtoObject proto = null;


            public void clear()
            {
                this.itemID.close();
                this.itemID = null;

                this.refCount = -1;
                this.NID = null;
                this.stackCount = -1;
                this.proto = null;
            }
        }

        private MetaData mMetaData = null;

        /// <summary>
        /// 物品ID
        /// </summary>
        public TezItemID itemID => mMetaData.itemID;

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
        public bool isCustomizable => mMetaData.customizable;

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
            mMetaData = new MetaData()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                customizable = customizable,
                itemID = TezItemID.create(TID, UID),
            };
        }

        /// <summary>
        /// 从保存的数据中加载物品元信息
        /// </summary>
        public void init(string NID, int stackCount, bool customizable, ushort TID, ushort UID, int RTID)
        {
            mMetaData = new MetaData()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                customizable = customizable,
                itemID = TezItemID.create(TID, UID, RTID)
            };
        }

        /// <summary>
        /// 从另一个元数据中复制共享数据
        /// </summary>
        public void init(TezProtoItemInfo itemInfo)
        {
            mMetaData = itemInfo.mMetaData;
            mMetaData.refCount++;
        }

        /// <summary>
        /// 根据模版重定义数据
        /// </summary>
        public void remodifyFrom(TezProtoItemInfo protoInfo)
        {
            this.init(protoInfo.NID, protoInfo.stackCount, protoInfo.isCustomizable, protoInfo.itemID.TID, protoInfo.itemID.IID);
        }

        public override int GetHashCode()
        {
            return mMetaData.itemID.GetHashCode();
        }

        public void setProto(TezProtoObject proto)
        {
            mMetaData.proto = proto;
        }

        public TezProtoObject getProto()
        {
            return mMetaData.proto;
        }

        public ITezProtoObject spawnObject()
        {
            return mMetaData.proto.spawnObject();
        }

        public T spawnObject<T>() where T : TezProtoObject
        {
            return mMetaData.proto.spawnObject<T>();
        }

        public TezProtoItemInfo share()
        {
            mMetaData.refCount++;
            return this;
        }

        public static bool isError(TezProtoItemInfo info)
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
            writer.enterObject(TezBuildInName.ProtoInfo);

            //writer.write(TezBuildInName.Type, TezItemID.getTypeName(this.itemID.TID));

            //必须保存物品IID
            //物品的TypeID跟ClassID绑定
            writer.write(TezBuildInName.IID, this.itemID.IID);

            //如果是实时生成的新物品
            //需要保存所有数据
            if (this.itemID.RTID > -1)
            {
                writer.write(TezBuildInName.Customizable, this.isCustomizable);
                writer.write(TezBuildInName.RTID, this.itemID.RTID);
                writer.write(TezBuildInName.Name, this.NID);
                writer.write(TezBuildInName.StackCount, this.stackCount);
            }

            writer.exitObject(TezBuildInName.ProtoInfo);
        }

        public void deserialize(TezSaveController.Reader reader)
        {
            var CID = reader.readString(TezBuildInName.CID);

            reader.enterObject(TezBuildInName.ProtoInfo);

            if (!reader.tryRead(TezBuildInName.RTID, out int runtime_id))
            {
                runtime_id = -1;
            }

            this.init(reader.readString(TezBuildInName.Name)
                , reader.readInt(TezBuildInName.StackCount)
                , reader.readBool(TezBuildInName.Customizable)
                , (ushort)TezcatFramework.protoDB.getTypeID(CID)
                , (ushort)reader.readInt(TezBuildInName.IID)
                , runtime_id);

            reader.exitObject(TezBuildInName.ProtoInfo);
        }

        bool ITezObjectPoolItem.recycleThis()
        {
            if ((--mMetaData.refCount) > 0)
            {
                return false;
            }

            mMetaData.clear();
            mMetaData = null;

            return true;
        }

        void ITezObjectPoolItem.destroyThis()
        {
            
        }
    }
}
