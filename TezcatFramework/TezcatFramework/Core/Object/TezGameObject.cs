namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础游戏对象
    /// 
    /// 所有游戏对象都从此类派生
    /// 
    /// 如果没有Category则为null
    /// </summary>
    public abstract class TezGameObject : TezBaseObject
    {
        bool mIsInited = false;

        /// <summary>
        /// 以当前的对象数据生成对象
        /// </summary>
        public void init()
        {
            if (!mIsInited)
            {
                mIsInited = true;
                this.preInit();
                this.onInit();
                this.postInit();
            }
        }

        /// <summary>
        /// 在初始化之前做点什么
        /// </summary>
        protected virtual void preInit()
        {

        }

        /// <summary>
        /// 初始化默认对象
        /// </summary>
        protected virtual void onInit()
        {

        }

        /// <summary>
        /// 初始化之后做点什么
        /// </summary>
        protected virtual void postInit()
        {

        }


        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="reader"></param>
        public override void deserialize(TezReader reader)
        {

        }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="writer"></param>
        public override void serialize(TezWriter writer)
        {
            writer.write(TezBuildInName.CID, this.CID);
        }

        protected override void onClose()
        {

        }
    }
}