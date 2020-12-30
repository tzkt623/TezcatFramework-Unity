using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public class TezInventorySlot : ITezCloseable
    {
        /// <summary>
        /// 格子属于的Inventory
        /// </summary>
        public ITezInventory inventory { get; private set; } = null;

        /// <summary>
        /// 指示当前Slot是否被绑定到显示页面上了
        /// </summary>
        public bool boundToUI => m_BoundRef > 0;
        byte m_BoundRef = 0;

        /// <summary>
        /// 格子Index
        /// </summary>
        public int index { get; set; } = -1;

        /// <summary>
        /// 装的Item
        /// </summary>
        public TezGameObject item { get; set; } = null;

        /// <summary>
        /// Item的数量
        /// </summary>
        public int count { get; set; } = -1;

        /// <summary>
        /// 转换Item
        /// 转换失败返回Null
        public T getItem<T>() where T : TezGameObject
        {
            return this.item as T;
        }

        public TezInventorySlot(ITezInventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
        }

        /// <summary>
        /// 保留一个绑定计数
        /// </summary>
        public void retainUIRef()
        {
            m_BoundRef++;
        }

        /// <summary>
        /// 减少一个绑定计数
        /// </summary>
        public void releaseUIRef()
        {
            if (m_BoundRef > 0)
            {
                m_BoundRef--;
            }
        }

        public virtual void close()
        {
            this.item = null;
            this.inventory = null;
        }
    }
}