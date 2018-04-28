using System;
using System.Collections.Generic;
using tezcat.String;

namespace tezcat.Utility
{
    public interface ITezPropertyRegister
    {
        TezPropertyManager propertyRegister { get; }
    }

    public enum TezPropertyType
    {
        T,
        Float,
        Int,
        Bool,
        String,
        List,
        HashSet,
        Dictionary,
        Class,
        Function,
        StaticString
    }

    public abstract class TezPropertyValue
        : ITezListSortItem
        , IComparable<TezPropertyValue>
    {
        public TezPropertyValue(TezPropertyName name)
        {
            this.propertyName = name;
        }

        public TezPropertyValue()
        {

        }

        public abstract Type propertyType { get; }

        public TezPropertyName propertyName { get; private set; } = null;

        public string name { get { return propertyName.key_name; } }
        public int ID { get { return propertyName.key_id; } }

        int ITezListSortItem.sortID
        {
            get { return propertyName.key_id; }
        }

        public abstract TezPropertyType getParameterType();

        public abstract void accept(TezPropertyFunction pf);

        public bool equalTo(TezPropertyValue other)
        {
            return propertyName.key_id == other.propertyName.key_id;
        }

        public virtual void clear()
        {
            propertyName = null;
        }

        int IComparable<TezPropertyValue>.CompareTo(TezPropertyValue other)
        {
            return this.propertyName.CompareTo(other.propertyName);
        }
    }

    public abstract class TezPropertyValue<T> : TezPropertyValue
    {
        public TezPropertyValue(TezPropertyName name) : base(name)
        {

        }

        public TezPropertyValue()
        {

        }

        public override Type propertyType
        {
            get { return typeof(T); }
        }

        protected T m_Value = default(T);
        public T value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public override void clear()
        {
            base.clear();
            m_Value = default(T);
        }
    }

    public class TezPV_T<T> : TezPropertyValue<T>
    {
        public TezPV_T(TezPropertyName name) : base(name)
        {
        }

        public TezPV_T()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            TezPF_T<T> tpf = pf as TezPF_T<T>;
            tpf.invoke(m_Value);
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.T;
        }
    }

    public class TezPV_Bool : TezPropertyValue<bool>
    {
        public TezPV_Bool(TezPropertyName name) : base(name)
        {

        }

        public TezPV_Bool()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            TezPF_Bool bpf = pf as TezPF_Bool;
            bpf.invoke(m_Value);
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Bool;
        }
    }

    public class TezPV_Int : TezPropertyValue<int>
    {
        public TezPV_Int(TezPropertyName name) : base(name)
        {

        }

        public TezPV_Int()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            TezPF_Int ipf = pf as TezPF_Int;
            ipf.invoke(m_Value);
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Int;
        }
    }

    public class TezPV_Float : TezPropertyValue<float>
    {
        public TezPV_Float(TezPropertyName name) : base(name)
        {
        }

        public TezPV_Float()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            TezPF_Float fpf = pf as TezPF_Float;
            fpf.invoke(m_Value);
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Float;
        }
    }

    public class TezPV_String : TezPropertyValue<string>
    {
        public TezPV_String(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            TezPF_String fpf = pf as TezPF_String;
            fpf.invoke(m_Value);
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.String;
        }
    }

    public class TezPV_StaticString : TezPropertyValue
    {
        public override Type propertyType
        {
            get { return typeof(TezStaticString); }
        }

        public TezStaticString value { get; set; } = TezStaticString.empty;

        public TezPV_StaticString(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.StaticString;
        }
    }

    public class TezPV_List<Item> : TezPropertyValue<List<Item>>
    {
        public TezPV_List(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.List;
        }
    }

    public class TezPV_HashSet<Item> : TezPropertyValue<HashSet<Item>>
    {
        public TezPV_HashSet(TezPropertyName name) : base(name)
        {
        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.HashSet;
        }
    }

    public class TezPV_Dictionary<Key, Value> : TezPropertyValue<Dictionary<Key, Value>>
    {
        public TezPV_Dictionary(TezPropertyName name) : base(name)
        {
        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Dictionary;
        }
    }

    public class TezPV_Class<T> : TezPropertyValue<T> where T : class
    {
        public TezPV_Class(TezPropertyName name) : base(name)
        {
        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Class;
        }
    }

    public class TezPV_Function<Return> : TezPropertyValue
    {
        TezEventBus.Function<Return> m_Function = null;
        public TezEventBus.Function<Return> function
        {
            set { m_Function = value; }
        }

        public Return value
        {
            get { return m_Function(); }
        }

        public override Type propertyType
        {
            get { return typeof(Return); }
        }

        public TezPV_Function(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Function;
        }

        public override void clear()
        {
            base.clear();
            m_Function = null;
        }
    }
}



