using System.Collections.Generic;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    public static class TezInventoryHelper
    {
        public static TezInventoryItemSlot add(TezComData gameObject, int count, List<TezInventoryItemSlot> slotList, TezInventory inventory)
        {
            //          var stackable = TezDatabaseItemConfig.getConfig(gameObject.).stackCount > 0;

            var stackable = false;
            int result_index = 0;
            TezInventoryItemSlot result_slot = null;
            while (result_index < slotList.Count)
            {
                var slot = slotList[result_index];
                if (stackable)
                {
                    ///找空格子
                    if (result_slot == null && slot.item == null)
                    {
                        result_slot = slot;
                    }

                    ///找相同格子
                    ///可堆叠的物品
                    ///他们的模板相同
                    if (slot.item != null && slot.item.templateAs(gameObject))
                    {
                        result_slot = slot;
                        break;
                    }
                }
                else
                {
                    ///找空格子
                    if (slot.item == null)
                    {
                        result_slot = slot;
                        break;
                    }
                }
                result_index++;
            }

            ///如果有可以放下的格子
            ///记录数据
            ///并回收临时格子
            if (result_slot != null)
            {
                if (result_slot.item != null)
                {
                    result_slot.count += count;
                    ///如果是可堆叠物品
                    ///并且已有存在的物品
                    ///计数+1并删除自己
                    if (stackable)
                    {
                        gameObject.close();
                    }
                }
                else
                {
                    result_slot.item = gameObject;
                    result_slot.count = count;
                }
            }
            ///如果没有格子用
            ///把当前格子变成现有格子
            ///不回收
            else
            {
                result_slot = new TezInventoryItemSlot(inventory)
                {
                    item = gameObject,
                    count = count,
                    index = slotList.Count
                };

                slotList.Add(result_slot);
            }

            return result_slot;
        }

        public static bool remove(TezComData gameObject, int count, List<TezInventoryItemSlot> slotList, out TezInventoryItemSlot resultSlot)
        {
            var index = slotList.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item == gameObject;
            });

            if (index >= 0)
            {
                resultSlot = slotList[index];
                resultSlot.count -= count;
                if (resultSlot.count == 0)
                {
                    resultSlot.item = null;
                    if (resultSlot.index == TezInventoryItemSlot.HideIndex)
                    {
                        slotList.RemoveAt(index);
                    }
                }

                return true;
            }

            resultSlot = null;
            return false;
        }
    }
}