using tezcat.Framework.Core;

namespace tezcat.Framework.Attribute
{
    public enum TezAttributeTokenType
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

    public interface ITezAttributeToken : ITezCloseable
    {
        /// <summary>
        /// Token类型
        /// </summary>
        TezAttributeTokenType tokenType { get; }
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