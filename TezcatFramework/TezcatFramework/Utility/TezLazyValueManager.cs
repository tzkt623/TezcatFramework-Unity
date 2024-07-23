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

        public IReadOnlyDictionary<string, Value> values => mDict;
        Dictionary<string, Value> mDict = new Dictionary<string, Value>();

        #region Add
        public void set(string name, bool value)
        {
            if (!mDict.TryGetValue(name, out var slot))
            {
                slot = new Value();
                mDict.Add(name, slot);
            }

            slot.iValue = value ? 1 : 0;
        }

        public void set(string name, int value)
        {
            if (!mDict.TryGetValue(name, out var slot))
            {
                slot = new Value();
                mDict.Add(name, slot);
            }

            slot.iValue = value;
        }

        public void set(string name, float value)
        {
            if (!mDict.TryGetValue(name, out var slot))
            {
                slot = new Value();
                mDict.Add(name, slot);
            }

            slot.fValue = value;
        }

        public void set(string name, string value)
        {
            if (!mDict.TryGetValue(name, out var slot))
            {
                slot = new Value();
                mDict.Add(name, slot);
            }

            slot.sValue = value;
        }
        #endregion

        #region Get
        public bool getBool(string name, bool def = default)
        {
            if (mDict.TryGetValue(name, out Value value))
            {
                return value.iValue > 0;
            }

            return def;
        }

        public string getString(string name, string def = default)
        {
            if (mDict.TryGetValue(name, out Value value))
            {
                return value.sValue;
            }

            return def;
        }

        public float getFloat(string name, float def = default)
        {
            if (mDict.TryGetValue(name, out Value value))
            {
                return value.fValue;
            }

            return def;
        }

        public int getInt(string name, int def = default)
        {
            if (mDict.TryGetValue(name, out Value value))
            {
                return value.iValue;
            }

            return def;
        }
        #endregion

        void ITezCloseable.closeThis()
        {
            foreach (var item in mDict)
            {
                item.Value.sValue = null;
            }

            mDict.Clear();
            mDict = null;
        }
    }
}