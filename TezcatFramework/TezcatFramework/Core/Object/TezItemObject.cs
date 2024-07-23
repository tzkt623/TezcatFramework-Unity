using System;

namespace tezcat.Framework.Core
{
    public interface ITezItemObject
    {
        TezGameItemInfo itemInfo { get; }
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
        protected TezGameItemInfo mItemInfo = null;
        public TezGameItemInfo itemInfo => mItemInfo;

        protected override void onClose()
        {
            base.onClose();
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
            mItemInfo = new TezGameItemInfo(this);
            mItemInfo.deserialize(reader);

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
            return mItemInfo.isShared ? this.shareObject() : this.remodifyObject();
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
            mItemInfo = new TezGameItemInfo(this);
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