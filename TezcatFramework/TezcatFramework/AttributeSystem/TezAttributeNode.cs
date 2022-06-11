using System;
using System.Collections.Generic;

namespace tezcat.Framework.Attribute
{
    /// <summary>
    /// 属性节点
    /// 用来制作Path和Leaf
    /// </summary>
    public abstract class TezAttributeNode : ITezAttributeNode
    {
        #region Pool
        static Queue<List<ITezAttributeDefObject>> ListPool = new Queue<List<ITezAttributeDefObject>>();
        static List<ITezAttributeDefObject> create()
        {
            if (ListPool.Count > 0)
            {
                return ListPool.Dequeue();
            }

            return new List<ITezAttributeDefObject>();
        }

        static void recycle(List<ITezAttributeDefObject> list)
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
        public ITezAttributeTree system { get; private set; } = null;
        public abstract TezAttributeNodeType nodeType { get; }

        List<ITezAttributeDefObject> m_AttributeObjects = null;

        protected TezAttributeNode(int id, ITezAttributeTree system)
        {
            this.ID = id;
            this.system = system;
        }

        public virtual void onRegisterObject(ITezAttributeHandler handler)
        {
            if (m_AttributeObjects != null)
            {
                for (int i = 0; i < m_AttributeObjects.Count; i++)
                {
                    handler.addAttributeDefObject(m_AttributeObjects[i]);
                }
            }
        }

        public virtual void onUnregisterObject(ITezAttributeHandler handler)
        {
            if (m_AttributeObjects != null)
            {
                for (int i = 0; i < m_AttributeObjects.Count; i++)
                {
                    handler.removeAttributeDefObject(m_AttributeObjects[i]);
                }
            }
        }

        /// <summary>
        /// 添加一个AttributeObject
        /// </summary>
        public void addAttributeDefObject(ITezAttributeDefObject def_object)
        {
            if (m_AttributeObjects == null)
            {
                m_AttributeObjects = create();
            }
            m_AttributeObjects.Add(def_object);
            this.addAttributeDefObjectToChildren(def_object);
        }

        /// <summary>
        /// 向子节点添加
        /// </summary>
        public abstract void addAttributeDefObjectToChildren(ITezAttributeDefObject def_object);

        public void removeAttributeDefObject(ITezAttributeDefObject def_object)
        {
            if (m_AttributeObjects.Remove(def_object))
            {
                this.removeAttributeDefObjectFromChildren(def_object);
            }
            else
            {
                throw new Exception("removeDefinitionObject!!");
            }
        }

        /// <summary>
        /// 从子节点移除
        /// </summary>
        public abstract void removeAttributeDefObjectFromChildren(ITezAttributeDefObject def_object);

        public virtual void close()
        {
            recycle(m_AttributeObjects);
            m_AttributeObjects = null;
            this.system = null;
        }
    }
}