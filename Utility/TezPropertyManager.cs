using System.Collections.Generic;

namespace tezcat.Utility
{
    public interface ITezPropertyOwner
    {
        System.Type GetType();
        List<TezPropertyValue> properties { get; }
    }

    public class TezPropertyManager
    {
        #region 属性注册
        static Dictionary<string, int> m_KeyDic = new Dictionary<string, int>();
        static List<TezPropertyName> m_IDList = new List<TezPropertyName>();

        public static TezPropertyName register(string name)
        {
#if UNITY_EDITOR
            TezDebug.isFalse(m_KeyDic.ContainsKey(name), "TezPropertyRegister", string.Format("PropertyKey [{0}] Has Registered", name));
            TezDebug.info("TezPropertyRegister", string.Format("Register Property [{0}]", name));
#endif
            int id = m_IDList.Count;
            TezPropertyName pname = new TezPropertyName(name, id);

            m_IDList.Add(pname);
            m_KeyDic.Add(pname.key_name, id);

            return pname;
        }

        public static int getKeyID(string name)
        {
            return m_IDList[m_KeyDic[name]].key_id;
        }

        public static string getKeyName(int id)
        {
            return m_IDList[id].key_name;
        }

        public static bool hasKey(string name)
        {
            return m_KeyDic.ContainsKey(name);
        }
        #endregion

        public ITezPropertyOwner owner { get; private set; } = null;

        public List<TezPropertyFunction> functions { get; private set; } = new List<TezPropertyFunction>();
        public List<TezPropertyValue> properties { get; private set; } = new List<TezPropertyValue>();

        public TezPropertyManager()
        {

        }

        public TezPropertyManager(ITezPropertyOwner owner)
        {
            this.owner = owner;
        }

        public void unregister(TezPropertyName name)
        {
            functions.Remove(name.key_id);
        }

        public void sortProperties()
        {
            properties.Sort();
        }

        public void sortFunctions()
        {
            functions.Sort();
        }

        public void register(TezPropertyValue value)
        {
            properties.Add(value);
        }

        #region Float
        public void register(TezPV_Float value, TezEventBus.Action<float> function)
        {
            this.register(value.name, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<float> function)
        {
            TezPF_Float p = new TezPF_Float(name);
            p.setFunction(function);

            functions.Add(p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<float[]> function)
        {
            TezPF_Float_Array p = new TezPF_Float_Array(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region Int
        public void register(TezPV_Int value, TezEventBus.Action<int> function)
        {
            this.register(value.name, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<int> function)
        {
            TezPF_Int p = new TezPF_Int(name);
            p.setFunction(function);

            functions.Add(p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<int[]> function)
        {
            TezPF_Int_Array p = new TezPF_Int_Array(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region Bool
        public void register(TezPV_Bool value, TezEventBus.Action<bool> function)
        {
            this.register(value.name, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<bool> function)
        {
            TezPF_Bool p = new TezPF_Bool(name);
            p.setFunction(function);

            functions.Add(p);
        }

        public void register(TezPropertyName name, TezEventBus.Action<bool[]> function)
        {
            TezPF_Bool_Array p = new TezPF_Bool_Array(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region String
        public void register(TezPV_String value, TezEventBus.Action<string> function)
        {
            this.register(value.name, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<string> function)
        {
            TezPF_String p = new TezPF_String(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion


        public TezPropertyFunction getPropertyFunction(TezPropertyName name)
        {
            TezPropertyFunction function;
            functions.binaryFind(name.key_id, out function);
            return function;
        }

        public TezPropertyValue getPropertyValue(TezPropertyName name)
        {
            TezPropertyValue value;
            properties.binaryFind(name.key_id, out value);
            return value;
        }

        protected PV getPropertyValue<PV>(TezPropertyName name) where PV : TezPropertyValue
        {
            TezPropertyValue value;
            properties.binaryFind(name.key_id, out value);
            return value as PV;
        }

        public void clear()
        {
            foreach (var pair in functions)
            {
                pair.clear();
            }

            functions.Clear();

            foreach (var pair in properties)
            {
                pair.clear();
            }

            properties.Clear();
        }

        public void inject<PV, InjectValue>(TezPropertyName name, TezPropertyInjector<PV, InjectValue> injector, InjectValue ivalue) where PV : TezPropertyValue
        {
            var property = this.getPropertyValue<PV>(name);
            if(property != null)
            {
                injector.inject(property, ivalue);
            }
        }

        public void inject<PV>(TezPropertyName name, TezEventBus.Action<PV> injector) where PV : TezPropertyValue
        {
            var property = this.getPropertyValue<PV>(name);
            if (property != null)
            {
                injector.Invoke(property);
            }
        }

        public void inject(TezPropertyName name, TezEventBus.Action<ITezPropertyOwner> injector)
        {
            var property = this.getPropertyValue(name);
            if (property != null)
            {
                injector.Invoke(this.owner);
            }
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
}
