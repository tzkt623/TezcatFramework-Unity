using System;

namespace tezcat.Utility
{
    public class TezPropertyName
        : IComparable<TezPropertyName>
    {
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
    }
}