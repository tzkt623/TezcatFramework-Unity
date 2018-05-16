using System;

namespace tezcat.Utility
{
    public abstract class TezPropertyFunction
        : IComparable<TezPropertyFunction>
        , ITezListSortItem
    {
        public TezPropertyName name
        {
            get; private set;
        }

        public abstract TezPropertyType parameterType { get; }

        int ITezListSortItem.sortID
        {
            get { return name.ID; }
        }

        public TezPropertyFunction(TezPropertyName name)
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
        TezEventBus.Action<T> m_Function = null;

        public TezPropertyFunctionT(TezPropertyName name) : base(name)
        {

        }

        public void setFunction(TezEventBus.Action<T> function)
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

    public class TezPF_Float : TezPropertyFunctionT<float>
    {
        public TezPF_Float(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Float; }
        }
    }

    public class TezPF_Int : TezPropertyFunctionT<int>
    {
        public TezPF_Int(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Int; }
        }
    }

    public class TezPF_Bool : TezPropertyFunctionT<bool>
    {
        public TezPF_Bool(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Bool; }
        }
    }

    public class TezPF_String : TezPropertyFunctionT<string>
    {
        public TezPF_String(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.String; }
        }
    }
}