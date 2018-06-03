using System.Collections.Generic;
using tezcat.Debug;
using tezcat.Signal;

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
        static Dictionary<string, int> m_PropertyDic = new Dictionary<string, int>();
        static List<TezPropertyName> m_PropertyList = new List<TezPropertyName>();

        public static TezPropertyName register(string name)
        {
#if UNITY_EDITOR
            TezDebug.isFalse(m_PropertyDic.ContainsKey(name), "TezPropertyRegister", string.Format("PropertyKey [{0}] Has Registered", name));
            TezDebug.info("TezPropertyRegister", string.Format("Register Property [{0}]", name));
#endif
            int id = m_PropertyList.Count;
            TezPropertyName pname = new TezPropertyName(name, id);

            m_PropertyList.Add(pname);
            m_PropertyDic.Add(pname.name, id);

            return pname;
        }

        public static int getKeyID(string name)
        {
            return m_PropertyList[m_PropertyDic[name]].ID;
        }

        public static string getKeyName(int id)
        {
            return m_PropertyList[id].name;
        }

        public static bool hasKey(string name)
        {
            return m_PropertyDic.ContainsKey(name);
        }

        public static void foreachProperty(TezEventBus.Action<TezPropertyName> action)
        {
            for (int i = 0; i < m_PropertyList.Count; i++)
            {
                action(m_PropertyList[i]);
            }
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
            functions.Remove(name.ID);
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
            this.register(value.propertyName, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<float> function)
        {
            TezPF_Float p = new TezPF_Float(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region Int
        public void register(TezPV_Int value, TezEventBus.Action<int> function)
        {
            this.register(value.propertyName, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<int> function)
        {
            TezPF_Int p = new TezPF_Int(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region Bool
        public void register(TezPV_Bool value, TezEventBus.Action<bool> function)
        {
            this.register(value.propertyName, function);
            properties.Add(value);
        }

        public void register(TezPropertyName name, TezEventBus.Action<bool> function)
        {
            TezPF_Bool p = new TezPF_Bool(name);
            p.setFunction(function);

            functions.Add(p);
        }
        #endregion

        #region String
        public void register(TezPV_String value, TezEventBus.Action<string> function)
        {
            this.register(value.propertyName, function);
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
            functions.binaryFind(name.ID, out function);
            return function;
        }

        public TezPropertyValue getPropertyValue(TezPropertyName name)
        {
            TezPropertyValue value;
            properties.binaryFind(name.ID, out value);
            return value;
        }

        protected PV getPropertyValue<PV>(TezPropertyName name) where PV : TezPropertyValue
        {
            TezPropertyValue value;
            properties.binaryFind(name.ID, out value);
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
            var pf = this.getPropertyFunction(pv.propertyName);
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
