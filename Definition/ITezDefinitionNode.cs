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

    public interface ITezDefinitionPathWithObject
    {
        TezDefinitionPath definitionPath { get; }
    }
}