using System;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
    public abstract class TezBaseItemInfo : ITezCloseable
    {
        protected TezItemID mItemID = TezItemID.EmptyID;
        public TezItemID itemID => mItemID;

        TezReader mDataReader = null;
        public TezReader dataReader => mDataReader;

        public string NID { get; set; } = "$ErrorItem$";

        bool mReadOnly = false;

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

        protected TezItemableObject mTemplate = null;
        TezClassFactory.Creator<TezItemableObject> mCreator = null;

        protected int mVersion = 0;
        public int version => mVersion;

        protected TezBaseItemInfo() { }

        public TezBaseItemInfo(TezReader dataReader)
        {
            mDataReader = dataReader;
            this.NID = dataReader.readString(TezReadOnlyString.NID);
            this.stackCount = dataReader.readInt(TezReadOnlyString.StackCount);
            TezItemID.create(ref mItemID,
                             dataReader.readInt(TezReadOnlyString.FDID),
                             dataReader.readInt(TezReadOnlyString.MDID));

            mReadOnly = dataReader.readBool(TezReadOnlyString.ReadOnly);
            if (mReadOnly)
            {
                mTemplate = TezcatFramework.classFactory.create<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
                mTemplate.init(dataReader);
            }
            else
            {
                mCreator = TezcatFramework.classFactory.getCreator<TezItemableObject>(dataReader.readString(TezReadOnlyString.CID));
            }
        }

        public bool isTemplate(TezItemableObject other)
        {
            return object.ReferenceEquals(mTemplate, other);
        }

        public override int GetHashCode()
        {
            return mItemID.GetHashCode();
        }

        public void setTemplate(TezItemableObject result)
        {
            mTemplate = result;
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

        public virtual void close()
        {

        }

        public abstract TezMItemInfo remodify();

        /// <summary>
        /// 记录MDID次数
        /// </summary>
        public abstract void retainModifiedRef();

        public static bool isError(TezBaseItemInfo info)
        {
            return info.itemID.fixedID == -1;
        }


        public static TezBaseItemInfo createItemInfo(TezReader reader)
        {
            TezBaseItemInfo result;
            var mid = reader.readInt(TezReadOnlyString.MDID);
            if (mid >= 0)
            {
                result = new TezMItemInfo(reader);
            }
            else
            {
                result = new TezItemInfo(reader);
            }

            return result;
        }
    }

    /// <summary>
    /// 固定数据库信息
    /// </summary>
    public class TezItemInfo : TezBaseItemInfo
    {
        public TezItemInfo(TezReader dataReader)
            : base(dataReader)
        {

        }

        //         public TezItemInfo(TezItemableObject template, string NID, int stackCount, int FDID)
        //         {
        //             mTemplate = template;
        //             this.NID = NID;
        //             this.stackCount = stackCount;
        // 
        //             mItemID.close();
        //             mItemID = TezItemID.copyFrom(FDID);
        //         }

        /// <summary>
        /// 使用另一个物品对象进行初始化
        /// 用于运行时自定义物品的生成
        /// </summary>
        //         public TezItemInfo(TezItemInfo source)
        //         {
        //             //克隆出信息
        //             //并且生成新的ItemID
        //             this.NID = source.NID;
        //             this.stackCount = source.stackCount;
        // 
        //             mItemID.close();
        //             mItemID = TezItemID.copyFrom(source.itemID.fixedID, TezcatFramework.rtDB.generateID());
        //         }

        public override void close()
        {

        }

        public int nextVersion()
        {
            return mVersion++;
        }

        public override TezMItemInfo remodify()
        {
            return new TezMItemInfo(this);
        }

        public sealed override void retainModifiedRef()
        {
        }
    }

    /// <summary>
    /// 自定义Item的信息
    /// </summary>
    public class TezMItemInfo : TezBaseItemInfo
    {
        /// <summary>
        /// MItemInfo的Parent永远都是ItemInfo
        /// 因为MItem是由Item生成的
        /// 所以只要一直传递模板就行了
        /// </summary>
        private TezItemInfo mParent = null;

        private int mModifiedRefCount = 0;
        /// <summary>
        /// 重定义引用
        /// </summary>
        public int refModifiedCount => mModifiedRefCount;

        public TezMItemInfo(TezReader dataReader)
            : base(dataReader)
        {

        }

        /// <summary>
        /// 由ItemInfo生成MItemInfo时调用
        /// </summary>
        public TezMItemInfo(TezItemInfo source)
            : base(source.dataReader)
        {
            mParent = source;
            this.init();
        }

        /// <summary>
        /// 由MItemInfo生成MItemInfo时调用
        /// </summary>
        public TezMItemInfo(TezMItemInfo source)
            : base(source.dataReader)
        {
            mParent = source.mParent;
            this.init();
        }

        private void init()
        {
            mVersion = mParent.nextVersion();

            this.stackCount = mParent.stackCount;
            this.NID = $"{mParent.NID}_M{mVersion}";
            TezItemID.create(ref mItemID,
                             mParent.itemID.fixedID,
                             TezcatFramework.fileDB.generateMDID());
        }

        public override TezMItemInfo remodify()
        {
            return new TezMItemInfo(this);
        }

        public override void retainModifiedRef()
        {
            mModifiedRefCount++;
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
                mParent = null;
                this.NID = null;
            }
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

        public override TezMItemInfo remodify()
        {
            throw new NotImplementedException("?Brain?");
        }

        public override void retainModifiedRef()
        {
            throw new NotImplementedException("?Brain?");
        }
    }
}