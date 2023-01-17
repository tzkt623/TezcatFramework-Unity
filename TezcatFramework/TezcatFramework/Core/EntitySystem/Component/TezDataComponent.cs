using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezDataComponent
        : TezBaseComponent
    {
        /// <summary>
        /// 注册ID
        /// </summary>
        public static int SComUID;
        public sealed override int comUID => SComUID;


        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; protected set; }

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initNew()
        {
            this.preInit();
            this.onInitNew();
            this.postInit();
        }

        /// <summary>
        /// 新建一个白板对象
        /// 不依赖数据模板
        /// </summary>
        protected virtual void onInitNew()
        {

        }

        /// <summary>
        /// 数据初始化之前
        /// </summary>
        protected virtual void preInit()
        {

        }

        /// <summary>
        /// 数据初始化之后
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            this.NID = null;
        }
    }
}