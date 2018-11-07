using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.String;
using tezcat.Framework.TypeTraits;

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
        StaticString,
        Type,
        Unknown
    }

    public enum TezValueSubType
    {
        Normal,
        WithMinMax,
        WithBasic,
        WithModifier,
        GetterSetter,
    }

    public interface ITezValueWrapper : ITezCloseable
    {
        ITezValueName valueName { get; }
        string name { get; }
        int ID { get; }
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
            {typeof(TezStaticString), TezValueType.StaticString },
        };

        protected class WrapperID<Value> : TezTypeInfo<Value, TezValueWrapper>
        {
            public static readonly TezValueType valueType;

            static WrapperID()
            {
                if (!Mapping.TryGetValue(typeof(Value), out valueType))
                {
                    if (typeof(Value).IsClass)
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


        public TezValueWrapper(ITezValueName name)
        {
            this.valueName = name;
        }

        public TezValueWrapper()
        {

        }

        public abstract Type systemType { get; }
        public abstract TezValueType valueType { get; }
        public virtual TezValueSubType valueSubType
        {
            get { return TezValueSubType.Normal; }
        }

        public virtual ITezValueName valueName { get; } = null;

        public string name
        {
            get { return valueName.name; }
        }

        public int ID
        {
            get { return valueName.ID; }
        }

        int ITezBinarySearchItem.binaryID
        {
            get { return this.ID; }
        }

        public bool Equals(TezValueWrapper other)
        {
            return this.valueName.Equals(other.valueName);
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezValueWrapper)obj);
        }

        public int CompareTo(TezValueWrapper other)
        {
            return this.valueName.CompareTo(other.valueName);
        }

        public override int GetHashCode()
        {
            return this.valueName.GetHashCode();
        }

        public virtual void close()
        {

        }

        public static bool operator ==(TezValueWrapper a, TezValueWrapper b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TezValueWrapper a, TezValueWrapper b)
        {
            return !a.Equals(b);
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
        public TezValueWrapper(ITezValueName name) : base(name)
        {

        }

        public sealed override Type systemType
        {
            get { return typeof(T); }
        }

        public override TezValueType valueType
        {
            get { return WrapperID<T>.valueType; }
        }

        public virtual T value { get; set; }

        public void assign(TezValueWrapper<T> wrapper)
        {
            this.value = wrapper.value;
        }

        public override void close()
        {
            this.value = default(T);
            base.close();
        }

        public static implicit operator T(TezValueWrapper<T> property)
        {
            return property.value;
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

        public TezPV_Type(TezValueName name) : base(name)
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

        public TezPV_Type(TezValueName name) : base(name)
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

        public override TezValueSubType valueSubType
        {
            get { return TezValueSubType.GetterSetter; }
        }

        public TezValueGetterSetter(TezValueName name) : base(name)
        {

        }
    }
    #endregion

    #region 范围Value
    public class TezValueWithMinMax<T> : TezValueWrapper<T>
    {
        public T min { get; set; }
        public T max { get; set; }

        public override TezValueSubType valueSubType
        {
            get { return TezValueSubType.WithMinMax; }
        }

        public TezValueWithMinMax(TezValueName name) : base(name)
        {

        }
    }
    #endregion

    #region 基数Value
    public class TezValueWithBasic<T> : TezValueWrapper<T>
    {
        public T basic { get; set; }

        public override TezValueSubType valueSubType
        {
            get { return TezValueSubType.WithBasic; }
        }

        public TezValueWithBasic(TezValueName name) : base(name)
        {

        }
    }
    #endregion

    #region Modifier
    public interface ITezModifiableValue : ITezValueWrapper
    {
        ITezRVMContainer container { get; }
    }

    public class TezModifiableIntValue
        : TezValueWrapper<int>
        , ITezModifiableValue
    {
        public sealed override TezValueSubType valueSubType => TezValueSubType.WithModifier;

        public ITezRVMContainer container { get; protected set; } = null;

        public int modifiedValue
        {
            get
            {
                return (int)this.container.refresh(this.value);
            }
        }

        public override int value
        {
            get
            {
                return base.value;
            }

            set
            {
                base.value = value;
                this.container.dirty = true;
            }
        }

        public TezModifiableIntValue(ITezValueName name) : base(name)
        {

        }

        public override void close()
        {
            base.close();
            this.container.close();
            this.container = null;
        }
    }

    public class TezModifiableFloatValue
        : TezValueWrapper<float>
        , ITezModifiableValue
    {
        public sealed override TezValueSubType valueSubType => TezValueSubType.WithModifier;

        public ITezRVMContainer container { get; protected set; } = null;

        public float modifiedValue
        {
            get
            {
                return this.container.refresh(this.value);
            }
        }

        public override float value
        {
            get
            {
                return base.value;
            }

            set
            {
                base.value = value;
                this.container.dirty = true;
            }
        }

        public TezModifiableFloatValue(ITezValueName name) : base(name)
        {

        }

        public override void close()
        {
            base.close();
            this.container.close();
            this.container = null;
        }
    }
    #endregion
}



