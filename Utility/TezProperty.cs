using System;

namespace tezcat.Utility
{
    public interface ITezPropertyRegister
    {
        TezPropertyManager propertyRegister { get; }
    }

    public enum TezPropertyType
    {
        Empty = -1,

        T,

        Float,
        Int,
        Bool,
        String,

        Float_Array,
        Int_Array,
        Bool_Array
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

        public TezPropertyName propertyName { get; private set; } = null;
        public string name { get { return propertyName.key_name; } }
        public int ID { get { return propertyName.key_id; } }

        int ITezListSortItem.sortID
        {
            get { return propertyName.key_id; }
        }

        public abstract TezPropertyType getParameterType();

        public abstract void accept(TezPropertyFunction pf);

        public abstract void copyFrom(TezPropertyValue value);

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

        public System.Type systemType
        {
            get { return typeof(T); }
        }

        protected T m_Value = default(T);
        public T value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public override void copyFrom(TezPropertyValue value)
        {
            var temp = value as TezPropertyValue<T>;
            m_Value = temp.m_Value;
        }

        public override void clear()
        {
            base.clear();
            m_Value = default(T);
        }
    }


    public class TezPV_Empty : TezPropertyValue
    {
        public override void accept(TezPropertyFunction pf)
        {
            throw new System.NotImplementedException();
        }

        public override void copyFrom(TezPropertyValue value)
        {
            throw new System.NotImplementedException();
        }

        public override TezPropertyType getParameterType()
        {
            return TezPropertyType.Empty;
        }
    }

    public class TezPV_T<T> : TezPropertyValue<T>
    {
        public System.Type valueType
        {
            get { return typeof(T); }
        }

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
}



