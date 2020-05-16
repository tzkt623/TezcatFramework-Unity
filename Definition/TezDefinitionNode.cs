using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionNode : ITezDefinitionNode
    {
        public int ID { get; }
        public TezDefinitionSystem system { get; private set; } = null;
        public abstract TezDefinitionNodeType nodeType { get; }

        List<ITezDefinitionObject> m_ObjectList = null;

        protected TezDefinitionNode(int id, TezDefinitionSystem set)
        {
            this.ID = id;
            this.system = set;
        }

        public virtual void onRegisterObject(ITezDefinitionHandler handler)
        {
            if (m_ObjectList != null)
            {
                for (int i = 0; i < m_ObjectList.Count; i++)
                {
                    handler.addDefinitionObject(m_ObjectList[i]);
                }
            }
        }

        public virtual void onUnregisterObject(ITezDefinitionHandler handler)
        {
            if (m_ObjectList != null)
            {
                for (int i = 0; i < m_ObjectList.Count; i++)
                {
                    handler.removeDefinitionObject(m_ObjectList[i]);
                }
            }
        }

        public void addDefinitionObject(ITezDefinitionObject def_object)
        {
            if (m_ObjectList == null)
            {
                m_ObjectList = new List<ITezDefinitionObject>();
            }
            m_ObjectList.Add(def_object);
            this.onAddCustomData(def_object);
        }

        protected abstract void onAddCustomData(ITezDefinitionObject def_object);

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            m_ObjectList.Remove(def_object);
            this.onRemoveCustomData(def_object);
        }

        protected abstract void onRemoveCustomData(ITezDefinitionObject def_object);

        public virtual void close(bool self_close = true)
        {
            m_ObjectList?.Clear();
            m_ObjectList = null;
            this.system = null;
        }
    }
}