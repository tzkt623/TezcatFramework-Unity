using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    public enum TezBonusTokenType
    {
        Error = -1,
        /// <summary>
        /// 根
        /// </summary>
        Root,
        /// <summary>
        /// 路径
        /// </summary>
        Path,
        /// <summary>
        /// 叶子
        /// </summary>
        Leaf
    }

    public interface ITezBonusToken : ITezNonCloseable
    {
        /// <summary>
        /// Token类型
        /// </summary>
        TezBonusTokenType tokenType { get; }

        /// <summary>
        /// ID
        /// </summary>
        int tokenID { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string tokenName { get; }

        /// <summary>
        /// 层级
        /// 只在Primary中生效
        /// </summary>
        int layer { get; }

        /// <summary>
        /// 父节点
        /// </summary>
        ITezBonusToken parent { get; }
    }
}