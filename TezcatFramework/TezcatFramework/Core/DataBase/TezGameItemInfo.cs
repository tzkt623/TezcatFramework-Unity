namespace tezcat.Framework.Core
{
    /// <summary>
    /// 
    /// 一个物品对象
    /// 1.MemAddress
    /// 2.TypeID-----
    ///              >>DBID---
    /// 3.UniqueID---         >>ItemID
    /// 4.RTID----------------
    /// 5.ItemID与StackCount合成ItemInfo
    /// 
    /// 所有的数据库物品RTID全都为-1
    /// 
    /// 保存时DBID和RTID都要保存,因为他们合成的ID才是真正的物品ID
    /// 同时需要保存RTID的当前最大值,以及RTID的FreeList
    /// 
    /// <para>
    /// 可堆叠物品共享同一个ItemInfo
    /// 所以他们共享同一个ItemID
    /// </para>
    /// 
    /// <para>
    /// 一个不可堆叠的护甲物品
    /// 
    /// 钢护甲(TID = 2, UID = 1, RTID = 1, MemAddress)
    /// 力量之钢护甲(TID = 2, UID = 1, RTID = 2, MemAddress)
    /// 快速的钢护甲(TID = 2, UID = 1, RTID = 3, MemAddress)
    /// 力量之快速的钢护甲(TID = 2, UID = 1, RTID = 4, MemAddress)
    /// 
    /// 每一个物品都需要单独保存
    /// 保存时DBID和RTID都要保存,因为他们合成的ID才是真正的物品ID
    /// 并且保存当前RTID的最大值,以及当前有哪些Free RTID
    /// </para>
    /// 
    /// <para>
    /// 一个可堆叠的消耗物品
    /// 
    /// 大型血瓶(TID = 3, UID = 3, RTID = 5, MemAddress)
    /// 高效之大型血瓶(TID = 3, UID = 3, RTID = 6, MemAddress)
    /// 
    /// 如果爆出来10瓶血瓶
    /// 他们在地上放着时的MemAddress肯定是不同的
    /// 但是他们的ItemInfo以及ID都是一样的(TID = 3, UID = 3, RTID = 6)
    /// 当玩家捡起来时,删除不同的内存对象,只保留一份以及堆叠数量即可
    /// 
    /// 每个物品都是可共享的物品,保存一份即可
    /// </para>
    /// 
    /// <para>
    /// 爆物品流程
    /// 
    /// 1.从物品池中挑选一个原型
    /// 2.生成前缀,生成后缀
    /// 3.组合前后缀,生成物品
    /// 4.赋予物品一个新的RTID
    /// </para>
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public class TezGameItemInfo
        : ITezCloseable
        , ITezSerializable
    {
        class MetaData
        {
            public int refCount;
            public TezItemID itemID;
            public string NID;
            public int stackCount;
            public TezItemObject proto;
            public TezCategory category;
        }

        private MetaData mMetaData = null;
        private TezItemObject mOwner = null;

        public TezItemID itemID => mMetaData.itemID;
        public TezCategory category => mMetaData.category;

        public string NID
        {
            get { return mMetaData.NID; }
            set { mMetaData.NID = value; }
        }

        /// <summary>
        /// 堆叠数量
        /// 0 : 初始化
        /// -1 : 物品不可堆叠,物品生成的对象数据独立,必须克隆模板数据
        /// >=1 : 物品可以堆叠,物品生成的对象数据共享,仅对模板数据只读
        /// </summary>
        public int stackCount => mMetaData.stackCount;
        public bool isShared => mMetaData.stackCount > 1;
        public bool invalid => mMetaData == null;
        public bool isProto => (mMetaData.proto != null) && (mOwner == mMetaData.proto);


        public TezGameItemInfo(TezItemObject itemObject)
        {
            mOwner = itemObject;
        }

        /// <summary>
        /// 从数据库中加载物品元信息
        /// </summary>
        public void initFrom(string NID, int stackCount, ushort TID, ushort UID, TezCategory category)
        {
            mMetaData = new MetaData()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                itemID = TezItemID.create(TID, UID),
                category = category
            };
        }

        /// <summary>
        /// 从保存的数据中加载物品元信息
        /// </summary>
        public void initFrom(string NID, int stackCount, ushort TID, ushort UID, int RTID, TezCategory category)
        {
            mMetaData = new MetaData()
            {
                refCount = 1,
                NID = NID,
                stackCount = stackCount,
                itemID = TezItemID.create(TID, UID, RTID),
                category = category
            };
        }

        /// <summary>
        /// 从另一个元数据中复制共享数据
        /// </summary>
        public void initFrom(TezGameItemInfo itemInfo)
        {
            mMetaData = itemInfo.mMetaData;
            mMetaData.refCount++;
        }

        /// <summary>
        /// 根据模版重定义数据
        /// </summary>
        public void remodifyFrom(TezGameItemInfo template)
        {
            this.initFrom(template.NID, template.stackCount, template.itemID.TID, template.itemID.IID, template.category);
        }

        public override int GetHashCode()
        {
            return mMetaData.itemID.GetHashCode();
        }

        public void setProto(TezItemObject proto)
        {
            mMetaData.proto = proto;
        }

        public TezGameItemInfo share()
        {
            mMetaData.refCount++;
            return this;
        }

        public static bool isError(TezGameItemInfo info)
        {
            return info.itemID.DBID == 0;
        }

        public void close()
        {
            mOwner = null;
            if ((--mMetaData.refCount) > 0)
            {
                return;
            }

            mMetaData.itemID.close();

            mMetaData.itemID = null;
            mMetaData.NID = null;
            mMetaData.proto = null;
            mMetaData.category = null;

            mMetaData = null;
        }

        public void serialize(TezWriter writer)
        {
            writer.beginObject(TezBuildInName.ItemInfo);
            writer.write(TezBuildInName.Name, this.NID);
            writer.write(TezBuildInName.StackCount, this.stackCount);

            writer.write(TezBuildInName.Type, TezItemID.getTypeName(this.itemID.TID));
            writer.write(TezBuildInName.IID, this.itemID.IID);
            if (this.itemID.RTID > -1)
            {
                writer.write(TezBuildInName.RTID, this.itemID.RTID);
            }

            writer.write(TezBuildInName.IsProto, this.isProto);

            writer.endObject(TezBuildInName.ItemInfo);
        }

        public void deserialize(TezReader reader)
        {
            reader.beginObject(TezBuildInName.ItemInfo);

            if (!reader.tryRead(TezBuildInName.RTID, out int runtime_id))
            {
                runtime_id = -1;
            }

            TezCategory category = null;
            if (reader.tryRead(TezBuildInName.Category, out string categoryName))
            {
                category = TezCategorySystem.getCategory(categoryName);
            }

            this.initFrom(reader.readString(TezBuildInName.Name)
                , reader.readInt(TezBuildInName.StackCount)
                , TezItemID.getTypeID(reader.readString(TezBuildInName.Type))
                , (ushort)reader.readInt(TezBuildInName.IID)
                , runtime_id
                , category);

            if (reader.readBool(TezBuildInName.IsProto))
            {
                this.setProto(mOwner);
            }

            reader.endObject(TezBuildInName.ItemInfo);
        }
    }
}
