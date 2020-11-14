using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventorySlot : ITezCloseable
    {
        public ITezInventory inventory { get; private set; } = null;
        /// <summary>
        /// 指示当前Slot是否被绑定到显示页面上了
        /// </summary>
        public bool isBound { get; set; } = false;
        public int index { get; private set; } = -1;

        public TezGameObject item { get; set; } = null;
        public int count { get; set; } = -1;

        public T getItem<T>() where T : TezGameObject
        {
            return (T)this.item;
        }

        public TezInventorySlot(ITezInventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
        }

        public virtual void close()
        {
            this.item = null;
            this.inventory = null;
        }
    }
}