using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    public enum TezBonusTreeNodeType
    {
        Error = -1,
        Path,
        Leaf
    }

    public interface ITezBonusTreeNode : ITezCloseable
    {
        TezBonusTreeNodeType nodeType { get; }
    }
}