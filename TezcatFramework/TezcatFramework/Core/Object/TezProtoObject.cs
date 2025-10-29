using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    /*
     * 新理论
     * 
     * class结构
     * 成员变量:只包含所有直接数据变量
     * 系统变量:包含一个单独的系统对象,系统对象里面包含所有系统
     * 初始化时将系统所需要的变量数据注入到各个系统中
     * 
     * 
     * class--1
     *  value1
     *  value2
     *  value3
     *  
     *  value_system1
     *  value_system2
     *  value_system3
     * 
     *  system
     *    system1
     *    system2
     *    system3
     *    
     *  class--2
     *   data_unit
     *   data_unit_skill
     *  
     *   system_skill
     *   system_skill
     *   
     *   
     *  class--角色
     *   health
     */
    public abstract class TezBasicObjectData : ITezCloseable
    {
        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }

    public interface ITezProtoObjectData
    {
        TezProtoInfoWrapper protoInfo { get; }
        TezProtoObject createObject();
        TezProtoObjectData copy();
    }

    public interface ITezProtoObjectTypeGetter<T> where T : TezProtoObject
    {

    }

    public static class TezProtoObjectExtension
    {
        public static T createProtoObject<T>(this ITezProtoObjectTypeGetter<T> p) where T : TezProtoObject, new()
        {
            return new T();
        }
    }

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
    public abstract class TezProtoObjectData : TezBasicObjectData, ITezProtoObjectData
    {
        protected TezProtoInfoWrapper mProtoInfo = TezObjectPool.create<TezProtoInfoWrapper>();
        public TezProtoInfoWrapper protoInfo => mProtoInfo;

        bool mInitialized = false;

        public void init()
        {
            if(mInitialized)
            {
                mInitialized = true;
                this.onInit();
            }
        }

        protected abstract void onInit();

        public TezProtoObject createObject()
        {
            if (!mProtoInfo.isSharedType)
            {
                //如果不是共享类型,那么需要先复制一份数据
                //然后再将数据注入到新对象中
                var data = this.copy();
                var obj = this.createObjectInternal();
                obj.initProtoData(data);
                obj.init();
                return obj;
            }
            else
            {
                //如果是共享类型,那么直接将当前数据注入到新对象中
                var obj = this.createObjectInternal();
                obj.initProtoData(this);
                obj.init();
                return obj;
            }
        }

        public T createObject<T> () where T : TezProtoObject
        {
            return (T)this.createObject();
        }

        protected abstract TezProtoObject createObjectInternal();

        public TezProtoObjectData copy()
        {
            var data = this.copySelfDataWithoutProtoInfo();
            //copy一份数据不会修改info的元数据
            //只会修改引用计数
            data.protoInfo.sharedFrom(mProtoInfo);
            return data;
        }

        protected abstract TezProtoObjectData copySelfDataWithoutProtoInfo();

        public void changeToRuntimeData()
        {
            //将当前数据转换为运行时数据
            mProtoInfo.changeToRuntimeInfo();
            //在运行时数据库中申请一个运行时ID,并保存模版对象
            TezcatFramework.runtimeDB.registerItem(this);
        }

        public void loadProtoData(TezSaveController.Reader reader)
        {
            mProtoInfo.loadProtoData(reader);

            reader.enterObject(TezBuildInName.SaveChunkName.ObjectData);
            this.onLoadProtoData(reader);
            reader.exitObject(TezBuildInName.SaveChunkName.ObjectData);
        }

        protected abstract void onLoadProtoData(TezSaveController.Reader reader);

        protected override void onClose()
        {
            if(mProtoInfo.recycleToPool())
            {
                mProtoInfo = null;
            }
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
        protected TezProtoObjectData mProtoData = null;

        public TezProtoObjectData baseProtoData => mProtoData;
        public TezProtoInfoWrapper protoInfo => mProtoData.protoInfo;

        public virtual void initProtoData(TezProtoObjectData data)
        {
            mProtoData = data;
            mProtoData.protoInfo.retain();
            mProtoData.init();
        }

        protected abstract TezProtoObjectData createProtoData();

        protected override void onClose()
        {
            base.onClose();
            mProtoData.close();
            mProtoData = null;
        }

        public bool isItemOf(TezProtoObject other)
        {
            return mProtoData.protoInfo.itemID == other.mProtoData.protoInfo.itemID;
        }

        public sealed override void loadProtoData(TezSaveController.Reader reader)
        {
            base.loadProtoData(reader);

            mProtoData = this.createProtoData();
            mProtoData.loadProtoData(reader);

            this.onLoadProtoData(reader);
        }

        protected virtual void onLoadProtoData(TezSaveController.Reader reader)
        {

        }

        /// <summary>
        /// 以当前对象为原型对象
        /// 制作一份运行时原型对象
        /// 并将当前对象从原数据对象中剥离
        /// 指向新建的运行时原型对象
        /// </summary>
        public void makeAnRuntimeProtoByThis()
        {
            var copy = mProtoData.copy();
            copy.changeToRuntimeData();
            mProtoData.close();
            mProtoData = copy;
        }
    }
}