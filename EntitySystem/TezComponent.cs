using System;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        int ComID { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
        void onOtherComponentAdded(ITezComponent component, int com_id);
        void onOtherComponentRemoved(ITezComponent component, int com_id);
    }

    public struct TezComponentID<Component> where Component : ITezComponent
    {
        public static int ID { get; private set; } = TezTypeInfo.ErrorID;

        public static void setID(int com_id)
        {
            if(ID != TezTypeInfo.ErrorID)
            {
                throw new Exception(string.Format("{0} this type has initialized", typeof(Component).Name));
            }

            ID = com_id;
        }

        public static bool sameAsID(int com_id)
        {
            if (ID == TezTypeInfo.ErrorID)
            {
                throw new Exception(string.Format("{0} this type is not a ID Getter", typeof(Component).Name));
            }

            return ID == com_id;
        }
    }

    public class TezComponentManager
    {
        public static int componentCount { get; private set; }

        public static int register<Component>() where Component : ITezComponent
        {
            if (TezComponentID<Component>.ID == TezTypeInfo.ErrorID)
            {
                var id = componentCount++;
                TezComponentID<Component>.setID(id);
                return id;
            }

            return -1;
        }
    }
}