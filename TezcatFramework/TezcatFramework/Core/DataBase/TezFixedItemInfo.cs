using System;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
    /*
     * 物品对象的元数据
     * 每一个由物品的生成的对象都会拥有一个此对象
     * 此对象用于记录一些共享型数据
     * 例如
     * 物品名称
     * 是否可以堆叠,堆叠数量
     * 物品ID,所有同类型物品共用同一个ID
     * 构建器
     * 文件数据读取器
     * 等
     * 
     * 如果在游戏运行时生成了一个物品
     * 谁持有这个物品,谁负责管理他,并不会加入物品数据库
     * 
     * 例如
     * 无主之地爆了一地的装备
     * 这些装备都是根据模板数据实时生成的
     * 没有被玩家捡起来的装备应该在某一时刻销毁
     * 爆装备管理器应该负责这堆未拾取的装备的生命周期
     * 
     */

    public abstract class TezBaseItemInfo : ITezCloseable
    {
        protected TezItemID mItemID = TezItemID.EmptyID;
        public TezItemID itemID => mItemID;

        protected TezReader mDataReader = null;
        public TezReader dataReader => mDataReader;

        public string NID { get; set; } = "$ErrorItem$";

        /*
         * #堆叠性质
         * 
         * 如果一个Item不可堆叠,在游戏运行时,每一个都是独立的Object
         * 例如EVE
         * 一艘飞船虽然数据都相同(同一艘船体),但是不同玩家,不同NPC操控的飞船,数据都是独立的
         * 一艘船受伤不会影响另一艘的数据
         * 
         * 如果一个Item可以堆叠,在游戏运行时,他是一个共享对象,必须是只读数据
         * 例如黑暗之魂
         * 原素瓶不管有多少瓶,本质上都是共享一个数据
         * +1/+2/+3原素瓶分别是不同的数据对象
         * 
         * 以上面的例子作为基础,规定
         * 1.所有可堆叠物品,在运行时都是共享对象,数据必须只读
         * 2.所有不可堆叠物品,在运行时都是独立对象,数据可读可写
         * 
         * ==0 : 物品不可堆叠,取得物品时必须克隆一份数据独立使用
         * >=1 : 物品可堆叠,取得物品时必须共享同一份数据
         */

        /// <summary>
        /// ==0 : 初始化
        /// ==-1 : 物品不可堆叠,物品生成的对象数据独立,必须克隆模板数据
        /// >=1 : 物品可以堆叠,物品生成的对象数据共享,仅对模板数据只读
        /// </summary>
        public int stackCount { get; set; } = 0;

        protected bool mReadOnly = false;
        public bool isReadOnly => mReadOnly;
        protected TezItemableObject mTemplate = null;
        protected TezClassFactory.Creator<TezItemableObject> mCreator = null;

        protected int mVersion = 0;
        public int version => mVersion;

        protected TezBaseItemInfo() { }

        protected void initData(string NID, int stackCount, int FDID, int MDID)
        {
            this.NID = NID;
            this.stackCount = stackCount;
            TezItemID.create(ref mItemID, FDID, MDID);
        }

        public bool isTemplate(TezItemableObject other)
        {
            return mReadOnly && object.ReferenceEquals(mTemplate, other);
        }

        public override int GetHashCode()
        {
            return mItemID.GetHashCode();
        }

        public void setTemplate(TezItemableObject obj)
        {
            mTemplate = obj;
        }

        /// <summary>
        /// 获得当前数据所提供的对象
        /// 此模板数据
        /// 要么是克隆数据
        /// 要么是共享数据
        /// </summary>
        public TezItemableObject getObject()
        {
            if (mReadOnly)
            {
                return mTemplate.share();
            }
            else
            {
                var obj = mCreator();
                obj.init(this.dataReader);
                obj.init(mTemplate);
                return obj;
            }
        }

        public T getObject<T>() where T : TezItemableObject
        {
            if (mReadOnly)
            {
                return (T)mTemplate.share();
            }
            else
            {
                T obj = (T)mCreator();
                obj.init(this.dataReader);
                return obj;
            }
        }

        public abstract int nextVersion();
        public abstract void updateVersion(int version);
        public abstract void addRef();

        public virtual void close()
        {

        }

        public abstract TezRuntimeItemInfo remodify();

        public static bool isError(TezBaseItemInfo info)
        {
            return info.itemID.fixedID == -1;
        }

        public static TezBaseItemInfo createItemInfo(TezReader reader)
        {
            TezBaseItemInfo result;
            var fdid = reader.readInt(TezReadOnlyString.FDID);
            if (!reader.tryRead(TezReadOnlyString.MDID, out int mdid))
            {
                mdid = -1;
            }

            if (mdid > 0)
            {
                result = new TezRuntimeItemInfo(reader, fdid, mdid);
            }
            else
            {
                result = new TezFixedItemInfo(reader, fdid, mdid);
            }

            return result;
        }
    }

    /// <summary>
    /// 固定数据库信息
    /// 此类只会在加载数据库文件时生成
    /// 运行时和去读玩家存档不会生成此类
    /// </summary>
    public class TezFixedItemInfo : TezBaseItemInfo
    {
        public TezFixedItemInfo(TezReader dataReader, int FDID, int MDID)
        {
            mDataReader = dataReader;
            mReadOnly = dataReader.readBool(TezReadOnlyString.ReadOnly);
            if (mReadOnly)
            {
                //生成一个模板
                mTemplate = TezcatFramework.classFactory.create<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
                mTemplate.init(dataReader);
            }
            else
            {
                //生成一个创建器
                mCreator = TezcatFramework.classFactory.getCreator<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
            }

            this.initData(dataReader.readString(TezReadOnlyString.NID),
                          dataReader.readInt(TezReadOnlyString.StackCount),
                          FDID,
                          MDID);
        }

        public override void close()
        {

        }

        public override int nextVersion()
        {
            return mVersion++;
        }

        public override void updateVersion(int version)
        {
            if (mVersion <= version)
            {
                mVersion = version + 1;
            }
        }

        public override TezRuntimeItemInfo remodify()
        {
            return new TezRuntimeItemInfo(this);
        }

        public override void addRef()
        {

        }
    }

    /// <summary>
    /// 运行时自定义Item的信息
    /// 
    /// 如果是从文件加载的保存数据,reader不为空
    /// 如果是运行时生成的,reader为空
    /// 
    /// 此类的父类可能是ItemInfo
    /// 也可能是MItemInfo
    /// </summary>
    public class TezRuntimeItemInfo : TezBaseItemInfo
    {
        private TezFixedItemInfo mParent = null;
        private int mModifiedRefCount = 0;
        /// <summary>
        /// 重定义引用
        /// </summary>
        public int refModifiedCount => mModifiedRefCount;

        /// <summary>
        /// 通过文件数据加载
        /// 此时并不知道他的
        /// </summary>
        public TezRuntimeItemInfo(TezReader dataReader, int FDID, int MDID)
        {
            mDataReader = dataReader;
            mVersion = dataReader.readInt(TezReadOnlyString.VID);
            this.initData(dataReader.readString(TezReadOnlyString.NID),
                          dataReader.readInt(TezReadOnlyString.StackCount),
                          FDID,
                          MDID);

            mParent = TezcatFramework.fileDB.getItemInfoByFDID(FDID);
            mParent.updateVersion(mVersion);
        }

        /// <summary>
        /// 由RuntimItemInfo生成RuntimItemInfo时调用
        /// 此时此类没有datareader
        /// </summary>
        public TezRuntimeItemInfo(TezFixedItemInfo source)
        {
            mParent = source;
            this.init();
        }

        /// <summary>
        /// 由RuntimItemInfo生成新的RuntimeItemInfo时调用
        /// 此时此类没有datareader
        /// </summary>
        public TezRuntimeItemInfo(TezRuntimeItemInfo source)
        {
            mParent = TezcatFramework.fileDB.getItemInfoByFDID(source.itemID.fixedID);
            this.init();
        }

        private void init()
        {
            mVersion = mParent.nextVersion();
            mReadOnly = mParent.isReadOnly;
            if (mReadOnly)
            {
                mTemplate = TezcatFramework.classFactory.create<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
                mTemplate.init(dataReader);
            }
            else
            {
                mCreator = TezcatFramework.classFactory.getCreator<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
            }
            this.initData($"{mParent.NID}_M{mVersion}",
                          mParent.stackCount,
                          mParent.itemID.fixedID,
                          TezcatFramework.fileDB.generateMDID());
        }

        public override TezRuntimeItemInfo remodify()
        {
            return new TezRuntimeItemInfo(this);
        }

        public override int nextVersion()
        {
            return mParent.nextVersion();
        }

        public override void close()
        {
            mModifiedRefCount--;
            if (mModifiedRefCount <= 0)
            {
                TezcatFramework.fileDB.unregisterItem(mItemID.modifiedID);

                mItemID.close();
                mItemID = null;
                mTemplate = null;
                this.NID = null;
            }
        }

        public override void updateVersion(int version)
        {

        }

        public override void addRef()
        {
            mModifiedRefCount++;
        }
    }

    public class TezErrorItemInfo : TezBaseItemInfo
    {
        public TezErrorItemInfo()
            : base()
        {

        }

        public override void close()
        {

        }

        public override TezRuntimeItemInfo remodify()
        {
            throw new NotImplementedException("?Brain?");
        }

        public override void addRef()
        {
            throw new NotImplementedException("?Brain?");
        }

        public override int nextVersion()
        {
            throw new NotImplementedException();
        }

        public override void updateVersion(int version)
        {
            throw new NotImplementedException();
        }
    }
}