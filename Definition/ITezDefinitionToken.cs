using tezcat.Framework.Core;

namespace tezcat.Framework.Definition
{
    public enum TezDefinitionTokenType
    {
        Error = -1,
        Root,
        Path,
        Leaf
    }

    public interface ITezDefinitionToken : ITezCloseable
    {
        TezDefinitionTokenType tokenType { get; }
        int tokenID { get; }
        string tokenName { get; }
    }
}