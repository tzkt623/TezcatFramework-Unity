using System;

namespace tezcat.Utility
{
    public class TezPropertyName
        : IComparable<TezPropertyName>
    {
        public readonly string key_name;
        public readonly int key_id;

        public TezPropertyName(string name, int id)
        {
            key_name = name;
            key_id = id;
        }

        public int CompareTo(TezPropertyName other)
        {
            return key_id.CompareTo(other.key_id);
        }
    }
}