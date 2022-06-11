using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
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
}