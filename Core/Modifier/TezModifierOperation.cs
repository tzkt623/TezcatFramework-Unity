using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public interface ITezModifierOperation : ITezEnumeration
    {

    }

    public abstract class TezModifierOperation<TSelf, TModifierOperation>
        : TezEnumeration<TSelf, TModifierOperation>
        , ITezModifierOperation
        where TSelf : TezModifierOperation<TSelf, TModifierOperation>
        where TModifierOperation : struct, IComparable
    {
        protected TezModifierOperation(TModifierOperation value) : base(value)
        {
        }
    }
}