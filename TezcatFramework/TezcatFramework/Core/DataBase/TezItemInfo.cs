using System;
using tezcat.Framework.Core;
using UnityEngine.Networking.Types;

namespace tezcat.Framework.Database
{
    public abstract class TezBaseItemInfo : ITezCloseable
    {
        protected TezItemID mItemID = TezItemID.EmptyID;
        public TezItemID itemID => mItemID;

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
        /// ==0 : 物品不可堆叠,物品生成的对象数据独立,必须克隆模板数据
        /// >=1 : 物品可以堆叠,物品生成的对象数据共享,仅对模板数据只读
        /// </summary>
        public int stackCount { get; set; } = 1;

        protected TezItemableObject mTemplate = null;
        /// <summary>
        /// 获得一份模板数据
        /// 此模板数据
        /// 要么是克隆数据
        /// 要么是共享数据
        /// </summary>
        public TezItemableObject template
        {
            set { mTemplate = value; }
            get { return mTemplate.copyOrShare(); }
        }

        protected int mVersion = 0;
        public int version => mVersion;

        public bool isTemplate(TezItemableObject other)
        {
            return object.ReferenceEquals(mTemplate, other);
        }

        public override int GetHashCode()
        {
            return mItemID.GetHashCode();
        }

        public abstract void close();
        public abstract TezMItemInfo remodify();
        public abstract void retainModifiedRef();

        public static bool isError(TezBaseItemInfo info)
        {
            return info.itemID.fixedID == -1;
        }
    }

    /// <summary>
    /// 固定数据库信息
    /// </summary>
    public class TezItemInfo : TezBaseItemInfo
    {
        public TezItemInfo()
        {

        }

        public TezItemInfo(TezItemableObject template, string NID, int stackCount, int FDID, int MDID = -1)
        {
            mTemplate = template;
            this.NID = NID;
            this.stackCount = stackCount;

            mItemID.close();
            mItemID = TezItemID.create(FDID, MDID);
        }

        /// <summary>
        /// 使用另一个物品对象进行初始化
        /// 用于运行时自定义物品的生成
        /// </summary>
        public TezItemInfo(TezItemInfo source)
        {
            //克隆出信息
            //并且生成新的ItemID
            this.NID = source.NID;
            this.stackCount = source.stackCount;

            mItemID.close();
            mItemID = TezItemID.create(source.itemID.fixedID, TezcatFramework.rtDB.generateID());
        }

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

        /// <summary>
        /// 由ItemInfo生成MItemInfo时调用
        /// </summary>
        public TezMItemInfo(TezItemInfo source)
        {
            mParent = source;
            this.init();
        }

        /// <summary>
        /// 由MItemInfo生成MItemInfo时调用
        /// </summary>
        public TezMItemInfo(TezMItemInfo source)
        {
            mParent = source.mParent;
            this.init();
        }

        private void init()
        {
            mVersion = mParent.nextVersion();

            this.stackCount = mParent.stackCount;
            this.NID = $"{mParent.NID}_M{mVersion}";
            mItemID.close();
            mItemID = TezItemID.create(mParent.itemID.fixedID, TezcatFramework.rtDB.generateID());
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
                TezcatFramework.rtDB.unregisterItem(mItemID.modifiedID);

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