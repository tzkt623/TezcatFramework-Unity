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

    public class TezComponentID<Component>
        : TezTypeInfo<Component, TezComponentManager>
        where Component : ITezComponent
    {
        public static bool sameAsID(int com_id)
        {
            if(ID == -1)
            {
                throw new Exception(string.Format("{0} is type is not a ID Getter", typeof(Component).Name));
            }

            return ID == com_id;
        }

        void test()
        {

        }
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

    public struct TezCID<Component> where Component : ITezComponent
    {
        public static int ParentID { get; private set; }

        public static void setParentID(int id)
        {
            ParentID = id;
        }
    }
}