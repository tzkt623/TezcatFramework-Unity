using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
        void onOtherComponentAdded(ITezComponent component, int com_id);
        void onOtherComponentRemoved(ITezComponent component, int com_id);
    }

    public class TezComponentID<Component>
        : TezTypeInfo<Component, TezComponentManager>
        where Component : ITezComponent
    {

    }

    public class TezComponentManager
    {
        public static int componentCount { get; private set; }

        public static void register<Component>() where Component : ITezComponent
        {
            if (TezComponentID<Component>.ID == TezTypeInfo.ErrorID)
            {
                TezComponentID<Component>.setID(componentCount++);
            }
        }
    }
}