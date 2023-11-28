using System.Collections.Generic;

namespace tezcat.Framework.TMath
{
    public static class TezHash
    {
        public static int SDBMLower(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return TezHash.SDBM(s.ToLower());
        }

        public static int SDBM(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            uint num = 0u;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                num = (uint)c + (num << 6) + (num << 16) - num;
            }
            return (int)num;
        }

        public static int[] SDBMLower(IList<string> strings)
        {
            int[] array = new int[strings.Count];
            for (int i = 0; i < strings.Count; i++)
            {
                array[i] = TezHash.SDBMLower(strings[i]);
            }
            return array;
        }

        public static int intHash(int key)
        {
            key += ~(key << 15);
            key ^= (key >> 10);
            key += (key << 3);
            key ^= (key >> 6);
            key += ~(key << 11);
            key ^= (key >> 16);
            return key;
        }

        public static int FNVHash1(string data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;
                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        public static int FNVHash(params int[] array)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;
                for (int i = 0; i < array.Length; i++)
                    hash = hash * p ^ array[i];
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}