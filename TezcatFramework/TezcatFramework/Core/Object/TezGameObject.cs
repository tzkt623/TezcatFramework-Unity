using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础游戏对象
    /// 
    /// 所有游戏对象都从此类派生
    /// 
    /// 如果没有Category则为null
    /// </summary>
    public abstract class TezGameObject
        : TezBaseObject
    {
        protected TezCategory mCategory = null;
        public TezCategory category => mCategory;

        /// <summary>
        /// 生成默认对象
        /// </summary>
        public void init()
        {
            this.preInit();
            this.initDefault();
            this.postInit();
        }

        /// <summary>
        /// 在初始化之前做点什么
        /// </summary>
        protected virtual void preInit()
        {

        }

        /// <summary>
        /// 初始化之后做点什么
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 初始化默认对象
        /// </summary>
        protected virtual void initDefault()
        {

        }

        public override void deserialize(TezReader reader)
        {
            if(reader.tryRead(TezBuildInName.Category, out string name))
            {
                mCategory = TezCategorySystem.getCategory(name);
            }
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(TezBuildInName.ClassID, this.CID);
            if(mCategory != null)
            {
                writer.write(TezBuildInName.Category, mCategory.name);
            }
        }

        public override void close()
        {
            base.close();
            mCategory = null;
        }
    }
}