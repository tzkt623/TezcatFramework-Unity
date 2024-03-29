﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezInventoryViewSlotData
    {
        TezItemObject item { get; }
        TezInventory inventory { get; set; }
        int count { get; }
        int viewIndex { get; set; }
    }

    public class TezInventoryViewSlot : ITezCloseable
    {
        public ITezInventoryViewSlotData data { get; set; }
        public int count => data.count;
        public int index { get; set; }

        public void close()
        {
            this.data = null;
        }

        public void clear()
        {
            this.data = null;
        }

        public virtual void pick()
        {

        }
    }
}