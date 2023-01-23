using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可装入数据库的游戏对象
    /// </summary>
    public abstract class TezItemableObject
        : TezGameObject
    {

        //        protected TezBaseItemInfo mItemInfo = TezcatFramework.emptyItemInfo;
        //        public TezBaseItemInfo itemInfo => mItemInfo;
        public TezItemID mItemID = TezItemID.EmptyID;
        public TezItemID itemID => mItemID;
        public TezBaseItemInfo itemInfo => TezcatFramework.fileDB.getItemInfo(mItemID);
        //         {
        //             get
        //             {
        //                 if (mItemID.modifiedID > -1)
        //                 {
        //                     return TezcatFramework.rtDB.getItem(mItemID.modifiedID);
        //                 }
        //                 else
        //                 {
        //                     return TezcatFramework.mainDB.getItem(mItemID.fixedID);
        //                 }
        //             }
        //         }

        public abstract bool isReadOnly { get; }

        /// <summary>
        /// 模板物品
        /// </summary>
        public bool isTemplate => itemInfo.isTemplate(this);

        /// <summary>
        /// 使用模板对象初始化
        /// </summary>
        public void init(TezItemableObject template)
        {
            this.preInit();
            this.initWithTemplate(template);
            this.postInit();
        }

        protected virtual void initWithTemplate(TezItemableObject template)
        {
            TezItemID.copyFrom(ref mItemID, template.itemID);
            mCategory = template.mCategory;
        }

        /// <summary>
        /// 通过reader中的数据初始化对象
        /// </summary>
        public void init(TezReader reader)
        {
            this.preInit();
            this.initWithFileData(reader);
            this.postInit();
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void initWithFileData(TezReader reader)
        {
            //通过文件数据加载物品数据
            //会出现ItemInfo重复生成的情况
            this.deserialize(reader);
        }

        public override void close()
        {
            base.close();
            mItemID.close();
            mItemID = null;
        }

        /// <summary>
        /// 像数据库保存数据
        /// </summary>
        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            var item_info = this.itemInfo;
            writer.write(TezReadOnlyString.NID, item_info.NID);
            writer.write(TezReadOnlyString.StackCount, item_info.stackCount);
            writer.write(TezReadOnlyString.ReadOnly, this.isReadOnly);

            writer.write(TezReadOnlyString.FDID, mItemID.fixedID);
            writer.write(TezReadOnlyString.MDID, mItemID.modifiedID);
        }

        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);

            if (!reader.tryRead(TezReadOnlyString.MDID, out int mdid))
            {
                mdid = -1;
            }

            TezItemID.create(ref mItemID,
                             reader.readInt(TezReadOnlyString.FDID),
                             mdid);
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
            TezItemableObject result = this.copyThisObject();
            result.initWithTemplate(this);

            var item_info = this.itemInfo;
            //生成新的模板信息数据
            var new_info = item_info.remodify();
            //关闭原本的数据
            item_info.close();

            //设定新模板
            new_info.setTemplate(result);
            new_info.retainModifiedRef();
            //注册
            TezcatFramework.fileDB.registerItem(new_info);

            return result;
        }

        /// <summary>
        /// 复制一个当前对象
        /// </summary>
        /// <returns></returns>
        protected abstract TezItemableObject copyThisObject();

        /// <summary>
        /// 共享数据
        /// 
        /// <para>
        /// 如果是ReadonlyObject会共享此对象的数据
        /// </para>
        /// 
        /// <para>
        /// 如果是UniqueObject会复制此对象的数据
        /// </para>
        /// 
        /// </summary>
        public abstract TezItemableObject share();
    }

    /// <summary>
    /// <para> 只读对象
    /// 每个对象在共享时都是使用同一份共享数据进行计算
    /// 此对象在关闭(close)时不会真正销毁
    /// 因为此类的对象是作为共享对象存在
    /// </para>
    /// 
    /// <para>例如</para>
    /// <para>
    /// 1.异星工厂里的合成材料
    /// 就算有5万个铁
    /// 实际上在内存里只有一份"铁"的数据
    /// 以及一个5万的计数
    /// 而不会真的new5万个对象
    /// </para>
    /// 
    /// <para>
    /// 2.EVE里的子弹--鱼雷
    /// 每一个鱼雷都是同一个数据(伤害,飞行速度,飞行时间)
    /// 但是每一颗鱼雷都是独立的个体,有独立的血量计算,可以被拦截
    /// 所以鱼雷类应该包装一下鱼雷数据类
    /// </para>
    /// 
    /// <para>例如</para>
    /// <para>
    /// 类Torpedo包含一个ReadOnly类TorpedoData作为共享数据
    /// 类Torpedo本身单独拥有health属性进行血量计算
    /// </para>
    /// 
    /// </summary>
    public abstract class TezReadOnlyObject : TezItemableObject
    {
        public sealed override bool isReadOnly => true;

        public sealed override TezItemableObject share()
        {
            return this.shareThisObject();
        }

        protected virtual TezItemableObject shareThisObject()
        {
            this.itemInfo.retainModifiedRef();
            return this;
        }
    }

    /// <summary>
    /// 
    /// <para>
    /// 唯一对象(ReadWrite)
    /// 运行时唯一数据对象
    /// 每个对象在共享时都会复制一份独立数据进行计算
    /// </para>
    /// 
    /// <para>
    /// 此对象在关闭(close)时会真正的销毁自己
    /// 因为每一个此类对象都是一个独立的个体
    /// </para>
    /// 
    /// <para>例如</para>
    /// <para>
    /// 例如EVE里的舰船
    /// 乌鸦级的数据都是一样的
    /// 但是每个玩家驾驶的乌鸦,血量单独计算(同一个数据的副本)
    /// 有一万个玩家就要new一万个对象
    /// </para>
    /// 
    /// </summary>
    public abstract class TezUniqueObject : TezItemableObject
    {
        public sealed override bool isReadOnly => false;

        public sealed override TezItemableObject share()
        {
            return this.copyThisObject();
        }
    }
}