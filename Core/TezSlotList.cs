using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezSlotHandler<Slot> where Slot : TezSlot
    {
        void onAddSlot(Slot slot);
        void onRemoveSlot(Slot slot);
        void onRemakeID(int old_id, Slot slot);
    }

    public class TezSlotHandler<T> : ITezSlotHandler<T> where T : TezSlot
    {
        void ITezSlotHandler<T>.onAddSlot(T slot)
        {

        }

        void ITezSlotHandler<T>.onRemakeID(int old_id, T slot)
        {

        }

        void ITezSlotHandler<T>.onRemoveSlot(T slot)
        {

        }
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
    public abstract class TezSlotList<T> where T : TezSlot, new()
    {
        public List<T> slots { get; private set; } = new List<T>();

        public int slotCount
        {
            get { return slots.Count; }
        }

        public T this[int slot_id]
        {
            get { return slots[slot_id]; }
            set { slots[slot_id] = value; }
        }

        public void setCapacity(int count)
        {
            slots.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                var slot = new T();
                slot.ID = slots.Count;
                this.slots.Add(slot);
            }
        }

        public void grow(int count)
        {
            while (slots.Count <= count)
            {
                var slot = new T();
                slot.ID = slots.Count;
                slots.Add(slot);
            }
        }

        protected T add()
        {
            var slot = new T();
            slot.ID = slots.Count;
            slots.Add(slot);
            return slot;
        }

        protected T remove(int slot_id)
        {
            var temp = this.slots[slot_id];
            this.slots.RemoveAt(slot_id);
            this.remakeID();
            return temp;
        }

        private void remakeID()
        {
            for (int i = 0; i < this.slots.Count; i++)
            {
                var old = slots[i].ID;
                slots[i].ID = i;
                this.onRemakeID(old, slots[i]);
            }
        }

        protected virtual void onRemakeID(int old_id, T slot)
        {

        }

        public bool tryGet(int slot_id, out T slot)
        {
            if (slot_id >= slots.Count)
            {
                slot = null;
                return false;
            }

            slot = slots[slot_id];
            return true;
        }

        public void foreachSlot(TezEventExtension.Action<T> function)
        {
            int count = this.slots.Count;
            for (int i = 0; i < count; i++)
            {
                function(this.slots[i]);
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