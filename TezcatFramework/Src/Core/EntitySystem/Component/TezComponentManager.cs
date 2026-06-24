using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
    public class TezComponentManager
    {
        public static int count { get; private set; }

        public static int register<Component>() where Component : ITezComponent
        {
            if (TezComponentID<Component>.ID == TezTypeInfo.ErrorID)
            {
                var id = count++;
                TezComponentID<Component>.setID(id);
                /*Debug.Log(string.Format("TezComponentManager : Register {0}-{1}", typeof(Component).Name, id));*/
                return id;
            }

            throw new Exception(string.Format("TezComponentManager : [{0}] register twice!!", typeof(Component).Name));
        }
    }
}