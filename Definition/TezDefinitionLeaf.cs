using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionLeaf : TezDefinitionNode
    {
        public sealed override TezDefinitionNodeType nodeType { get; } = TezDefinitionNodeType.Leaf;

        List<ITezDefinitionHandler> m_Objects = new List<ITezDefinitionHandler>();

        protected TezDefinitionLeaf(int id, ITezDefinitionSystem system) : base(id, system)
        {

        }

        public override void onRegisterObject(ITezDefinitionHandler handler)
        {
            base.onRegisterObject(handler);
            m_Objects.Add(handler);
        }

        public override void onUnregisterObject(ITezDefinitionHandler handler)
        {
            base.onUnregisterObject(handler);
            m_Objects.Remove(handler);
        }

        public override void addDefinitionObjectToChildren(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].addDefinitionObject(def_object);
            }
        }

        public override void removeDefinitionObjectFromChildren(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].removeDefinitionObject(def_object);
            }
        }

        public override void close()
        {
            base.close();
            m_Objects.Clear();
            m_Objects = null;
        }
    }
}