using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezPropertyContainer : ITezCloseable
    {
        public abstract T create<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new();
        public abstract bool remove(ITezValueDescriptor descriptor);
        public abstract void clear();
        public abstract void close();

        public abstract ITezProperty get(int id);
        public abstract ITezProperty get(ITezValueDescriptor vd);

        public abstract bool tryGet(int id, out ITezProperty property);
        public virtual bool tryGet(ITezValueDescriptor vd, out ITezProperty property)
        {
            return this.tryGet(vd.ID, out property);
        }
    }

    public class TezPropertyListContainer : TezPropertyContainer
    {
        List<ITezProperty> m_List = new List<ITezProperty>();

        public override T create<T>(ITezValueDescriptor descriptor)
        {
            var id = descriptor.ID;
            if (m_List.Count <= id)
            {
                m_List.AddRange(new ITezProperty[id - m_List.Count + 1]);
            }

            T property = (T)m_List[id];
            if (property != null)
            {
                return property;
            }

            property = new T();
            property.descriptor = descriptor;
            m_List[id] = property;
            return property;
        }

        public override bool remove(ITezValueDescriptor descriptor)
        {
            var id = descriptor.ID;
            var result = m_List[id];

            if (result != null)
            {
                result.close();
                m_List[id] = null;
                return true;
            }

            return false;
        }

        public override void clear()
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i]?.close();
            }

            m_List.Clear();
        }

        public override void close()
        {
            this.clear();
            m_List = null;
        }

        public override ITezProperty get(int id)
        {
            return m_List[id];
        }

        public override ITezProperty get(ITezValueDescriptor vd)
        {
            return m_List[vd.ID];
        }

        public override bool tryGet(int id, out ITezProperty property)
        {
            property = m_List[id];
            return property != null;
        }
    }

    public class TezPropertySortListContainer : TezPropertyContainer
    {
        TezValueSortList<ITezProperty> m_List = new TezValueSortList<ITezProperty>(4);

        public override T create<T>(ITezValueDescriptor descriptor)
        {
            if (m_List.binaryFind(descriptor.ID, out int index))
            {
                return (T)m_List[index];
            }
            else
            {
                var property = new T();
                property.descriptor = descriptor;
                m_List.insert(index, property);
                return property;
            }
        }

        public override void clear()
        {
            for (int i = 0; i < m_List.count; i++)
            {
                m_List[i].close();
            }

            m_List.clear();
        }

        public override void close()
        {
            this.clear();
            m_List = null;
        }

        public override ITezProperty get(int id)
        {
            return m_List.binaryFind(id);
        }

        public override ITezProperty get(ITezValueDescriptor vd)
        {
            return m_List.binaryFind(vd.ID);
        }

        public override bool remove(ITezValueDescriptor descriptor)
        {
            if (m_List.binaryFind(descriptor.ID, out int index))
            {
                m_List.removeAt(index);
                return true;
            }

            return false;
        }

        public override bool tryGet(int id, out ITezProperty property)
        {
            if (m_List.binaryFind(id, out int index))
            {
                property = m_List[id];
                return true;
            }

            property = null;
            return false;
        }
    }
}