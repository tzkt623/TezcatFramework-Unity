using System;

namespace tezcat.Utility
{
    public sealed class TezPropertyName
        : IComparable<TezPropertyName>
        , IEquatable<TezPropertyName>
    {
        public static readonly TezPropertyName name_id = TezPropertyManager.register("name_id");

        public readonly string name;
        public readonly int ID;

        public TezPropertyName(string name, int id)
        {
            this.name = name;
            this.ID = id;
        }

        public int CompareTo(TezPropertyName other)
        {
            return ID.CompareTo(other.ID);
        }

        public bool Equals(TezPropertyName other)
        {
            if(other == null)
            {
                return false;
            }

            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}