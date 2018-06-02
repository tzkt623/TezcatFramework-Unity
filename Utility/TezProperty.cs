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
        Bool,
        Int,
        Limit_Int,
        Range_Int,
        Float,
        Limit_Float,
        Range_Float,
        String,
        List,
        HashSet,
        Dictionary,
        Class,
        Getter,
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

    #region 通用Value
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

        public T value { get; set; } = default(T);

        public override void clear()
        {
            base.clear();
            value = default(T);
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
            bpf.invoke(value);
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
            ipf.invoke(value);
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
            fpf.invoke(value);
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
            fpf.invoke(value);
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
    #endregion

    #region 集合Value
    public class TezPV_List<Item> : TezPropertyValue<List<Item>>
    {
        public TezPV_List(TezPropertyName name, bool init = false) : base(name)
        {
            if (init)
            {
                value = new List<Item>();
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
                value = new HashSet<Item>();
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
                value = new Dictionary<Key, Value>();
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

    public class TezPV_Class<T> : TezPropertyValue<T> where T : class, new()
    {
        public TezPV_Class(TezPropertyName name, bool init = false) : base(name)
        {
            if(init)
            {
                value = new T();
            }
        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Class;
        }
    }
    #endregion

    #region 特殊Value
    public class TezPV_Getter<Return> : TezPropertyValue
    {
        TezEventBus.Function<Return> m_Getter = null;
        public TezEventBus.Function<Return> getter
        {
            set { m_Getter = value; }
        }

        public Return value
        {
            get { return m_Getter(); }
        }

        public override Type propertyType
        {
            get { return typeof(Return); }
        }

        public TezPV_Getter(TezPropertyName name) : base(name)
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Getter;
        }

        public override void clear()
        {
            base.clear();
            m_Getter = null;
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
    #endregion

    #region 范围Value
    public abstract class TezPropertyRangeValue<T> : TezPropertyValue<T>
    {
        public TezPropertyRangeValue(TezPropertyName name) : base(name)
        {

        }

        public TezPropertyRangeValue()
        {

        }

        public override Type propertyType
        {
            get { return typeof(T); }
        }

        public T min { get; set; }
        public T max { get; set; }

        public abstract bool inRange();
    }

    public class TezPV_RangeInt : TezPropertyRangeValue<int>
    {
        public TezPV_RangeInt(TezPropertyName name) : base(name)
        {

        }

        public TezPV_RangeInt()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Range_Int;
        }

        public override bool inRange()
        {
            return value >= min && value <= max;
        }
    }

    public class TezPV_RangeFloat : TezPropertyRangeValue<float>
    {
        public TezPV_RangeFloat(TezPropertyName name) : base(name)
        {

        }

        public TezPV_RangeFloat()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Range_Float;
        }

        public override bool inRange()
        {
            return value >= min && value <= max;
        }
    }
    #endregion

    #region 限制Value
    public abstract class TezPropertyLimitValue<T> : TezPropertyValue<T>
    {
        public TezPropertyLimitValue(TezPropertyName name) : base(name)
        {

        }

        public TezPropertyLimitValue()
        {

        }

        public override Type propertyType
        {
            get { return typeof(T); }
        }

        public T limit { get; set; }
    }

    public class TezPV_LimitInt : TezPropertyLimitValue<int>
    {
        public TezPV_LimitInt(TezPropertyName name) : base(name)
        {

        }

        public TezPV_LimitInt()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Limit_Int;
        }
    }

    public class TezPV_LimitFloat : TezPropertyLimitValue<float>
    {
        public TezPV_LimitFloat(TezPropertyName name) : base(name)
        {

        }

        public TezPV_LimitFloat()
        {

        }

        public override void accept(TezPropertyFunction pf)
        {

        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Limit_Float;
        }
    }
    #endregion
}



