using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventory : ITezCloseable
    {
        event TezEventExtension.Action<TezInventorySlot> onItemAdded;
        event TezEventExtension.Action<TezInventorySlot> onItemRemoved;

        int slotCount { get; }
        TezInventorySlot this[int index] { get; }
        void clearEvents();
        void paging(int begin, int end, TezEventExtension.Action<int, TezInventorySlot> action);
    }

    public class TezInventory<Object>
        : ITezInventory
        where Object : TezGameObject, ITezInventoryItem
    {
        public event TezEventExtension.Action<TezInventorySlot> onItemAdded;
        public event TezEventExtension.Action<TezInventorySlot> onItemRemoved;

        public int slotCount
        {
            get { return m_Slots.Count; }
        }

        List<TezInventorySlot> m_Slots = new List<TezInventorySlot>();

        public TezInventorySlot this[int index]
        {
            get
            {
                while (index >= m_Slots.Count)
                {
                    var slot = new TezInventorySlot(this, m_Slots.Count);
                    m_Slots.Add(slot);
                }
                return m_Slots[index];
            }
        }

        public void add(Object game_object, int count)
        {
            var stackable = game_object.itemConfig.stackable;

            TezInventorySlot result = null;
            for (int i = 0; i < m_Slots.Count; i++)
            {
                var slot = m_Slots[i];

                if (stackable)
                {
                    ///找空格子
                    if (result == null && slot.item == null)
                    {
                        result = slot;
                    }

                    ///找相同格子
                    if (slot.item != null && slot.item.sameAs(game_object))
                    {
                        result = slot;
                        break;
                    }
                }
                else
                {
                    ///找空格子
                    if (slot.item == null)
                    {
                        result = slot;
                        break;
                    }
                }
            }

            ///如果有可以放下的格子
            if (result != null)
            {
                if (result.item != null)
                {
                    result.count += count;
                }
                else
                {
                    result.item = game_object;
                    result.count = count;
                }

                if (result.isBound)
                {
                    onItemAdded?.Invoke(result);
                }
            }
            else
            {
                result = new TezInventorySlot(this, m_Slots.Count);
                result.count = count;
                result.item = game_object;

                m_Slots.Add(result);
                onItemAdded?.Invoke(result);
            }
        }

        public void add(int slot_id, Object game_object, int count)
        {
            var slot = m_Slots[slot_id];
            slot.item = game_object;
            slot.count = count;
            if (slot.isBound)
            {
                onItemAdded?.Invoke(slot);
            }
        }

        public void remove(int slot_id, int count)
        {
            var slot = m_Slots[slot_id];
            slot.count -= count;
            if (slot.count == 0)
            {
                slot.item = null;
            }

            if (slot.isBound)
            {
                onItemRemoved?.Invoke(slot);
            }
        }

        public bool remove(Object game_object, int count)
        {
            var index = m_Slots.FindIndex((TezInventorySlot slot) =>
            {
                return slot.item != null && slot.item.sameAs(game_object);
            });

            if (index >= 0)
            {
                this.remove(index, count);
                return true;
            }

            return false;
        }

        public void paging(int begin, int end, TezEventExtension.Action<int, TezInventorySlot> action)
        {
            for (int i = begin; i < end; i++)
            {
                action(i, m_Slots[i]);
            }
        }

        public void clearEvents()
        {
            onItemAdded = null;
            onItemRemoved = null;
        }

        public virtual void close()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }
            m_Slots.Clear();
            m_Slots = null;

            onItemAdded = null;
            onItemRemoved = null;
        }
    }
}