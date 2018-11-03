using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public interface ITezModifierOrder : ITezEnumeration
    {

    }

    public abstract class TezModifierOrder<TSelf, TModifiedOrder>
        : TezEnumeration<TSelf, TModifiedOrder>
        , ITezModifierOrder
        where TSelf : TezModifierOrder<TSelf, TModifiedOrder>
        where TModifiedOrder : struct, IComparable
    {
        protected TezModifierOrder(TModifiedOrder value) : base(value)
        {
        }
    }
}