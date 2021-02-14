using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Framework.ECS
{
    public interface ITezComponent : ITezCloseable
    {
        TezEntity entity { get; }
        int UID { get; }
        void onAdd(TezEntity entity);
        void onRemove(TezEntity entity);
        void onOtherComponentAdded(ITezComponent component, int com_id);
        void onOtherComponentRemoved(ITezComponent component, int com_id);
    }

    /// <summary>
    /// Com的ID管理器
    /// </summary>
    public struct TezComponentID<Component> where Component : ITezComponent
    {
        public static int ID { get; private set; } = TezTypeInfo.ErrorID;

        public static void setID(int comID)
        {
            if (ID != TezTypeInfo.ErrorID)
            {
                throw new Exception(string.Format("{0} this type has initialized", typeof(Component).Name));
            }

            ID = comID;
        }

        public static bool sameAsID(int comID)
        {
            if (ID == TezTypeInfo.ErrorID)
            {
                throw new Exception(string.Format("{0} this type is not a ID Getter", typeof(Component).Name));
            }

            return ID == comID;
        }
    }

    public class TezComponentManager
    {
        public static int count { get; private set; }

        public static int register<Component>() where Component : ITezComponent
        {
            if (TezComponentID<Component>.ID == TezTypeInfo.ErrorID)
            {
                var id = count++;
                TezComponentID<Component>.setID(id);
                Debug.Log(string.Format("TezComponentManager : Register {0}-{1}", typeof(Component).Name, id));
                return id;
            }

            throw new Exception(string.Format("TezComponentManager : [{0}] register twice!!", typeof(Component).Name));
        }
    }
}