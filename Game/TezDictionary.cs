using System.Collections.Generic;

namespace tezcat
{
    public static class TezDictionaryExtension
    {
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if(dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            return false;
        }
    }
}