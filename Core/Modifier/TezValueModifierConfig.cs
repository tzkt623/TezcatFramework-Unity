using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public class TezValueModifierConfig : ITezCloseable
    {
        /// <summary>
        /// 值加成方式
        /// </summary>
        public enum Assemble : byte
        {
            /// <summary>
            /// 在基础值上增加
            /// </summary>
            SumBase = 0,
            /// <summary>
            /// 在全局值上增加
            /// </summary>
            SumTotal,
            /// <summary>
            /// 在基础值上乘算
            /// </summary>
            MultBase,
            /// <summary>
            /// 在全局值上乘算
            /// </summary>
            MultTotal
        }

        /// <summary>
        /// 组装方式
        /// </summary>
        public Assemble assemble { get; set; } = Assemble.SumBase;

        /// <summary>
        /// 目标属性
        /// </summary>
        public ITezValueDescriptor target { get; set; } = null;

        public virtual void close()
        {
            this.target = null;
        }

        public override string ToString()
        {
            return string.Format("Assemble:{0}\nTarget:{1}\nPath:\n**********\n{2}\n**********"
                , this.assemble
                , target.name
                , base.ToString());
        }
    }
}