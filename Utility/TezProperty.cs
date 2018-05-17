using System;
using System.Collections.Generic;
using tezcat.String;
using tezcat.TypeTraits;

namespace tezcat.Utility
{
    public interface ITezPropertyRegister
    {
        TezPropertyManager propertyRegister { get; }
    }

    public enum TezPropertyType
    {
        Float,
        Int,
        Bool,
        String,
        List,
        HashSet,
        Dictionary,
        Class,
        Function,
        StaticString,
        Type
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

        public string name { get { return propertyName.name; } }
        public int ID { get { return propertyName.ID; } }

        int ITezListSortItem.sortID
        {
            get { return propertyName.ID; }
        }

        public abstract TezPropertyType getParameterType();

        public abstract void accept(TezPropertyFunction pf);

        public bool equalTo(TezPropertyValue other)
        {
            return propertyName.ID == other.propertyName.ID;
        }

        public virtual void clear()
        {
            propertyName = null;
        }

        int IComparable<TezPropertyValue>.CompareTo(TezPropertyValue other)
        {
            return this.propertyName.CompareTo(other.propertyName);
        }

        public static bool operator true(TezPropertyValue value)
        {
            return !object.ReferenceEquals(value, null);
        }

        public static bool operator false(TezPropertyValue value)
        {
            return object.ReferenceEquals(value, null);
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

        public TezStaticString value { get; set; } = new TezStaticString();

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
        public TezPV_List(TezPropertyName name, bool init = false) : base(name)
        {
            if (init)
            {
                m_Value = new List<Item>();
            }
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
        public TezPV_HashSet(TezPropertyName name, bool init = false) : base(name)
        {
            if (init)
            {
                m_Value = new HashSet<Item>();
            }
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
        public TezPV_Dictionary(TezPropertyName name, bool init = false) : base(name)
        {
            if (init)
            {
                m_Value = new Dictionary<Key, Value>();
            }
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

    public class TezPV_ReadOnly<Return> : TezPropertyValue
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

        public TezPV_ReadOnly(TezPropertyName name) : base(name)
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

    public abstract class TezPV_Type : TezPropertyValue
    {
        public abstract TezType baseValue { get; set; }

        public TezPV_Type(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {
            throw new NotImplementedException();
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Type;
        }
    }

    public class TezPV_Type<T> : TezPV_Type where T : TezType, new()
    {
        public T value { get; set; }

        public override TezType baseValue
        {
            get { return value; }
            set { this.value = (T)value; }
        }

        public override Type propertyType
        {
            get { return typeof(T); }
        }

        public TezPV_Type(TezPropertyName name) : base(name)
        {

        }
    }
}



