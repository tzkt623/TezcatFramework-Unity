using System.Collections.Generic;
using tezcat.Signal;

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
        public List<T> slots { get; private set; } = new List<T>();
        Queue<T> m_EmptySlots = new Queue<T>();

        public int slotCount
        {
            get { return slots.Count; }
        }

        public T this[int slot_id]
        {
            get
            {
                return slots[slot_id];
            }
        }

        public void setCapacity(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var slot = new T();
                slot.manager = this;
                slot.ID = slots.Count;
                this.slots.Add(slot);
            }
        }

        public void grow(int slot_count)
        {
            while (slots.Count <= slot_count)
            {
                var slot = new T();
                slot.manager = this;
                slot.ID = slots.Count;
                slots.Add(slot);
            }
        }

        protected void add(TezEventCenter.Action<T> set_slot)
        {
            if(m_EmptySlots.Count > 0)
            {
                set_slot(m_EmptySlots.Dequeue());
            }
            else
            {
                var slot = new T();
                slot.manager = this;
                slot.ID = slots.Count;
                slots.Add(slot);
                set_slot(slot);
            }
        }

        protected void set(int slot_id, TezEventCenter.Action<T> set_slot)
        {
            set_slot(slots[slot_id]);
        }

        protected void remove(int slot_id)
        {
            var slot = slots[slot_id];
            slot.item = null;
            m_EmptySlots.Enqueue(slot);
        }

        public bool tryGet(int slot_id, out T slot)
        {
            slot = default(T);

            if (slot_id >= slots.Count)
            {
                return false;
            }

            slot = slots[slot_id];
            return slot != null;
        }

        public void foreachSlot(TezEventCenter.Action<T> function)
        {
            foreach (var slot in slots)
            {
                function(slot);
            }
        }

        public void clear()
        {
            foreach (var slot in slots)
            {
                slot.close();
            }

            slots.Clear();
        }
    }
}