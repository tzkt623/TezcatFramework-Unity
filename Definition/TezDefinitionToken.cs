using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Definition
{
    public enum TezDefinitionTokenType
    {
        Error = -1,
        Root,
        Path,
        Leaf
    }

    public interface ITezDefinitionToken : ITezEnumeration
    {
        TezDefinitionTokenType tokenType { get; }
    }

    public abstract class TezDefinitionToken<TEnumeration, TEnumValue>
        : TezEnumeration<TEnumeration, TEnumValue>
        , ITezDefinitionToken
        where TEnumeration : TezEnumeration<TEnumeration, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        public abstract TezDefinitionTokenType tokenType { get; }

        protected TezDefinitionToken(TEnumValue value) : base(value)
        {

        }
    }
}

