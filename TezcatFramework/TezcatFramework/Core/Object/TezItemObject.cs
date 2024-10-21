using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /*
     * 
     * 
     */

    public interface ITezItemObject
    {
        TezProtoItemInfo itemInfo { get; }
    }

    /// <summary>
    /// 可读可写物品
    /// 玩家可自定义物品
    /// 
    /// 可自定义物品在保存的时候需要单独保存每一个数据
    /// </summary>
    public interface ITezCustomItemObject
    {

    }

    /// <summary>
    /// 只读物品
    /// 玩家不能自定义物品
    /// 
    /// 不可自定义物品在保存的时候只需要保存数据库ID以及个数
    /// </summary>
    public interface ITezReadOnlyItemObject
    {
    }


    /// <summary>
    /// 游戏物品对象
    /// 
    /// 动态生成的物品对象生命周期归持有它的对象管理
    /// </summary>
    public abstract class TezItemObject
        : TezGameObject
        , ITezProtoObject
    {
        protected TezProtoItemInfo mItemInfo = null;
        public TezProtoItemInfo itemInfo => mItemInfo;

        protected override void onClose()
        {
            base.onClose();
            mItemInfo.poolRecycle();
            if (mItemInfo.invalid)
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
            mItemInfo.serialize(writer);

            writer.beginObject(TezBuildInName.ObjectData);
            this.onSerialize(writer);
            writer.endObject(TezBuildInName.ObjectData);
        }

        protected virtual void onSerialize(TezWriter reader)
        {

        }

        public sealed override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
            //mItemInfo = new TezGameItemInfo(this);
            mItemInfo = TezObjectPool.create<TezProtoItemInfo>();
            mItemInfo.deserialize(reader);
            mItemInfo.setProto(this);

            reader.beginObject(TezBuildInName.ObjectData);
            this.onDeserialize(reader);
            reader.endObject(TezBuildInName.ObjectData);
        }

        protected virtual void onDeserialize(TezReader reader)
        {

        }

        /// <summary>
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
        public ITezProtoObject spawnObject()
        {
            return mItemInfo.isCustomizable ? this.shareObject() : this.remodifyObject();
        }

        public T spawnObject<T>() where T : TezItemObject
        {
            return (T)this.spawnObject();
        }

        public TezItemObject shareObject()
        {
            mItemInfo.share();
            return this;
        }

        protected virtual TezItemObject copy()
        {
            throw new NotImplementedException(this.GetType().Name);
        }

        public TezItemObject remodifyObject()
        {
            var obj = this.copy();
            obj.copyDataFrom(this);
            return obj;
        }

        /// <summary>
        /// 使用模板对象初始化
        /// </summary>
        private void copyDataFrom(TezItemObject template)
        {
            //mItemInfo = new TezGameItemInfo(this);
            mItemInfo = TezObjectPool.create<TezProtoItemInfo>();
            mItemInfo.remodifyFrom(template.itemInfo);
            mItemInfo.setProto(this);
            this.onCopyDataFrom(template);
        }

        protected virtual void onCopyDataFrom(TezItemObject template)
        {

        }
    }

    public static class TezItemObjectHelper
    {
        public static T remodifyObject<T>(this TezItemObject obj) where T : TezItemObject
        {
            return (T)obj.remodifyObject();
        }

        public static T shareObject<T>(this TezItemObject obj) where T : TezItemObject
        {
            return (T)obj.shareObject();
        }
    }
}