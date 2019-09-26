using System;
using tezcat.Framework.Core;
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

    public interface ITezDefinitionToken : ITezCloseable
    {
        TezDefinitionTokenType tokenType { get; }
        int tokenID { get; }
        string tokenName { get; }
    }

    public abstract class TezDefinitionToken<TEnumeration, TEnumValue>
        : TezEnumeration<TEnumeration, TEnumValue>
        where TEnumeration : TezEnumeration<TEnumeration, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        public TezDefinitionTokenType tokenType { get; }

        protected TezDefinitionToken(TEnumValue value, TezDefinitionTokenType token_type) : base(value)
        {
            this.tokenType = token_type;
        }
    }
}

