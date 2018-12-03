using System.Diagnostics;

namespace tezcat.Framework.Core
{
    public static class TezAssert
    {
        public static void mustTrue(bool condition)
        {
            if (!condition)
            {
                StackTrace stackTrace = new StackTrace(1, true);
            }
        }

        public static void notNull(object obj)
        {
            if (obj == null)
            {
                StackTrace stackTrace = new StackTrace(1, true);
            }
        }
    }
}