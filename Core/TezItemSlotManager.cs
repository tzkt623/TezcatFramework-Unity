using System.Collections.Generic;

namespace tezcat.Core
{
    public interface ITezItemSlotManager
    {
        System.Type GetType();
    }
    /// <summary>
    /// 
    /// 基础Item管理器
    /// 
    /// 在此框架下
    /// 所有Item类的物品
    /// 都必须处于此类管理器中
    /// 由一个Slot进行保管
    /// 
    /// 可通过一个ID获得Slot
    /// 进而获得其中Item的信息
    /// 
    /// 所以当你选择到一个Item时
    /// 必然会得到Slot以及管理器的信息
    /// 方便进行后续操作
    /// 
    /// </summary>
    public abstract class TezItemSlotManager<T> : ITezItemSlotManager where T : TezItemSlot, new()
    {
        protected List<T> m_Slots = new List<T>();
        protected Queue<T> m_EmptySlots = new Queue<T>();

        public int slotCount
        {
            get { return m_Slots.Count; }
        }

        public T this[int slot_id]
        {
            get { return m_Slots[slot_id]; }
        }

        public void setCapacity(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var slot = new T();
                slot.manager = this;
                slot.ID = m_Slots.Count;
                this.m_Slots.Add(slot);
            }
        }

        public void grow(int slot_count)
        {
            while (m_Slots.Count <= slot_count)
            {
                var slot = new T();
                slot.manager = this;
                slot.ID = m_Slots.Count;
                m_Slots.Add(slot);
            }
        }

        protected void add(TezEventBus.Action<T> set_slot)
        {
            var slot = new T();
            slot.manager = this;
            slot.ID = m_Slots.Count;
            m_Slots.Add(slot);
            set_slot(slot);
        }

        protected void set(int slot_id, TezEventBus.Action<T> set_slot)
        {
            var slot = m_Slots[slot_id];
            set_slot(slot);
        }

        protected void remove(int slot_id)
        {
            var slot = m_Slots[slot_id];
            slot.item = null;
        }

        public bool tryGet(int slot_id, out T slot)
        {
            slot = default(T);

            if (slot_id >= m_Slots.Count)
            {
                return false;
            }

            slot = m_Slots[slot_id];
            return slot != null;
        }

        public void foreachSlot(TezEventBus.Action<T> function)
        {
            foreach (var slot in m_Slots)
            {
                function(slot);
            }
        }

        public void clear()
        {
            foreach (var slot in m_Slots)
            {
                slot.clear();
            }

            m_Slots.Clear();
        }
    }
}