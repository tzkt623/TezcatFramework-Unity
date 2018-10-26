using System;
using tezcat.Framework.Core;


namespace tezcat.Framework.AI
{
    public class TezAICondition
        : ITezCloseable
        , IEquatable<TezAICondition>
        , IComparable<TezAICondition>
    {
        public int ID { get; protected set; } = -1;

        public void close()
        {

        }

        int IComparable<TezAICondition>.CompareTo(TezAICondition other)
        {
            return this.ID.CompareTo(other.ID);
        }

        bool IEquatable<TezAICondition>.Equals(TezAICondition other)
        {
            return this.ID == other.ID;
        }
    }
}