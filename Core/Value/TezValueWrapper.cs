﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    public enum TezValueType
    {
        Bool,
        Int,
        Float,
        Double,
        String,
        Class,
        IDString,
        Type,
        Unknown
    }

    public enum TezWrapperType
    {
        Normal,
        Property,
        MRProperty,
        Attribute,
        Notification,
        WithMinMax,
        WithBasic,
        GetterSetter,
    }

    public interface ITezValueWrapper : ITezCloseable
    {
        ITezValueDescriptor descriptor { get; set; }
        string name { get; }
        int ID { get; }
        Type systemType { get; }
        TezValueType valueType { get; }
        TezWrapperType wrapperType { get; }
    }

    public abstract class TezValueWrapper
        : ITezValueWrapper
        , IComparable<TezValueWrapper>
        , IEquatable<TezValueWrapper>
        , ITezBinarySearchItem
    {
        static Dictionary<Type, TezValueType> Mapping = new Dictionary<Type, TezValueType>()
        {
            {typeof(bool), TezValueType.Bool },
            {typeof(int), TezValueType.Int },
            {typeof(float), TezValueType.Float },
            {typeof(double), TezValueType.Double },
            {typeof(string), TezValueType.String },
            {typeof(TezIDString), TezValueType.IDString },
        };

        protected class WrapperID<Value> : TezTypeInfo<Value, TezValueWrapper>
        {
            public static readonly TezValueType valueType;

            static WrapperID()
            {
                if (!Mapping.TryGetValue(systemType, out valueType))
                {
                    if (systemType.IsClass)
                    {
                        valueType = TezValueType.Class;
                    }
                    else
                    {
                        valueType = TezValueType.Unknown;
                    }
                }
            }
        }

        public abstract Type systemType { get; }

        public abstract TezValueType valueType { get; }

        public virtual TezWrapperType wrapperType
        {
            get { return TezWrapperType.Normal; }
        }

        public virtual ITezValueDescriptor descriptor { get; set; } = null;

        public string name
        {
            get { return descriptor.name; }
        }

        public int ID
        {
            get { return descriptor.ID; }
        }

        int ITezBinarySearchItem.binaryWeight
        {
            get { return descriptor.ID; }
        }

        public TezValueWrapper(ITezValueDescriptor name)
        {
            this.descriptor = name;
        }

        public TezValueWrapper()
        {

        }

        public bool Equals(TezValueWrapper other)
        {
            return other != null && this.descriptor.Equals(other.descriptor);
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezValueWrapper)obj);
        }

        public int CompareTo(TezValueWrapper other)
        {
            return this.descriptor.CompareTo(other.descriptor);
        }

        public override int GetHashCode()
        {
            return this.descriptor.GetHashCode();
        }

        public virtual void close(bool self_close = true)
        {
            this.descriptor = null;
        }

        public static bool operator ==(TezValueWrapper a, TezValueWrapper b)
        {
            var flag_a = object.ReferenceEquals(a, null);
            var flag_b = object.ReferenceEquals(b, null);

            return (flag_a && flag_b) || (!flag_a && !flag_b && a.descriptor.Equals(b.descriptor));
        }

        public static bool operator !=(TezValueWrapper a, TezValueWrapper b)
        {
            var flag_a = object.ReferenceEquals(a, null);
            var flag_b = object.ReferenceEquals(b, null);

            return (!flag_a || !flag_b) && (flag_a || flag_b || !a.descriptor.Equals(b.descriptor));
        }

        public static bool operator true(TezValueWrapper value)
        {
            return !object.ReferenceEquals(value, null);
        }

        public static bool operator false(TezValueWrapper value)
        {
            return object.ReferenceEquals(value, null);
        }

        public static bool operator !(TezValueWrapper value)
        {
            return object.ReferenceEquals(value, null);
        }
    }

    public class TezValueWrapper<T> : TezValueWrapper
    {
        public sealed override Type systemType
        {
            get { return WrapperID<T>.systemType; }
        }

        public override TezValueType valueType
        {
            get { return WrapperID<T>.valueType; }
        }

        public virtual T value { get; set; }

        public TezValueWrapper(ITezValueDescriptor name) : base(name) { }

        public TezValueWrapper() : base() { }

        public void assign(TezValueWrapper<T> wrapper)
        {
            this.value = wrapper.value;
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);
            this.value = default;
        }

        public static implicit operator T(TezValueWrapper<T> property)
        {
            return property.value;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }

    #region 特殊Value
    public abstract class TezPV_Type : TezValueWrapper
    {
        public abstract TezType baseValue { get; set; }

        public override TezValueType valueType
        {
            get { return TezValueType.Type; }
        }

        public TezPV_Type(ITezValueDescriptor name) : base(name)
        {

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

        public override Type systemType
        {
            get { return typeof(T); }
        }

        public TezPV_Type(ITezValueDescriptor name) : base(name)
        {

        }
    }

    public class TezValueGetterSetter<T> : TezValueWrapper<T>
    {
        public TezEventExtension.Function<T> getter { set; get; } = null;
        public TezEventExtension.Action<T> setter { set; get; } = null;

        public override T value
        {
            get { return getter(); }
            set { setter(value); }
        }

        public override TezWrapperType wrapperType
        {
            get { return TezWrapperType.GetterSetter; }
        }

        public TezValueGetterSetter(ITezValueDescriptor name) : base(name)
        {

        }
    }
    #endregion

    #region 范围Value
    public class TezValueWithMinMax<T> : TezValueWrapper<T>
    {
        public T min { get; set; }
        public T max { get; set; }

        public override TezWrapperType wrapperType
        {
            get { return TezWrapperType.WithMinMax; }
        }

        public TezValueWithMinMax(ITezValueDescriptor name) : base(name)
        {

        }
    }
    #endregion

    #region 基数Value
    public class TezValueWithBasic<T> : TezValueWrapper<T>
    {
        public T basic { get; set; }

        public override TezWrapperType wrapperType
        {
            get { return TezWrapperType.WithBasic; }
        }

        public TezValueWithBasic(ITezValueDescriptor name) : base(name)
        {

        }
    }
    #endregion
}



