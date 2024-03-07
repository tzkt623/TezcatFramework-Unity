using System;
using tezcat.Framework.Database;
using tezcat.Framework.Game;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可放入Inventory的对象
    /// 需要使用ItemInfo里面的数据进行操作
    /// </summary>
    public interface ITezItemObject : ITezCloseable
    {
        TezGameItemInfo itemInfo { get; }
        void init();
        void copyDataFrom(ITezItemObject template);

        ITezItemObject duplicate();
        ITezItemObject remodify();
        ITezItemObject share();
    }

    public static class TezItemObjectTool
    {
        public static void loadItemInfo(this ITezItemObject itemObject, TezReader reader)
        {
            reader.beginObject(TezBuildInName.ItemInfo);

            if (!reader.tryRead(TezBuildInName.RuntimeID, out int runtime_id))
            {
                runtime_id = -1;
            }

            TezCategory category = null;
            if (reader.tryRead(TezBuildInName.Category, out string categoryName))
            {
                category = TezCategorySystem.getCategory(categoryName);
            }

            itemObject.itemInfo.initFrom(""
                , reader.readString(TezBuildInName.NameID)
                , reader.readInt(TezBuildInName.StackCount)
                , (ushort)reader.readInt(TezBuildInName.TypeID)
                , (ushort)reader.readInt(TezBuildInName.UniqueID)
                , runtime_id
                , category);

            reader.endObject(TezBuildInName.ItemInfo);
        }

        public static T duplicate<T>(this ITezItemObject obj) where T : ITezItemObject
        {
            return (T)obj.duplicate();
        }

        public static T remodify<T>(this ITezItemObject obj) where T : TezItemObject
        {
            return (T)obj.remodify();
        }
    }

    /// <summary>
    /// 游戏物品对象
    /// 
    /// 动态生成的物品对象生命周期归持有它的对象管理
    /// </summary>
    public abstract class TezItemObject
        : TezGameObject
        , ITezItemObject
    {
        protected TezGameItemInfo mItemInfo = null;
        public TezGameItemInfo itemInfo => mItemInfo;
        bool mIsInited = false;

        public override void init()
        {
            if (!mIsInited)
            {
                mIsInited = true;
                base.init();
            }
        }

        /// <summary>
        /// 使用模板对象初始化
        /// </summary>
        public void copyDataFrom(ITezItemObject template)
        {
            mItemInfo = new TezGameItemInfo();
            mItemInfo.remodifyFrom(template.itemInfo);
            mItemInfo.setPrototype(this);
            this.onCopyDataFrome(template);
        }

        protected virtual void onCopyDataFrome(ITezItemObject template)
        {

        }

        public override void close()
        {
            base.close();
            mItemInfo.close();
            if(mItemInfo.invalid)
            {
                mItemInfo = null;
            }
        }

        /// <summary>
        /// 向数据库保存数据
        /// </summary>
        public sealed override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            writer.beginObject(TezBuildInName.ItemInfo);
            writer.write(TezBuildInName.NameID, mItemInfo.NID);
            writer.write(TezBuildInName.StackCount, mItemInfo.stackCount);

            writer.write(TezBuildInName.TypeID, TezItemID.getTypeName(mItemInfo.itemID.TID));
            writer.write(TezBuildInName.UniqueID, mItemInfo.itemID.UID);
            if (mItemInfo.itemID.RTID > -1)
            {
                writer.write(TezBuildInName.RuntimeID, mItemInfo.itemID.RTID);
            }
            writer.endObject(TezBuildInName.ItemInfo);

            writer.beginObject(TezBuildInName.ItemData);
            this.onSerialize(writer);
            writer.endObject(TezBuildInName.ItemData);
        }

        protected virtual void onSerialize(TezWriter reader)
        {

        }

        public sealed override void deserialize(TezReader reader)
        {
            base.deserialize(reader);

            reader.beginObject(TezBuildInName.ItemInfo);
            if (!reader.tryRead(TezBuildInName.RuntimeID, out int runtime_id))
            {
                runtime_id = -1;
            }

            TezCategory category = null;
            if (reader.tryRead(TezBuildInName.Category, out string categoryName))
            {
                category = TezCategorySystem.getCategory(categoryName);
            }

            mItemInfo = new TezGameItemInfo();
            mItemInfo.initFrom(""
                , reader.readString(TezBuildInName.NameID)
                , reader.readInt(TezBuildInName.StackCount)
                , TezItemID.getTypeID(reader.readString(TezBuildInName.TypeID))
                , (ushort)reader.readInt(TezBuildInName.UniqueID)
                , runtime_id
                , category);
            reader.endObject(TezBuildInName.ItemInfo);

            reader.beginObject(TezBuildInName.ItemData);
            this.onDeserialize(reader);
            reader.endObject(TezBuildInName.ItemData);
        }

        protected virtual void onDeserialize(TezReader reader)
        {

        }

        /// <summary>
        /// 双份数据
        /// 
        /// <para>
        /// 如果是SharedObject会共享此对象的数据
        /// </para>
        /// 
        /// <para>
        /// 如果是UniqueObject会复制此对象的数据
        /// </para>
        /// 
        /// </summary>
        public ITezItemObject duplicate()
        {
            return mItemInfo.isShared ? this.share() : this.remodify();
        }

        protected virtual ITezItemObject copy()
        {
            throw new NotImplementedException(this.GetType().Name);
        }

        public ITezItemObject remodify()
        {
            var obj = this.copy();
            obj.copyDataFrom(this);
            return obj;
        }

        public ITezItemObject share()
        {
            mItemInfo.share();
            return this;
        }
    }
}