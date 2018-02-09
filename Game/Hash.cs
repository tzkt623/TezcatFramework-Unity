using System;
using System.Collections.Generic;

namespace tezcat
{
    public static class Hash
    {
        public static int SDBMLower(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return Hash.SDBM(s.ToLower());
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
                array[i] = Hash.SDBMLower(strings[i]);
            }
            return array;
        }
    }
}