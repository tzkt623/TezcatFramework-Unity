using System.Collections.Generic;
using tezcat.Framework.ArchetypeECS;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 元数据信息里面必须包含整个对象的所有数据
    /// 
    /// 数据有三种使用方法
    /// 1.被N个单位共享,改变此数据会改变所有单位的数据
    /// 2.被N个单位复制,改变此数据不会改变其他单位的数据
    /// 3.被变成运行时数据,创造出一个新的原型,此数据会被运行时数据库管理
    /// 
    /// 当从数据库里面生成一个对象时,需要明确知道此对象是不是可以共享的对象
    /// 1.如果是可共享对象,那么全部共享数据库的数据,新对象的数据全都是同一个
    /// 2.如果是不可共享对象,那么复制一份数据,新对象的数据是独立的
    /// 
    /// </summary>
    /// 

    public enum TezProtoObjectCreateMode
    {
        /// <summary>
        /// 复制当前对象的数据
        /// 
        /// <para>
        /// 如果当前对象是共享型
        /// 那么复制之后的对象与原型对象为同一个
        /// </para>
        /// 
        /// <para>
        /// 如果当前对象不是共享型
        /// 那么复制之后的对象与原型对象不是同一个,但是他们的ItemID相同
        /// </para>
        /// 
        /// </summary>
        Copy,

        /// <summary>
        /// 新建对象
        /// 
        /// <para>
        /// 新建一个数据对象,与当前原型对象不是同一个,并且ItemID也不同
        /// </para>
        /// </summary>
        New
    }

    public interface ITezEntityProtoData
    {

    }

    public interface ITezProtoData
    {

    }

    public class TezProtoObjectHelper
    {
        public TezWorld.Entity createEntity(TezProtoObjectData data)
        {
            var entity = TezWorld.instance.createEntity();
            return entity;
        }
    }

    public abstract class TezProtoObjectData
        : ITezCloseable
        , ITezProtoLoader
    {
        protected TezProtoInfoWrapper mProtoInfo = TezObjectPool.create<TezProtoInfoWrapper>();
        public TezProtoInfoWrapper protoInfo => mProtoInfo;

        public void close()
        {
            this.onClose();
        }

        protected abstract void onInit();
        
        public void retain()
        {
            mProtoInfo.retain();
        }

        public TezProtoObjectData createDataWhitMe(TezProtoObjectCreateMode mode)
        {
            switch (mode)
            {
                case TezProtoObjectCreateMode.Copy:
                    return this.copyOrShareFromThis();
                case TezProtoObjectCreateMode.New:
                    return this.newPortoObjectFromThis();
                default:
                    break;
            }

            throw new System.Exception();
        }

        public T createDataWhitMe<T>(TezProtoObjectCreateMode mode) where T : TezProtoObjectData
        {
            switch (mode)
            {
                case TezProtoObjectCreateMode.Copy:
                    return (T)this.copyOrShareFromThis();
                case TezProtoObjectCreateMode.New:
                    return (T)this.newPortoObjectFromThis();
                default:
                    break;
            }

            throw new System.Exception();
        }

        /// <summary>
        /// <para>如果是共享对象 会共享自身</para>
        /// <para>不是共享对象 会复制自身</para>
        /// <para>复制之后的对象与原型对象不是同一个,但是他们的ItemID相同</para>
        /// </summary>
        private TezProtoObjectData copyOrShareFromThis()
        {
            //如果是共享类型,那么直接将当前数据注入到新对象中
            if (mProtoInfo.isSharedType)
            {
                return this;
            }

            //如果不是共享类型,那么需要先复制一份数据
            //然后再将数据注入到新对象中
            var data = this.beginNewObject_JustNewProtoData();
            //copy一份数据不会修改info的元数据
            data.mProtoInfo.copyFrom(mProtoInfo);
            data.endNewObject_CopyDataFrom(this);
            return data;
        }

        /// <summary>
        /// 新建一个数据对象,复制原型的数据,但与当前原型对象不是同一个,并且ItemID也不同
        /// </summary>
        private TezProtoObjectData newPortoObjectFromThis()
        {
            var data = this.beginNewObject_JustNewProtoData();
            //copy一份数据不会修改info的元数据
            //只会修改引用计数
            data.mProtoInfo.sharedFrom(mProtoInfo);
            data.mProtoInfo.redefineProtoInfo();
            data.endNewObject_CopyDataFrom(this);
            return data;
        }

        protected abstract TezProtoObjectData beginNewObject_JustNewProtoData();
        protected abstract void endNewObject_CopyDataFrom(TezProtoObjectData other);

        //把存档数据加载到类中
        void ITezProtoLoader.loadProtoData(TezSaveController.Reader reader)
        {
            mProtoInfo.loadProtoData(reader);

            reader.enterObject(TezBuildInName.SaveChunkName.ObjectData);
            this.onLoadProtoData(reader);
            reader.exitObject(TezBuildInName.SaveChunkName.ObjectData);
        }

        protected abstract void onLoadProtoData(TezSaveController.Reader reader);

        protected virtual void onClose()
        {
            if (mProtoInfo.recycleToPool())
            {
                mProtoInfo = null;
            }
        }

        public bool isTheSameProtoDataOf(TezProtoObjectData other)
        {
            return this.protoInfo.itemID == other.protoInfo.itemID;
        }

        public bool isTheSameDataBaseIDOf(TezProtoObjectData other)
        {
            return this.protoInfo.itemID.DBID == other.protoInfo.itemID.DBID;
        }
    }

    public abstract class TezEntityProtoObjectData : TezProtoObjectData
    {
        protected TezWorld.Entity mEntity;
        protected bool mInitialized = false;

        /// <summary>
        /// 创建Entity
        /// </summary>
        public TezWorld.Entity instantiateEntity()
        {
            if(!mInitialized)
            {
                mEntity = TezWorld.instance.createEntity();
                this.onCreateEntity(ref mEntity);
                this.onInit();
                mInitialized = true;
            }

            return mEntity;
        }

        protected abstract void onCreateEntity(ref TezWorld.Entity entity);
    }

    /// <summary>
    /// 游戏原型对象
    /// 
    /// 用于管理原型数据 以及利用原型生成游戏对象
    /// 既然是原型对象,就说明此对象必然由某个原型生成
    /// 而不会凭空生成
    /// </summary>
    public abstract class TezProtoObject
        : TezGameObject
        , ITezProtoObject
    {
        static Queue<uint> sFreeIDs = new Queue<uint>();
        static uint sUIDGenerator = 1;

        private readonly uint mProtoObjectUID = 0;
        public uint protoObjectUID => mProtoObjectUID;
        public bool isProtoObjectValid => mProtoObjectUID > 0;

        public abstract TezProtoObjectData baseProtoData { get; }
        public TezProtoInfoWrapper protoInfo => this.baseProtoData.protoInfo;

        public TezProtoObject()
        {
            if(sFreeIDs.Count > 0)
            {
                mProtoObjectUID = sFreeIDs.Dequeue();
            }
            else
            {
                mProtoObjectUID = sUIDGenerator++;
            }
        }

        //读取原型数据到对象中
        internal void initProtoData(TezProtoObjectData data)
        {
            //mProtoData = data;
            this.onSetProtoData(data);
            this.baseProtoData.protoInfo.retain();
            //this.baseProtoData.init();
            this.onInitProtoData();
        }

        protected abstract void onSetProtoData(TezProtoObjectData data);
        protected virtual void onInitProtoData() { }

        protected override void onClose()
        {
            base.onClose();
            sFreeIDs.Enqueue(mProtoObjectUID);
            this.baseProtoData.close();
        }

        public bool isTheSameProtoObjectOf(ITezProtoObject other)
        {
            return mProtoObjectUID == other.protoObjectUID;
        }

        public bool isTheSameProtoDataOf(ITezProtoObject other)
        {
            return this.baseProtoData.protoInfo.itemID == other.baseProtoData.protoInfo.itemID;
        }
    }
}