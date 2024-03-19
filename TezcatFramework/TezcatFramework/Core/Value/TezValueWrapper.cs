using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public enum TezValueType
    {
        Unknown = -1,
        Bool,
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        String,
        StaticString,
        Class,
        Type
    }

    public enum TezWrapperType
    {
        Normal,
        Property,
        MRProperty,
        LitProperty,
        Bonusable,
        Notification,
        WithMinMax,
        WithBasic,
        GetterSetter,
    }

    public interface ITezValueWrapper : ITezCloseable
    {
        Type systemType { get; }
        TezValueType valueType { get; }
        TezWrapperType wrapperType { get; }

        ITezValueDescriptor descriptor { get; set; }
        string name { get; }
        int ID { get; }
        string valueToString();
    }

    public abstract class TezValueWrapper
        : ITezValueWrapper
        , IComparable<TezValueWrapper>
        , IEquatable<TezValueWrapper>
        , ITezBinarySearchItem
    {
        static Dictionary<Type, TezValueType> Mapping = new Dictionary<Type, TezValueType>()
        {
            {typeof(bool),      TezValueType.Bool },
            {typeof(sbyte),     TezValueType.SByte },
            {typeof(byte),      TezValueType.Byte },
            {typeof(short),     TezValueType.Short },
            {typeof(ushort),    TezValueType.UShort },
            {typeof(int),       TezValueType.Int },
            {typeof(uint),      TezValueType.UInt },
            {typeof(long),      TezValueType.Long },
            {typeof(ulong),     TezValueType.ULong },
            {typeof(float),     TezValueType.Float },
            {typeof(double),    TezValueType.Double },
            {typeof(string),    TezValueType.String },
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

        protected ITezValueDescriptor mDescriptor = null;
        public virtual ITezValueDescriptor descriptor
        {
            get => mDescriptor;
            set => mDescriptor = value;
        }

        /// <summary>
        /// Value的名称
        /// </summary>
        public string name
        {
            get { return mDescriptor.name; }
        }

        /// <summary>
        /// Value的ID
        /// </summary>
        public int ID
        {
            get { return mDescriptor.ID; }
        }

        int ITezBinarySearchItem.binaryWeight
        {
            get { return mDescriptor.ID; }
        }

        public TezValueWrapper(ITezValueDescriptor valueDescriptor)
        {
            mDescriptor = valueDescriptor;
        }

        public TezValueWrapper()
        {

        }

        /// <summary>
        /// 等于比较
        /// 会比较两个Wrapper的descriptor的ID是否相同
        /// 比较内存请使用object.ReferenceEquals
        /// </summary>
        public bool Equals(TezValueWrapper other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mDescriptor.Equals(other.mDescriptor);
        }

        public int CompareTo(TezValueWrapper other)
        {
            return mDescriptor.CompareTo(other.mDescriptor);
        }

        /// <summary>
        /// 等于比较
        /// 会比较两个Wrapper的descriptor的ID是否相同
        /// 比较内存请使用object.ReferenceEquals
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals((TezValueWrapper)other);
        }

        public override int GetHashCode()
        {
            return mDescriptor.GetHashCode();
        }

        public virtual void close()
        {
            mDescriptor = null;
        }

        public abstract string valueToString();

        /// <summary>
        /// 等于比较
        /// 会比较两个Wrapper的descriptor的ID是否相同
        /// 比较内存请使用object.ReferenceEquals
        /// </summary>
        public static bool operator ==(TezValueWrapper a, TezValueWrapper b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        /// <summary>
        /// 不等于比较
        /// 会比较两个Wrapper的descriptor的ID是否相同
        /// 比较内存请使用object.ReferenceEquals
        /// </summary>
        public static bool operator !=(TezValueWrapper a, TezValueWrapper b)
        {
            return !(a == b);
        }

        public static bool operator true(TezValueWrapper value)
        {
            return !object.ReferenceEquals(value, null);
        }

        public static bool operator false(TezValueWrapper value)
        {
            return object.ReferenceEquals(value, null);
        }
    }

    /// <summary>
    /// 一种包含唯一ID的值
    /// 可用于制作多种系统
    /// </summary>
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

        public TezValueWrapper(ITezValueDescriptor valueDescriptor) : base(valueDescriptor) { }

        public TezValueWrapper() : base() { }

        public void assign(TezValueWrapper<T> wrapper)
        {
            this.value = wrapper.value;
        }

        public override void close()
        {
            base.close();
            this.value = default;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override string valueToString()
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

        public override string valueToString()
        {
            return this.value.ToString();
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