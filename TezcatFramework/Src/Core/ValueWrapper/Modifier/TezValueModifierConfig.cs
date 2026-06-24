namespace tezcat.Framework.Core
{
    /// <summary>
    /// 组装配置
    /// 0-3已被占用
    /// 自定义请从BuildInEnd号索引+1开始
    /// 最大支持256个
    /// </summary>
    public class TezValueModifierAssembleConfig
    {
        /// <summary>
        /// 错误值
        /// </summary>
        public const byte Error = 0;

        /// <summary>
        /// 在基础值上增加
        /// </summary>
        public const byte SumBase = 1;

        /// <summary>
        /// 在全局值上增加
        /// </summary>
        public const byte SumTotal = 2;

        /// <summary>
        /// 在基础值上乘算
        /// </summary>
        public const byte MultBase = 3;

        /// <summary>
        /// 在全局值上乘算
        /// </summary>
        public const byte MultTotal = 4;

        /// <summary>
        /// 系统预留位置
        /// </summary>
        public const byte BuildInHold = 64;
    }

    /// <summary>
    /// 数值型Modifeir配置信息
    /// </summary>
    public class TezValueModifierConfig : ITezCloseable
    {
        /// <summary>
        /// 组装方式
        /// </summary>
        public byte assemble = TezValueModifierAssembleConfig.Error;

        /// <summary>
        /// 目标属性
        /// </summary>
        public ITezValueDescriptor target = null;

        public void close()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            this.target = null;
        }

        public override string ToString()
        {
            return string.Format("Assemble:{0}\nTarget:{1}\nPath:\n**********\n{2}\n**********"
                , this.assemble
                , this.target.name
                , base.ToString());
        }
    }
}