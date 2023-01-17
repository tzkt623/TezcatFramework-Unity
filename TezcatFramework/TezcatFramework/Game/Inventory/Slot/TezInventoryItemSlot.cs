using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public class TezInventoryItemSlot : TezInventoryDataSlot
    {
        /// <summary>
        /// 初始IndexID
        /// </summary>
        public const int InitIndex = -1;
        /// <summary>
        /// 被隐藏后IndexID
        /// </summary>
        public const int HideIndex = -2;
        /// <summary>
        /// 临时槽位ID
        /// </summary>
        public const int TempIndex = -3;

        public override Category category => Category.Item;

        public override TezInventoryItemSlot itemSlot => this;

        public bool picked { get; set; } = false;

        TezInventory mInventory = null;
        /// <summary>
        /// 格子属于的Inventory
        /// </summary>
        public TezInventory inventory => mInventory;


        TezItemableObject mItem = null;
        /// <summary>
        /// 装的Item
        /// </summary>
        public TezItemableObject item
        {
            get { return mItem; }
            set { mItem = value; }
        }

        /// <summary>
        /// Item的数量
        /// 为-1表示不可堆叠
        /// </summary>
        public int count { get; set; } = -1;


        /// <summary>
        /// 上一个
        /// </summary>
        public TezInventoryItemSlot preSlot { get; set; }
        /// <summary>
        /// 下一个
        /// </summary>
        public TezInventoryItemSlot nextSlot { get; set; }

        /// <summary>
        /// 转换Item
        /// 转换失败返回Null
        /// </summary>
        public T getItem<T>() where T : TezItemableObject
        {
            return (T)mItem;
        }

        public TezInventoryItemSlot(TezInventory inventory)
        {
            mInventory = inventory;
            this.picked = false;
        }

        public override void close()
        {
            base.close();
            mItem = null;
            mInventory = null;
            this.preSlot = null;
            this.nextSlot = null;
        }

        /// <summary>
        /// 拿出
        /// </summary>
        public TezItemableObject take()
        {
            return mInventory.take(this.index);
        }

        /// <summary>
        /// 存入
        /// </summary>
        public void store(TezItemableObject item, int count)
        {
            mInventory.store(this.index, item, count);
        }
    }
}