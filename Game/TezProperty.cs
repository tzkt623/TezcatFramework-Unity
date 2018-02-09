using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public interface ITezPropertyRegister
    {
        TezPropertyRegister propertyRegister { get; }
    }

    public enum TezPropertyType
    {
        Empty,

        T,

        Float,
        Int,
        Bool,
        String,

        Float_Array,
        Int_Array,
        Bool_Array
    }

    #region Name
    public class TezPropertyName
    {
        public readonly string key_name;
        public readonly int key_id;

        public TezPropertyName(string name, int id)
        {
            key_name = name;
            key_id = id;
        }
    }
    #endregion

    #region PV
    public abstract class TezPropertyValue
    {
        public TezPropertyValue(TezPropertyName name)
        {
            m_Name = name;
        }

        public TezPropertyValue()
        {

        }

        TezPropertyName m_Name = null;
        public TezPropertyName name
        {
            get { return m_Name; }
        }

        public abstract TezPropertyType getParameterType();

        public abstract void accept(TezPropertyFunction pf);

        public abstract void copyFrom(TezPropertyValue value);

        public bool equalTo(TezPropertyValue other)
        {
            return m_Name.key_id == other.m_Name.key_id;
        }

        public virtual void clear()
        {
            m_Name = null;
        }
    }

    public abstract class TezPropertyValueT<T> : TezPropertyValue
    {
        public TezPropertyValueT(TezPropertyName name) : base(name)
        {

        }

        public TezPropertyValueT()
        {

        }

        protected T m_Value;
        public T value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public override void copyFrom(TezPropertyValue value)
        {
            var temp = value as TezPropertyValueT<T>;
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

    public class TezPV_T<T> : TezPropertyValueT<T>
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

    public class TezPV_Bool : TezPropertyValueT<bool>
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

    public class TezPV_Int : TezPropertyValueT<int>
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

    public class TezPV_Float : TezPropertyValueT<float>
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

    public class TezPV_String : TezPropertyValueT<string>
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
    #endregion



    #region PF
    public abstract class TezPropertyFunction
    {
        public TezPropertyName name
        {
            get; private set;
        }

        public abstract TezPropertyType parameterType { get; }

        public TezPropertyFunction(TezPropertyName name)
        {
            this.name = name;
        }

        public virtual void clear()
        {
            this.name = null;
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

    public class TezPF_T<T> : TezPropertyFunctionT<T>
    {
        public TezPF_T(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.T; }
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

    public class TezPF_Float_Array : TezPropertyFunctionT<float[]>
    {
        public TezPF_Float_Array(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Float_Array; }
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

    public class TezPF_Int_Array : TezPropertyFunctionT<int[]>
    {
        public TezPF_Int_Array(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Int_Array; }
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

    public class TezPF_Bool_Array : TezPropertyFunctionT<bool[]>
    {
        public TezPF_Bool_Array(TezPropertyName name) : base(name)
        {
        }

        public override TezPropertyType parameterType
        {
            get { return TezPropertyType.Bool_Array; }
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
    #endregion



    #region Register
    public class TezPropertyRegister
    {
        #region 属性注册
        static Dictionary<string, TezPropertyName> m_KeyDic = new Dictionary<string, TezPropertyName>();
        static Dictionary<int, TezPropertyName> m_IDDic = new Dictionary<int, TezPropertyName>();

        public static TezPropertyName register(string name)
        {
            int id = m_KeyDic.Count;
            Assert.IsFalse(m_KeyDic.ContainsKey(name), "PropertyKey [" + name + "] has added");

            TezPropertyName pname = new TezPropertyName(name, id);

            m_KeyDic.Add(pname.key_name, pname);
            m_IDDic.Add(pname.key_id, pname);
            return pname;
        }

        public static int getKeyID(string name)
        {
            return m_KeyDic[name].key_id;
        }

        public static string getKeyName(int id)
        {
            return m_IDDic[id].key_name;
        }

        public static bool hasKey(string name)
        {
            return m_KeyDic.ContainsKey(name);
        }
        #endregion



        Dictionary<int, TezPropertyFunction> m_PropertyFunctionDic = new Dictionary<int, TezPropertyFunction>();
        Dictionary<int, TezPropertyValue> m_PropertyValueDic = new Dictionary<int, TezPropertyValue>();

        public void unregister(TezPropertyName name)
        {
            m_PropertyFunctionDic.Remove(name.key_id);
        }

        #region Float
        public void register(TezPV_Float value, TezEventBus.Action<float> function)
        {
            this.register(value.name, function);
            m_PropertyValueDic.Add(value.name.key_id, value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<float> function)
        {
            TezPF_Float p = new TezPF_Float(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<float[]> function)
        {
            TezPF_Float_Array p = new TezPF_Float_Array(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }
        #endregion

        #region Int
        public void register(TezPV_Int value, TezEventBus.Action<int> function)
        {
            this.register(value.name, function);
            m_PropertyValueDic.Add(value.name.key_id, value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<int> function)
        {
            TezPF_Int p = new TezPF_Int(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<int[]> function)
        {
            TezPF_Int_Array p = new TezPF_Int_Array(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }
        #endregion

        #region Bool
        public void register(TezPV_Bool value, TezEventBus.Action<bool> function)
        {
            this.register(value.name, function);
            m_PropertyValueDic.Add(value.name.key_id, value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<bool> function)
        {
            TezPF_Bool p = new TezPF_Bool(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<bool[]> function)
        {
            TezPF_Bool_Array p = new TezPF_Bool_Array(name);
            p.setFunction(function);

            m_PropertyFunctionDic.Add(name.key_id, p);
        }
        #endregion


        public TezPropertyFunction getPropertyFunction(TezPropertyName name)
        {
            TezPropertyFunction function;
            m_PropertyFunctionDic.TryGetValue(name.key_id, out function);
            return function;
        }

        public TezPropertyValue getPropertyValue(TezPropertyName name)
        {
            TezPropertyValue value;
            m_PropertyValueDic.TryGetValue(name.key_id, out value);
            return value;
        }

        public void clear()
        {
            foreach (var pair in m_PropertyFunctionDic)
            {
                pair.Value.clear();
            }

            m_PropertyFunctionDic.Clear();

            foreach (var pair in m_PropertyValueDic)
            {
                pair.Value.clear();
            }

            m_PropertyValueDic.Clear();
        }

        public void inject(TezPropertyValue pv)
        {
            var pf = this.getPropertyFunction(pv.name);
            if (pf != null)
            {
                pv.accept(pf);
            }
        }

        public void inject(TezPropertyName name, bool value)
        {
            var pf = this.getPropertyFunction(name) as TezPF_Bool;
            pf?.invoke(value);
        }

        public void inject(TezPropertyName name, int value)
        {
            var pf = this.getPropertyFunction(name) as TezPF_Int;
            pf?.invoke(value);

        }
        public void inject(TezPropertyName name, float value)
        {
            var pf = this.getPropertyFunction(name) as TezPF_Float;
            pf?.invoke(value);
        }
    }

    public static class TezPropertyInjector
    {
        public static bool inject(TezPropertyRegister register, TezPropertyValue pv)
        {
            var pf = register.getPropertyFunction(pv.name);
            if (pf != null)
            {
                pv.accept(pf);
                return true;
            }

            return false;
        }

        public static bool inject(TezPropertyRegister register, TezPropertyName name, float value)
        {
            var pf = (TezPF_Float)register.getPropertyFunction(name);
            if (pf != null)
            {
                pf.invoke(value);
                return true;
            }

            return false;
        }

        public static bool inject(TezPropertyRegister register, TezPropertyName name, int value)
        {
            var pf = (TezPF_Int)register.getPropertyFunction(name);
            if (pf != null)
            {
                pf.invoke(value);
                return true;
            }

            return false;
        }

        public static bool inject(TezPropertyRegister register, TezPropertyName name, int[] value)
        {
            var pf = (TezPF_Int_Array)register.getPropertyFunction(name);
            if (pf != null)
            {
                pf.invoke(value);
                return true;
            }

            return false;
        }

        public static bool inject(TezPropertyRegister register, TezPropertyName name, bool[] value)
        {
            var pf = (TezPF_Bool_Array)register.getPropertyFunction(name);
            if (pf != null)
            {
                pf.invoke(value);
                return true;
            }

            return false;
        }
    }

    #endregion
}