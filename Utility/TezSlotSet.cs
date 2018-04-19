using System.Collections.Generic;
using tezcat.DataBase;

namespace tezcat.Utility
{
    public class TezSlot
    {
        /// <summary>
        /// 槽ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 槽物品
        /// </summary>
        public TezItem item { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Type slotType
        {
            get { return this.GetType(); }
        }

        public virtual void clear()
        {
            item = null;
        }
    }

    public interface ITezSlotSet
    {
        int count { get; }

        TezItem get(int slot_id);
    }

    public abstract class TezSlotSet<T> where T : TezSlot, new()
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

        //         public void split(int slot_id, TezEventBus.Action<T, T> split_function)
        //         {
        //             var slot = slots[slot_id];
        // 
        //             var split_slot = this.createSlot();
        //             split_slot.item = slot.item;
        // 
        //             split_function(slot, split_slot);
        //         }
        // 
        //         public void combine(int from, int to, TezEventBus.Action<T, T> combine)
        //         {
        //             var slot_from = slots[from];
        //             var slot_to = slots[to];
        // 
        //             combine(slot_from, slot_to);
        //         }
        // 
        //         public void swap(int slot_1, int slot_2)
        //         {
        //             var slot1 = slots[slot_1];
        //             var slot2 = slots[slot_2];
        // 
        //             var temp = slot1.item;
        //             slot1.item = slot2.item;
        //             slot2.item = temp;
        //         }
    }

#if false
    public abstract class TezStorage
    {
        public List<TezStorageSlot> slots { get; private set; } = new List<TezStorageSlot>();
        protected Queue<TezStorageSlot> m_EmptySlots = new Queue<TezStorageSlot>();

        public int count
        {
            get { return slots.Count; }
        }

        public ITezItem this[int index]
        {
            get { return slots[index].item; }
        }

        protected T createSlot<T>() where T : TezStorageSlot, new()
        {
            if (m_EmptySlots.Count > 0)
            {
                return (T)m_EmptySlots.Dequeue();
            }
            else
            {
                var slot = new T();
                slot.id = slots.Count;
                slots.Add(slot);
                return slot;
            }
        }

        public void grow<T>(int count) where T : TezStorageSlot, new()
        {
            while (slots.Count <= count)
            {
                var slot = new T();
                slot.id = slots.Count;
                slots.Add(slot);
            }
        }

        public void add<T>(TezEventBus.Action<T> set_slot) where T : TezStorageSlot, new()
        {
            var slot = this.createSlot<T>();
            set_slot(slot);
            this.onHandleAdd(slot);
        }
        protected abstract void onHandleAdd<T>(T slot) where T : TezStorageSlot, new();

        public void set<T>(int slot_id, TezEventBus.Action<T> set_slot) where T : TezStorageSlot, new()
        {
            var slot = (T)slots[slot_id];
            set_slot(slot);
            this.onHandleSet(slot);
        }
        protected abstract void onHandleSet<T>(T slot) where T : TezStorageSlot, new();


        public void remove<T>(int slot_id) where T : TezStorageSlot, new()
        {
            var slot = (T)slots[slot_id];
            slot.item = null;
            slot.next_split = null;
            this.onHandleRemove(slot);
        }
        protected abstract void onHandleRemove<T>(T slot) where T : TezStorageSlot, new();

        public void split<T>(int slot_id, TezEventBus.Action<T, T> split_function) where T : TezStorageSlot, new()
        {
            var slot = (T)slots[slot_id];

            var split_slot = this.createSlot<T>();
            split_slot.item = slot.item;

            split_function(slot, split_slot);
        }

        public void combine<T>(int from, int to, TezEventBus.Action<T, T> combine) where T : TezStorageSlot
        {
            var slot_from = slots[from];
            var slot_to = slots[to];

            combine((T)slot_from, (T)slot_to);
        }

        public void swap(int slot_1, int slot_2)
        {
            var slot1 = slots[slot_1];
            var slot2 = slots[slot_2];

            var temp = slot1.item;
            slot1.item = slot2.item;
            slot2.item = temp;
        }

        public bool tryGet<T>(int slot_id, out T slot) where T : TezStorageSlot
        {
            slot = default(T);

            if (slot_id >= slots.Count)
            {
                return false;
            }

            slot = (T)slots[slot_id];
            if (slot.item == null)
            {
                return false;
            }

            return true;
        }

        public void foreachSlot<T>(TezEventBus.Action<T> function) where T : TezStorageSlot
        {
            foreach (var slot in slots)
            {
                function((T)slot);
            }
        }

        public void clear()
        {
            foreach (var slot in slots)
            {
                slot.item = null;
            }

            slots.Clear();
        }
    }
#endif
}