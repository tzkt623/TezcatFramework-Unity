using System.Collections.Generic;

namespace tezcat
{
    public class TezStorage<Item> where Item : class, ITezItem
    {
        /// <summary>
        /// Item Count ID
        /// </summary>
        public event TezEventBus.Action<Item, int, int> onItemAdded;
        /// <summary>
        /// Item Count ID
        /// </summary>
        public event TezEventBus.Action<Item, int, int> onItemRemoved;
        /// <summary>
        /// Item Count ID
        /// </summary>
        public event TezEventBus.Action<Item, int, int> onItemSetted;

        public class Slot
        {
            public int id = -1;
            public Item item = null;
            public int count = 0;
        }

        List<Slot> m_SlotList = new List<Slot>();
        public int count
        {
            get { return m_SlotList.Count; }
        }

        public Item getItem(int id)
        {
            if (id >= m_SlotList.Count)
            {
                return null;
            }

            return m_SlotList[id].item;
        }

        public int getItemCount(int id)
        {
            if (id >= m_SlotList.Count)
            {
                return -1;
            }

            return m_SlotList[id].count;
        }

        /// <summary>
        /// 新增加一个槽位来存放物品
        /// 不会合并相同的物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void newSlot(Item item, int count)
        {
            Slot slot = new Slot()
            {
                id = m_SlotList.Count,
                item = item,
                count = count
            };

            m_SlotList.Add(slot);

            onItemAdded?.Invoke(slot.item, slot.count, slot.id);
        }

        /// <summary>
        /// 加入一个物品
        /// 如果有相同的物品
        /// 自动合并
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void add(Item item, int count)
        {
            Slot selected = null;
            foreach (var slot in m_SlotList)
            {
                if (selected == null && slot.item == null)
                {
                    selected = slot;
                }

                if (slot.item == item)
                {
                    selected = slot;
                    break;
                }
            }

            if (selected == null)
            {
                selected = new Slot()
                {
                    id = m_SlotList.Count,
                };

                m_SlotList.Add(selected);
            }

            selected.item = item;
            selected.count += count;

            onItemAdded?.Invoke(selected.item, selected.count, selected.id);
        }

        /// <summary>
        /// 移除指定数量的物品
        /// 如果一个槽中的物品不够
        /// 会自动寻找下一个相同的物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void remove(Item item, int count)
        {
            int remaining = count;
            foreach (var slot in m_SlotList)
            {
                if (slot.item == item)
                {
                    if (slot.count >= remaining)
                    {
                        slot.count -= remaining;
                        remaining = 0;
                    }
                    else
                    {
                        remaining -= slot.count;
                        slot.count = 0;
                    }

                    if (slot.count == 0)
                    {
                        slot.item = null;
                    }

                    onItemRemoved?.Invoke(slot.item, slot.count, slot.id);

                    if(remaining == 0)
                    {
                        break;
                    }
                }
            }
        }

        public void remove(int id, int count)
        {
            var slot = m_SlotList[id];
            slot.count -= count;
            if(slot.count == 0)
            {
                slot.item = null;
            }
            onItemRemoved?.Invoke(slot.item, slot.count, slot.id);
        }

        public void split(int slot_id, int split_count)
        {
            var slot = m_SlotList[slot_id];
            slot.count -= split_count;
            onItemSetted?.Invoke(slot.item, slot.count, slot.id);

            var split_slot = new Slot()
            {
                item = slot.item,
                count = split_count,
                id = m_SlotList.Count
            };

            m_SlotList.Add(split_slot);
            onItemAdded?.Invoke(split_slot.item, split_slot.count, split_slot.id);
        }

        private void combine(int from, int to)
        {
            var slot_from = m_SlotList[from];

            var slot_to = m_SlotList[to];
            slot_to.count += slot_from.count;

            onItemRemoved?.Invoke(slot_from.item, slot_from.count, slot_from.id);
            onItemSetted?.Invoke(slot_to.item, slot_to.count, slot_to.id);
        }

        private void swap(int slot_1, int slot_2)
        {
            var slot1 = m_SlotList[slot_1];
            var slot2 = m_SlotList[slot_2];

            m_SlotList[slot_1] = slot2;
            slot2.id = slot_1;

            m_SlotList[slot_2] = slot1;
            slot1.id = slot_2;

            onItemSetted?.Invoke(slot1.item, slot1.count, slot1.id);
            onItemSetted?.Invoke(slot2.item, slot2.count, slot2.id);
        }

        public void swapOrCombine(int slot_1, int slot_2)
        {
            if (m_SlotList[slot_1].item == m_SlotList[slot_2].item)
            {
                this.combine(slot_1, slot_2);
            }
            else
            {
                this.swap(slot_1, slot_2);
            }
        }

        public bool tryGetItem(int slot_id, out Item item, out int count)
        {
            item = null;
            count = -1;

            if (slot_id >= m_SlotList.Count)
            {
                return false;
            }

            var slot = m_SlotList[slot_id];
            if (slot.item == null)
            {
                return false;
            }

            item = slot.item;
            count = slot.count;
            return true;
        }

        public void foreachItem(TezEventBus.Action<Item, int, int> function)
        {
            foreach (var slot in m_SlotList)
            {
                function(slot.item, slot.count, slot.id);
            }
        }

        public void clearItem()
        {
            foreach (var slot in m_SlotList)
            {
                slot.item = null;
            }

            m_SlotList.Clear();
        }
    }
}