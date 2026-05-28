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
    public abstract class TezProtoObjectData 
        : ITezCloseable
        , ITezProtoLoader
    {
        protected TezProtoInfoWrapper mProtoInfo = TezObjectPool.create<TezProtoInfoWrapper>();
        public TezProtoInfoWrapper protoInfo => mProtoInfo;

        bool mInitialized = false;

        public void close()
        {
            this.onClose();
        }

        internal void init()
        {
            if (!mInitialized)
            {
                mInitialized = true;
                this.onInit();
            }
        }

        protected abstract void onInit();

        /// <summary>
        /// 创建Entity
        /// </summary>
        public TezWorld.Entity createEntity()
        {
            var entity = TezWorld.createEntity();
            this.onCreateEntity(ref entity, this.copyOrShare());
            return entity;
        }

        public TezWorld.Entity detachEntity()
        {
            var entity = TezWorld.createEntity();
            this.onCreateEntity(ref entity, this.detachNewPortoObjectFromThis());
            return entity;
        }

        protected abstract void onCreateEntity(ref TezWorld.Entity entity, TezProtoObjectData protoData);
        /*
        public TezProtoObject createObjectByCopyMe(int index = 0)
        {
            var obj = this.createObjectInternal(index);
            obj.initProtoData(this.copyOrShare());
            return obj;
        }

        public T createObjectByCopyMe<T>(int index = 0) where T : TezProtoObject
        {
            return (T)this.createObjectByCopyMe(index);
        }

        public TezProtoObject createObjectWithMe(int index = 0)
        {
            var obj = this.createObjectInternal(index);
            obj.initProtoData(this);
            return obj;
        }

        public ProtoObject createObjectWithMe<ProtoObject>(int index = 0) where ProtoObject : TezProtoObject
        {
            return (ProtoObject)this.createObjectWithMe(index);
        }

        protected virtual TezProtoObject createObjectInternal(int index) { throw new System.NotImplementedException(); }
        */

        public void retain()
        {
            mProtoInfo.retain();
        }

        /// <summary>
        /// 如果是共享对象 会共享自身
        /// 不是共享对象 会复制自身
        /// </summary>
        public TezProtoObjectData copyOrShare()
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
            data.init();
            return data;
        }

        /// <summary>
        /// 从当前protoobject对象中分离出一个新的protoobject对象
        /// </summary>
        public TezProtoObjectData detachNewPortoObjectFromThis()
        {
            var data = this.beginNewObject_JustNewProtoData();
            //copy一份数据不会修改info的元数据
            //只会修改引用计数
            data.mProtoInfo.sharedFrom(mProtoInfo);
            data.mProtoInfo.redefineProtoInfo();
            data.endNewObject_CopyDataFrom(data);
            data.init();
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
            this.baseProtoData.init();
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