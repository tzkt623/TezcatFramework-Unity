using tezcat.Framework.Core;

namespace tezcat.Framework.Definition
{
    public enum TezDefinitionTokenType
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

    public interface ITezDefinitionToken : ITezCloseable
    {
        /// <summary>
        /// Token类型
        /// </summary>
        TezDefinitionTokenType tokenType { get; }
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
    }
}