﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezSamplePool : ITezCloseable
    {
        string poolName { get; }
        object create();
        object customCreate();
        void recycle(object obj);
        void recycleWithClear(object obj);
    }
}