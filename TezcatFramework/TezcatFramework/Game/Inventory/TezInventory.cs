using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 
    /// 背包系统
    /// 
    /// </summary>
    public class TezInventory
        : TezFlagable
    {
        public event TezEventExtension.Action<TezInventoryItemSlot> onItemAdded;
        public event TezEventExtension.Action<TezInventoryItemSlot> onItemRemoved;

        /// <summary>
        /// 物品数量
        /// </summary>
        public int count
        {
            get { return mSlots.Count; }
        }

        protected List<TezInventoryItemSlot> mSlots = new List<TezInventoryItemSlot>();
        protected Dictionary<long, TezInventoryItemSlot> mStackItemTable = new Dictionary<long, TezInventoryItemSlot>();

        /// <summary>
        /// 获得一个Slot
        /// </summary>
        public TezInventoryItemSlot this[int index]
        {
            get
            {
                return mSlots[index];
            }
        }

        /// <summary>
        /// 可以自己修改Slot
        /// </summary>
        protected virtual TezInventoryItemSlot createItemSlot()
        {
            return new TezInventoryItemSlot(this);
        }

        /// <summary>
        /// 存入1个
        /// </summary>
        public void store(TezItemableObject item)
        {
            this.store(item, 1);
        }

        /// <summary>
        /// 加入多个相同item
        /// </summary>
        public void store(TezItemableObject item, int count)
        {
            TezInventoryItemSlot result_slot;
            TezBaseItemInfo item_info = TezcatFramework.fileDB.getItemInfo(item.itemID);

            var stack_max_count = item_info.stackCount;
            bool stackable = stack_max_count > 0;

            if (stackable)//如果可以堆叠
            {
                TezInventoryItemSlot pre_slot = null;
                bool has_root = mStackItemTable.TryGetValue(item.itemID.ID, out result_slot);
                int remain;
                while (count > 0)
                {
                    //如果当前slot为空
                    if (result_slot == null)
                    {
                        if (!this.findEmptySlot(out result_slot))
                        {
                            result_slot = this.createItemSlot();
                            result_slot.index = mSlots.Count;

                            mSlots.Add(result_slot);
                        }
                        result_slot.count = 0;

                        //第一次加入搜索table
                        if (!has_root)
                        {
                            has_root = true;
                            mStackItemTable.Add(item.itemID.ID, result_slot);
                        }

                        //如果有前一个slot
                        //把这两个slot连接起来
                        if (pre_slot != null)
                        {
                            pre_slot.nextSlot = result_slot;
                            result_slot.preSlot = pre_slot;
                        }
                    }

                    //计算剩余空位
                    remain = stack_max_count - result_slot.count;
                    if (remain >= count)
                    {
                        //如果剩余空位大于等于加入数量
                        //直接加入
                        result_slot.count += count;
                        count = 0;
                    }
                    else
                    {
                        //如果剩余空间小于加入数量
                        //计算当前slot可以添加多少个
                        //以及剩余多少个
                        result_slot.count = stack_max_count;
                        count -= remain;

                        //记录当前slot
                        pre_slot = result_slot;
                        //取得下一个连接slot
                        result_slot = result_slot.nextSlot;
                    }
                }
            }
            else//如果不可以堆叠
            {
                //找到格子
                if (this.findEmptySlot(out result_slot))
                {
                    result_slot.item = item;
                    result_slot.count = -1;
                }
                //没有找到格子
                else
                {
                    result_slot = this.createItemSlot();
                    result_slot.item = item;
                    result_slot.count = -1;
                    result_slot.index = mSlots.Count;
                    mSlots.Add(result_slot);
                }
            }

            onItemAdded?.Invoke(result_slot);
        }

        /// <summary>
        /// 添加多个Item到某个位置上
        /// 由于数量可能会超过堆叠
        /// 不可堆叠的物品不会超过堆叠
        /// </summary>
        public void store(int slotIndex, TezItemableObject item, int count)
        {
            TezBaseItemInfo item_info = TezcatFramework.fileDB.getItemInfo(item.itemID);

            var stack_max_count = item_info.stackCount;
            bool stackable = stack_max_count > 0;

            var slot = mSlots[slotIndex];
            slot.item = item;
            if (stackable)
            {
                slot.count += count;
                if (slot.count >= stack_max_count)
                {
                    var remain = slot.count - stack_max_count;
                    this.store(item, remain);
                }
            }
            else
            {
                slot.count = -1;
            }

            onItemAdded?.Invoke(slot);
        }

        /// <summary>
        /// 从精确的位置上取出精确个数的物品
        /// </summary>
        public TezItemableObject take(int slotIndex, int count)
        {
            var slot = mSlots[slotIndex];
            var item = slot.item;

            if (slot.count < 0)
            {
                //不允许堆叠
                slot.item = null;
            }
            else
            {
                //允许堆叠
                slot.count -= count;
                if (slot.count == 0)
                {
                    if (slot.preSlot != null)
                    {
                        //如果有前一个节点
                        //说明当前节点是在链表中间位置
                        //需要重新连接前后两个Slot
                        if (slot.nextSlot != null)
                        {
                            slot.preSlot.nextSlot = slot.nextSlot;
                            slot.nextSlot.preSlot = slot.preSlot;
                        }
                        else
                        {
                            slot.preSlot.nextSlot = null;
                        }
                    }
                    else
                    {
                        //如果没有前一个节点
                        //说明此节点存在于索引中
                        //需要重新建立索引
                        if (slot.nextSlot != null)
                        {
                            //如果下一个节点存在
                            //则把下一个节点变成索引的Root节点
                            slot.nextSlot.preSlot = null;
                            mStackItemTable[slot.item.itemID.ID] = slot.nextSlot;
                        }
                        else
                        {
                            //如果下一个节点也为空
                            //说明此物品不存在物品栏中
                            //删除索引
                            mStackItemTable.Remove(slot.item.itemID.ID);
                        }
                    }

                    slot.item = null;
                }
            }

            onItemRemoved?.Invoke(slot);
            return item;
        }

        /// <summary>
        /// 从精确的位置上取出1个物品
        /// </summary>
        public TezItemableObject take(int slotIndex)
        {
            return this.take(slotIndex, 1);
        }

        private bool findEmptySlot(out TezInventoryItemSlot slot)
        {
            int result_index = 0;

            while (result_index < mSlots.Count)
            {
                slot = mSlots[result_index];
                ///找空格子
                if (slot.item == null)
                {
                    return true;
                }
                result_index++;
            }

            slot = null;
            return false;
        }

        private TezInventoryItemSlot findSlotStackable(TezItemableObject item, int stackCount, int count)
        {
            TezInventoryItemSlot result_slot = null;

            int result_index = 0;
            while (result_index < mSlots.Count)
            {
                var slot = mSlots[result_index];
                //找空格子
                if (result_slot == null && slot.item == null)
                {
                    result_slot = slot;
                }

                //找相同格子
                //可堆叠的物品
                //他们的模板相同
                if (slot.item != null && slot.item.itemID.sameAs(item.itemID))
                {
                    if (slot.count < stackCount)
                    {
                        result_slot = slot;
                        break;
                    }
                }

                result_index++;
            }

            return result_slot;
        }

        /// <summary>
        /// 从包裹里面查询是否有某个特定物品
        /// 返回-1表示没有
        /// </summary>
        public int find(TezItemableObject storableObject)
        {
            return mSlots.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item == storableObject;
            });
        }

        /// <summary>
        /// 从包裹里面查询是否有某个特定类型的物品
        /// </summary>
        public int find(TezCategory category)
        {
            return mSlots.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item.category == category;
            });
        }

        public override void close()
        {
            base.close();

            for (int i = 0; i < mSlots.Count; i++)
            {
                mSlots[i].close();
            }
            mSlots.Clear();
            mSlots = null;

            onItemAdded = null;
            onItemRemoved = null;
        }
    }
}