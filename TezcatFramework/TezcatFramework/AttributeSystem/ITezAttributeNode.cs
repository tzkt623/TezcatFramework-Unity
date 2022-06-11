using tezcat.Framework.Core;

namespace tezcat.Framework.Attribute
{
    public enum TezAttributeNodeType
    {
        Error = -1,
        Root,
        Path,
        Leaf
    }

    public interface ITezAttributeNode : ITezCloseable
    {
        TezAttributeNodeType nodeType { get; }
    }
}