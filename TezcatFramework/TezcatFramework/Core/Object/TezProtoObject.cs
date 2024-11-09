using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 游戏原型对象
    /// 
    /// 用于管理原型数据 以及利用原型生成游戏对象
    /// </summary>
    public abstract class TezProtoObject
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
            return mItemInfo.isCustomizable ? this.remodifyObject() : this.shareObject();
        }

        public T spawnObject<T>() where T : TezProtoObject
        {
            return (T)this.spawnObject();
        }

        public TezProtoObject shareObject()
        {
            mItemInfo.share();
            return this;
        }

        protected virtual TezProtoObject copy()
        {
            throw new NotImplementedException(this.GetType().Name);
        }

        public TezProtoObject remodifyObject()
        {
            var obj = this.copy();
            obj.copyDataFrom(this);
            return obj;
        }

        /// <summary>
        /// 使用模板对象初始化
        /// </summary>
        private void copyDataFrom(TezProtoObject template)
        {
            //mItemInfo = new TezGameItemInfo(this);
            mItemInfo = TezObjectPool.create<TezProtoItemInfo>();
            mItemInfo.remodifyFrom(template.itemInfo);
            mItemInfo.setProto(this);
            this.onCopyDataFrom(template);
        }

        protected virtual void onCopyDataFrom(TezProtoObject template)
        {

        }
    }

    public static class TezProtoObjectHelper
    {
        public static T remodifyObject<T>(this TezProtoObject obj) where T : TezProtoObject
        {
            return (T)obj.remodifyObject();
        }

        public static T shareObject<T>(this TezProtoObject obj) where T : TezProtoObject
        {
            return (T)obj.shareObject();
        }
    }
}