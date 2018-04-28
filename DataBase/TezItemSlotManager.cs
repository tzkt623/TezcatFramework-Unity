using System.Collections.Generic;
namespace tezcat.DataBase
{
    public abstract class TezItemSlotManager<T> where T : TezItemSlot, new()
    {
        public List<T> slots { get; private set; } = new List<T>();
        protected Queue<T> m_EmptySlots = new Queue<T>();

        public int count
        {
            get { return slots.Count; }
        }

        public T this[int slot_id]
        {
            get { return slots[slot_id]; }
        }

        public T get(int slot_id)
        {
            return slots[slot_id];
        }

        public void grow(int count)
        {
            while (slots.Count <= count)
            {
                var slot = new T();
                slot.id = slots.Count;
                slots.Add(slot);
            }
        }

        protected void add(TezEventBus.Action<T> set_slot)
        {
            var slot = new T();
            slot.id = slots.Count;
            slots.Add(slot);
            set_slot(slot);
        }

        protected void set(int slot_id, TezEventBus.Action<T> set_slot)
        {
            var slot = slots[slot_id];
            set_slot(slot);
        }

        protected void remove(int slot_id)
        {
            var slot = slots[slot_id];
            slot.item = null;
        }

        public bool tryGet(int slot_id, out T slot)
        {
            slot = default(T);

            if (slot_id >= slots.Count)
            {
                return false;
            }

            slot = slots[slot_id];
            if (slot.item == null)
            {
                return false;
            }

            return true;
        }

        public void foreachSlot(TezEventBus.Action<T> function)
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
                slot.clear();
            }

            slots.Clear();
        }
    }
}