using System;
using System.Reflection;
using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可装入数据库的游戏对象
    /// </summary>
    public abstract class TezItemableObject
        : TezGameObject
    {
        protected TezBaseItemInfo mItemInfo = TezcatFramework.emptyItemInfo;
        public TezBaseItemInfo itemInfo => mItemInfo;
        public TezItemID itemID => mItemInfo.itemID;

        /// <summary>
        /// 模板物品
        /// </summary>
        public bool isTemplate => mItemInfo.isTemplate(this);

        public void init(TezItemableObject template)
        {
            mItemInfo = template.mItemInfo;
            mCategory = template.mCategory;

            this.preInit();
            this.initWithTemplate(template);
            this.postInit();
        }

        protected virtual void initWithTemplate(TezItemableObject template)
        {

        }

        public override void close()
        {
            base.close();
            mItemInfo.close();
            mItemInfo = null;
        }

        /// <summary>
        /// 像数据库保存数据
        /// </summary>
        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            writer.write(TezReadOnlyString.NID, mItemInfo.NID);
            writer.write(TezReadOnlyString.FDID, mItemInfo.itemID.fixedID);
            writer.write(TezReadOnlyString.MDID, mItemInfo.itemID.modifiedID);
            writer.write(TezReadOnlyString.StackCount, mItemInfo.stackCount);
        }

        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
            mItemInfo = new TezItemInfo(this,
                                        reader.readString(TezReadOnlyString.NID),
                                        reader.readInt(TezReadOnlyString.StackCount),
                                        reader.readInt(TezReadOnlyString.FDID),
                                        reader.readInt(TezReadOnlyString.MDID));
        }

        /*
         * #可堆叠物品的运行时重定义
         * 
         * 1.先复制一份模板
         * 2.进行数据的更改
         * 3.注册到运行时数据库当中
         * =======================
         * 1.可堆叠物品使用的都是同一个数据的引用
         * 2.让模板拷贝一份自己
         */
        /// <summary>
        /// 以此对象为模板重新生成新定义对象
        /// </summary>
        public TezItemableObject modifyByThis()
        {
            TezItemableObject result = this.cloneThisObject(mItemInfo);

            //生成新的模板信息数据
            var new_info = mItemInfo.remodify();
            //设定新模板
            new_info.template = result;
            new_info.retainModifiedRef();
            //关闭前一个物品信息
            mItemInfo.close();
            //重定位当前类物品信息
            mItemInfo = new_info;
            //注册
            TezcatFramework.rtDB.registerItem(new_info);

            return result;
        }

        /// <summary>
        /// 复制一份数据
        /// </summary>
        protected abstract TezItemableObject cloneThisObject(TezBaseItemInfo itemInfo);


        /// <summary>
        /// 复制自己
        /// 或者
        /// 共享自己
        /// </summary>
        public virtual TezItemableObject copyOrShare()
        {
            throw new NotImplementedException($"{this.CID} : {MethodBase.GetCurrentMethod().Name} NotImplemented");
        }
    }

    /// <summary>
    /// 不可复制对象
    /// 每个单位都是使用同一份数据进行共享计算
    /// </summary>
    public abstract class TezNonCopyableObject : TezItemableObject
    {
        public override TezItemableObject copyOrShare()
        {
            mItemInfo.retainModifiedRef();
            return this;
        }

        public override void close()
        {
            base.close();
            mItemInfo.close();
        }
    }

    /// <summary>
    /// 可复制对象
    /// 每个单位都会复制一份数据进行独立计算
    /// </summary>
    public abstract class TezCopyableObject : TezItemableObject
    {

    }
}