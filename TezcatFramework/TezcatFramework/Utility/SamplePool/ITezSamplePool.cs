using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezSamplePool
    {
        string poolName { get; }
        object create();
        object customCreate();
        void recycle(object obj);
        void recycleWithClear(object obj);
        void destroy();
    }
}