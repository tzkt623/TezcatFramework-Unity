using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    public interface ITezBonusTree
        : ITezCloseable
        , ITezBonusObjectHandler
    {
        TezBonusTreeNode getNode(int id);
    }
}