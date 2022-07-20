using tezcat.Framework.Database;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.ECS
{
    public class TezInfoComponent
        : TezBaseComponent
        , ITezInventoryItem
    {
        public static int SComUID;
        public override int comUID => SComUID;

        /// <summary>
        /// 等于0为不允许堆叠
        /// </summary>
        public int stackCount { get; private set; } = 0;

        public int itemTypeID { get; private set; } = -1;

        /// <summary>
        /// 使用物品模板初始化对象
        /// </summary>
        public void loadItemData(ITezSerializableItem item)
        {
            var data_item = (TezDatabaseGameItem)item;
            this.stackCount = data_item.stackCount;
            this.itemTypeID = data_item.itemTypeID;
        }

        public bool templateAs(ITezInventoryItem item)
        {
            return this.itemTypeID == ((TezInfoComponent)item).itemTypeID;
        }

        public override void close()
        {

        }

        /// <summary>
        /// 回收掉这物品
        /// </summary>
        void ITezInventoryItem.recycle()
        {
            this.entity.close();
        }
    }
}