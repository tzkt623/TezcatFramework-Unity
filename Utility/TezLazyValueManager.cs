using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public class TezLazyValueManager : ITezCloseable
    {
        public class Value
        {
            public int iValue;
            public float fValue;
            public string sValue;
        }

        public IReadOnlyDictionary<string, Value> values => m_Dic;
        Dictionary<string, Value> m_Dic = new Dictionary<string, Value>();

        #region Add
        public void set(string name, bool value)
        {
            if (!m_Dic.TryGetValue(name, out var slot))
            {
                slot = new Value();
                m_Dic.Add(name, slot);
            }

            slot.iValue = value ? 1 : 0;
        }

        public void set(string name, int value)
        {
            if (!m_Dic.TryGetValue(name, out var slot))
            {
                slot = new Value();
                m_Dic.Add(name, slot);
            }

            slot.iValue = value;
        }

        public void set(string name, float value)
        {
            if (!m_Dic.TryGetValue(name, out var slot))
            {
                slot = new Value();
                m_Dic.Add(name, slot);
            }

            slot.fValue = value;
        }

        public void set(string name, string value)
        {
            if (!m_Dic.TryGetValue(name, out var slot))
            {
                slot = new Value();
                m_Dic.Add(name, slot);
            }

            slot.sValue = value;
        }
        #endregion

        #region Get
        public bool getBool(string name, bool def = default)
        {
            if (m_Dic.TryGetValue(name, out Value value))
            {
                return value.iValue > 0;
            }

            return def;
        }

        public string getString(string name, string def = default)
        {
            if (m_Dic.TryGetValue(name, out Value value))
            {
                return value.sValue;
            }

            return def;
        }

        public float getFloat(string name, float def = default)
        {
            if (m_Dic.TryGetValue(name, out Value value))
            {
                return value.fValue;
            }

            return def;
        }

        public int getInt(string name, int def = default)
        {
            if (m_Dic.TryGetValue(name, out Value value))
            {
                return value.iValue;
            }

            return def;
        }
        #endregion

        public void close()
        {
            foreach (var item in m_Dic)
            {
                item.Value.sValue = null;
            }

            m_Dic.Clear();
            m_Dic = null;
        }
    }
}