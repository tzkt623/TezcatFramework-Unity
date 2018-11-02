using System;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public interface ITezModifiedOrder : ITezEnumeration
    {

    }

    public abstract class TezModifiedOrder<TSelf, TModifiedOrder>
        : TezEnumeration<TSelf, TModifiedOrder>
        , ITezModifiedOrder
        where TSelf : TezModifiedOrder<TSelf, TModifiedOrder>
        where TModifiedOrder : struct, IComparable
    {
        protected TezModifiedOrder(TModifiedOrder value) : base(value)
        {
        }
    }

    public abstract class TezValueModifier
    {
        public int order
        {
            get { return this.modifiedOrder.toID; }
        }

        public enum ParamType
        {
            Float,
            Int
        }

        public object sourceObject { get; }
        public ITezModifiedOrder modifiedOrder { get; }
        public ParamType paramType { get; }

        public TezValueModifier(ITezModifiedOrder type, ParamType param_type, object source_object)
        {
            this.modifiedOrder = type;
            this.sourceObject = source_object;
            this.paramType = param_type;
        }
    }

    public class TezValueModifier<TValue, TOrder>
        : TezValueModifier
        where TOrder : ITezModifiedOrder
    {
        public readonly TValue value;

        public TezValueModifier(TValue value, TOrder order, ParamType param_type, object source_object)
            : base(order, param_type, source_object)
        {
            this.value = value;
        }
    }
}