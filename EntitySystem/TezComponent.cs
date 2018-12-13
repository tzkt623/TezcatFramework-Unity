using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
    }

    public class TezComponentID<Component>
        : TezTypeInfo<Component, TezComponentManager>
        where Component : ITezComponent
    {

    }

    public class TezComponentManager : ITezService
    {
        public int componentCount { get; private set; }

        public void register<Component>() where Component : ITezComponent
        {
            if (TezComponentID<Component>.ID == TezTypeInfo.ErrorID)
            {
                TezComponentID<Component>.setID(componentCount++);
            }
        }

        public void close()
        {

        }
    }
}