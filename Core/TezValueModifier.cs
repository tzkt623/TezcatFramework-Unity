using System;
using System.Collections.Generic;
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

    public enum TezModifiedType
    {
        ValueFloat,
        ValueInt,
        Function
    }

    public interface ITezValueModifier
    {
        int order { get; }
        object sourceObject { get; }
        ITezModifiedOrder modifiedOrder { get; }
        TezModifiedType modifiedType { get; }
        ITezValueName valueName { get; }
        ITezValueModifierCombiner createCombiner();
    }

    public abstract class TezValueModifier : ITezValueModifier
    {
        public int order
        {
            get { return this.modifiedOrder.toID; }
        }

        public object sourceObject { get; }
        public ITezModifiedOrder modifiedOrder { get; }
        public TezModifiedType modifiedType { get; }
        public ITezValueName valueName { get; }

        public TezValueModifier(ITezValueName value_name, ITezModifiedOrder type, TezModifiedType modified_type, object source_object)
        {
            this.modifiedOrder = type;
            this.sourceObject = source_object;
            this.modifiedType = modified_type;
            this.valueName = valueName;
        }

        public abstract ITezValueModifierCombiner createCombiner();
    }

    public abstract class TezValueModifier<TValue, TOrder>
        : TezValueModifier
        where TOrder : ITezModifiedOrder
    {
        public TValue value { get; protected set; }

        public TezValueModifier(ITezValueName value_name, TValue value, TOrder order, TezModifiedType modified_type, object source_object)
            : base(value_name, order, modified_type, source_object)
        {
            this.value = value;
        }
    }

    public interface ITezValueModifierCombiner : ITezValueModifier
    {
        bool empty { get; }
        void combine(ITezValueModifier modifier);
        bool separate(ITezValueModifier modifier);
        bool separate(object source);
    }

    public abstract class TezValueModifierCombiner<TValue, TOrder>
        : TezValueModifier<TValue, TOrder>
        , ITezValueModifierCombiner
        where TOrder : ITezModifiedOrder
    {
        List<KeyValuePair<object, ITezValueModifier>> m_CombineOwner = new List<KeyValuePair<object, ITezValueModifier>>();

        public bool empty
        {
            get { return m_CombineOwner.Count == 0; }
        }

        public TezValueModifierCombiner(ITezValueName value_name, TValue value, TOrder order, TezModifiedType modified_type)
            : base(value_name, value, order, modified_type, null)
        {

        }

        public void combine(ITezValueModifier modifier)
        {
            m_CombineOwner.Add(new KeyValuePair<object, ITezValueModifier>(modifier.sourceObject, modifier));
            this.onCombine(modifier);
        }

        protected abstract void onCombine(ITezValueModifier modifier);

        public bool separate(ITezValueModifier modifier)
        {
            var index = m_CombineOwner.FindIndex((KeyValuePair<object, ITezValueModifier> pair) =>
            {
                return pair.Key == modifier.sourceObject;
            });

            if (index >= 0)
            {
                m_CombineOwner.RemoveAt(index);
                this.onSeparate(modifier);
                return true;
            }

            return false;
        }

        protected abstract void onSeparate(ITezValueModifier modifier);

        public bool separate(object source)
        {
            var index = m_CombineOwner.FindIndex((KeyValuePair<object, ITezValueModifier> pair) =>
            {
                return pair.Key == source;
            });

            if (index >= 0)
            {
                var pair = m_CombineOwner[index];
                m_CombineOwner.RemoveAt(index);
                this.onSeparate(pair.Value);
                return true;
            }

            return false;
        }

        public sealed override ITezValueModifierCombiner createCombiner()
        {
            throw new Exception("ModifierCombiner Can not invoke this method!!");
        }
    }
}