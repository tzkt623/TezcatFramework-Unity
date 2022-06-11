using System.Collections.Generic;

namespace tezcat.Framework.Attribute
{
    public abstract class TezAttributeLeaf : TezAttributeNode
    {
        public sealed override TezAttributeNodeType nodeType { get; } = TezAttributeNodeType.Leaf;

        List<ITezAttributeHandler> m_Objects = new List<ITezAttributeHandler>();

        protected TezAttributeLeaf(int id, ITezAttributeTree system) : base(id, system)
        {

        }

        public override void onRegisterObject(ITezAttributeHandler handler)
        {
            base.onRegisterObject(handler);
            m_Objects.Add(handler);
        }

        public override void onUnregisterObject(ITezAttributeHandler handler)
        {
            base.onUnregisterObject(handler);
            m_Objects.Remove(handler);
        }

        public override void addAttributeDefObjectToChildren(ITezAttributeDefObject def_object)
        {
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].addAttributeDefObject(def_object);
            }
        }

        public override void removeAttributeDefObjectFromChildren(ITezAttributeDefObject def_object)
        {
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].removeAttributeDefObject(def_object);
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