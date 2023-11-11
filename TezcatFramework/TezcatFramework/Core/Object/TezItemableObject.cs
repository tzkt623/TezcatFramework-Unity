using tezcat.Framework.Database;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.Core
{
    /*
     * 游戏对象设计
     * 
     * 1.共享对象
     *   共享对象拥有唯一一个实体数据
     *   此类对象在复制时只需要复制引用
     * 2.唯一对象
     *   唯一对象每一个都是一个实体数据
     *   此类对象生在复制时需要生成一个新对象出来
     *   
     * 游戏接口
     * 1.共享对象接口
     * 2.唯一对象接口
     * 
     * 
     * 物品栏的比较不应该直接与基础对象绑定
     * 应该利用物品接口进行存储,比较,过滤等
     * 
     * 
     */

    /// <summary>
    /// 可装入数据库的游戏对象
    /// </summary>
    public abstract class TezItemableObject
        : TezGameObject
        , ITezInventoryItem
    {
        protected TezGameItemInfo mItemInfo = null;
        public TezGameItemInfo itemInfo => mItemInfo;

        /// <summary>
        /// 物品ID
        /// </summary>
        public TezItemID itemID => mItemInfo.itemID;

        /// <summary>
        /// 是否是共享对象
        /// </summary>
        public bool isShared => mItemInfo.stackCount > 0;

        /// <summary>
        /// 模板物品
        /// </summary>
        public bool isPrototype => mItemInfo.isPrototype(this);

        public void init(TezItemableObject template, TezGameItemInfo itemInfo)
        {
            mItemInfo = itemInfo;
            mCategory = template.category;

            this.preInit();
            this.initWithTemplate(template);
            this.postInit();
        }

        /// <summary>
        /// 使用模板对象初始化
        /// </summary>
        public void init(TezItemableObject template)
        {
            mItemInfo = itemInfo;
            mCategory = template.category;

            this.preInit();
            this.initWithTemplate(template);
            this.postInit();
        }

        protected virtual void initWithTemplate(TezItemableObject template)
        {

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
            mItemInfo.close();
            mItemInfo = null;
        }

        /// <summary>
        /// 向数据库保存数据
        /// </summary>
        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            var item_info = this.itemInfo;
            writer.write(TezBuildInName.NameID, item_info.NID);
            writer.write(TezBuildInName.StackCount, item_info.stackCount);

            writer.write(TezBuildInName.FixedID, this.itemID.fixedID);
            writer.write(TezBuildInName.ModifiedID, this.itemID.modifiedID);
        }

        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);

            reader.beginObject("ItemInfo");

            int fdid = reader.readInt(TezBuildInName.FixedID);
            if (!reader.tryRead(TezBuildInName.ModifiedID, out int mdid))
            {
                mdid = -1;
            }

            mItemInfo = new TezGameItemInfo
                ( reader.readString(TezBuildInName.NameID)
                , reader.readInt(TezBuildInName.StackCount)
                , fdid
                , mdid);
            mItemInfo.setPrototype(this);

            reader.endObject("ItemInfo");
        }

        /// <summary>
        /// 双份数据
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
        public abstract TezItemableObject duplicate();
    }

    /// <summary>
    /// <para> 共享对象
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
    /// <para>
    /// 例如,Torpedo类(Unique)包含一个TorpedoData(ReadOnly)作为共享数据
    /// Torpedo本身单独拥有health属性进行血量计算
    /// </para>
    /// 
    /// </summary>
    public abstract class TezSharedObject : TezItemableObject
    {
        public sealed override TezItemableObject duplicate()
        {
            mItemInfo.addRef();
            return this.share();
        }

        protected virtual TezItemableObject share()
        {
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
    /// <para>
    /// 例如,EVE里的舰船
    /// 乌鸦级的数据都是一样的
    /// 但是每个玩家驾驶的乌鸦,血量单独计算(同一个数据的副本)
    /// 有一万个玩家就要new一万个对象
    /// </para>
    /// 
    /// </summary>
    public abstract class TezUniqueObject : TezItemableObject
    {
        public sealed override TezItemableObject duplicate()
        {
            var newinfo = mItemInfo.remodify();

            var obj = this.copy();
            obj.init(this, newinfo);

            newinfo.setPrototype(obj);

            return obj;
        }

        /// <summary>
        /// 复制一个当前对象
        /// </summary>
        protected abstract TezItemableObject copy();
    }
}