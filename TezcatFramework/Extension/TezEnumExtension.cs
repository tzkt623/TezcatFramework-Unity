using System;

namespace tezcat.Framework.Extension
{
    public static class TezEnumExtension
    {
        public static T convertTo<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static int getCount<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}