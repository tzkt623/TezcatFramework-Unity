using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    public abstract class TezPropertyFunction
        : IComparable<TezPropertyFunction>
        , ITezBinarySearchItem
    {
        public TezValueWrapper name
        {
            get; private set;
        }

        public abstract TezValueType parameterType { get; }

        int ITezBinarySearchItem.binaryID
        {
            get { return name.ID; }
        }

        public TezPropertyFunction(TezValueWrapper name)
        {
            this.name = name;
        }

        public virtual void clear()
        {
            this.name = null;
        }

        public int CompareTo(TezPropertyFunction other)
        {
            return this.name.CompareTo(other.name);
        }
    }

    public abstract class TezPropertyFunctionT<T> : TezPropertyFunction
    {
        TezEventExtension.Action<T> m_Function = null;

        public TezPropertyFunctionT(TezValueWrapper name) : base(name)
        {

        }

        public void setFunction(TezEventExtension.Action<T> function)
        {
            m_Function = function;
        }

        public void invoke(T value)
        {
            m_Function(value);
        }

        public override void clear()
        {
            base.clear();
            m_Function = null;
        }
    }
}