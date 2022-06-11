using tezcat.Framework.Core;

namespace tezcat.Framework.Attribute
{
    public interface ITezAttributeTree
        : ITezCloseable
        , ITezAttributeHandler
    {
        TezAttributeNode getPrimaryNode(int id);
        TezAttributeLeaf getSecondaryNode(int id);
    }
}