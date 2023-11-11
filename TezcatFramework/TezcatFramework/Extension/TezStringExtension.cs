namespace tezcat.Framework.Extension
{
    public static class TezStringExtension
    {
        public static bool isNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}