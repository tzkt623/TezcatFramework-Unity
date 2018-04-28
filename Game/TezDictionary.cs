using System.Collections.Generic;

namespace tezcat
{
    public static class TezDictionaryExtension
    {
        public static bool tryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if(dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            value = default(TValue);
            return false;
        }

        public static bool tryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, out TValue result)
        {
            if(dictionary.TryGetValue(key, out result))
            {
                return false;
            }

            result = value;
            dictionary.Add(key, value);
            return true;
        }
    }
}