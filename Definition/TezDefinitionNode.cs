using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionNode : ITezDefinitionNode
    {
        #region Pool
        static Queue<List<ITezDefinitionObject>> ListPool = new Queue<List<ITezDefinitionObject>>();
        static List<ITezDefinitionObject> create()
        {
            if (ListPool.Count > 0)
            {
                return ListPool.Dequeue();
            }

            return new List<ITezDefinitionObject>();
        }

        static void recycle(List<ITezDefinitionObject> list)
        {
            if (list == null)
            {
                return;
            }
            list.Clear();
            ListPool.Enqueue(list);
        }
        #endregion

        public int ID { get; }
        public TezDefinitionSystem system { get; private set; } = null;
        public abstract TezDefinitionNodeType nodeType { get; }

        List<ITezDefinitionObject> m_DefinitionObjects = null;

        protected TezDefinitionNode(int id, TezDefinitionSystem set)
        {
            this.ID = id;
            this.system = set;
        }

        public virtual void onRegisterObject(ITezDefinitionHandler handler)
        {
            if (m_DefinitionObjects != null)
            {
                for (int i = 0; i < m_DefinitionObjects.Count; i++)
                {
                    handler.addDefinitionObject(m_DefinitionObjects[i]);
                }
            }
        }

        public virtual void onUnregisterObject(ITezDefinitionHandler handler)
        {
            if (m_DefinitionObjects != null)
            {
                for (int i = 0; i < m_DefinitionObjects.Count; i++)
                {
                    handler.removeDefinitionObject(m_DefinitionObjects[i]);
                }
            }
        }

        public void addDefinitionObject(ITezDefinitionObject def_object)
        {
            if (m_DefinitionObjects == null)
            {
                m_DefinitionObjects = create();
            }
            m_DefinitionObjects.Add(def_object);
            this.onAddCustomData(def_object);
        }

        protected abstract void onAddCustomData(ITezDefinitionObject def_object);

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            if (m_DefinitionObjects.Remove(def_object))
            {
                this.onRemoveCustomData(def_object);
            }
            else
            {
                throw new System.Exception("removeDefinitionObject!!");
            }
        }

        protected abstract void onRemoveCustomData(ITezDefinitionObject def_object);

        public virtual void close()
        {
            recycle(m_DefinitionObjects);
            m_DefinitionObjects = null;
            this.system = null;
        }
    }
}