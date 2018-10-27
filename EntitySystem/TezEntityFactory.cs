using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using tezcat.Framework.Utility;

namespace tezcat.Framework.EntitySystem
{
    public class TezEntityFactory<TMask>
        : ITezService
        where TMask : ITezMask
    {
        static int m_ComponentIDGiver = 0;
        static int m_EntityIDGiver = 0;

        class ComponentID<T> : TezTypeInfo<T, TezEntityFactory<TMask>>
        {

        }

        struct EntityState
        {
            public TMask componentQuery;
            public TMask componentDirty;
        }

        class ComponentManager
        {
            public ITezComponent[] components = null;

            public void close()
            {
                components = null;
            }
        }

        public int capacity { get; private set; }
        int m_EntityComponentCount = 32;
        EntityState[] m_EntityStates = null;

        TMask[] m_ComponentQuery = null;
        ComponentManager[] m_ComponentManagers = null;
        Queue<int> m_FreeID = new Queue<int>();

        public TezEntityFactory()
        {
            m_ComponentManagers = new ComponentManager[m_EntityComponentCount];
        }

        public void setup(int capacity)
        {
            this.capacity = capacity;
            m_ComponentQuery = new TMask[this.capacity];
            for (int i = 0; i < m_EntityComponentCount; i++)
            {
                m_ComponentManagers[i].components = new ITezComponent[this.capacity];
            }
        }

        public TezEntity createEntity()
        {
            if (m_FreeID.Count > 0)
            {
                return new TezEntity(m_FreeID.Dequeue());
            }
            else
            {
                return new TezEntity(m_EntityIDGiver++);
            }
        }

        public void recycleEntity(TezEntity entity)
        {

        }

        public Component addComponent<Component>(TezEntity entity) where Component : class, ITezComponent, new()
        {
            switch (ComponentID<Component>.ID)
            {
                case TezTypeInfo.ErrorID:
                    ComponentID<Component>.setID(m_ComponentIDGiver++);
                    break;
                default:
                    break;
            }

            var component = new Component();
            m_ComponentManagers[ComponentID<Component>.ID].components[entity.ID] = component;
            return component;
        }

        public Component getComponent<Component>(TezEntity entity) where Component : class, ITezComponent, new()
        {
            var id = ComponentID<Component>.ID;
            switch (id)
            {
                case TezTypeInfo.ErrorID:
                    return null;
                default:
                    return (Component)m_ComponentManagers[id].components[entity.ID];
            }
        }

        public void removeComponent<Component>(TezEntity entity) where Component : class, ITezComponent, new()
        {
            m_ComponentManagers[ComponentID<Component>.ID].components[entity.ID] = null;
        }

        public void close()
        {

        }
    }
}

