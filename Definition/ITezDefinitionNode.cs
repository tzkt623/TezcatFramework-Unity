using tezcat.Framework.Core;

namespace tezcat.Framework.Definition
{
    public enum TezDefinitionNodeType
    {
        Error = -1,
        Root,
        Path,
        Leaf
    }

    public interface ITezDefinitionNode : ITezCloseable
    {
        TezDefinitionNodeType nodeType { get; }
    }

    public interface ITezDefinitionPathObject
    {
        TezDefinitionPath definitionPath { get; }
    }
}