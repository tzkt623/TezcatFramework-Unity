using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        int comUID { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
        void onOtherComponentAdded(ITezComponent component, int comID);
        void onOtherComponentRemoved(ITezComponent component, int comID);
    }
}