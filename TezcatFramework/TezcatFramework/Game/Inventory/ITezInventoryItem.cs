using tezcat.Framework.Database;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventoryItem
    {
        /// <summary>
        /// 可堆叠数量
        /// 只有一模一样的物品才有可堆叠的可能
        /// 比如都是+500血的血瓶 属性一模一样的一件武器
        /// 
        /// 具有随机属性性质的物品都不可以堆叠
        /// 因为他们每一件都是唯一的
        /// </summary>
        int stackCount { get; }

        bool templateAs(ITezInventoryItem item);
        void recycle();
    }
}