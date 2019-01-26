using System.Collections.Generic;
using tezcat.Framework.ECS;
using tezcat.Framework.Wrapper;

namespace tezcat.Framework.Core
{
    public abstract class TezBasicSlot<T>
        : TezGameObjectWrapper<T>
        where T : TezGameObject
    {
        public int slotID { get; set; } = -1;

        public bool theSameAs(T obj)
        {
            return this.myGameObject.sameAs(obj);
        }
    }

    public abstract class TezBasicSlotCollection : ITezCloseable
    {
        /// <summary>
        /// 容量
        /// 
        /// 为-1时可以自动增长
        /// 为固定值时为固定容量
        /// </summary>
        public int capacity { get; set; }
        public abstract int count { get; }

        public abstract void addSlot();
        public abstract void removeSlot();
        public abstract void setSlots(int count);

        public abstract class Slot
        {
            public int slotID { get; }
            public Slot(int id)
            {
                this.slotID = id;
            }
        }

        public abstract void close();
    }

    public abstract class TezListSlotCollection<T, Slot>
        : TezBasicSlotCollection
        where Slot : TezBasicSlot<T>, new()
        where T : TezGameObject
    {
        protected List<Slot> m_Slots = new List<Slot>();

        public Slot this[int index]
        {
            get { return m_Slots[index]; }
        }

        public override int count
        {
            get { return m_Slots.Count; }
        }

        public override void addSlot()
        {
            var slot = new Slot();
            slot.slotID = m_Slots.Count;
            m_Slots.Add(slot);
        }

        public override void removeSlot()
        {
            m_Slots.RemoveAt(m_Slots.Count - 1);
        }

        public override void setSlots(int count)
        {
            if (m_Slots.Count < count)
            {
                while (m_Slots.Count < count)
                {
                    this.addSlot();
                }
            }
            else if (m_Slots.Count > count)
            {
                m_Slots.RemoveRange(count, m_Slots.Count - count);
            }
        }

        protected virtual int find(T item)
        {
            int result = -1;

            for (int i = 0; i < m_Slots.Count; i++)
            {
                if (result == -1 && m_Slots[i].myGameObject == null)
                {
                    result = i;
                }

                if (m_Slots[i].myGameObject != null && m_Slots[i].myGameObject.sameAs(item))
                {
                    return i;
                }
            }

            return result;
        }


        public override void close()
        {
            m_Slots.Clear();
        }
    }
}