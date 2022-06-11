using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Attribute
{
    /// <summary>
    /// <para>AttributeDef构建器</para>
    /// <para>自身可以在运行时按照编辑好的格式构建RTTI信息</para>
    /// 
    /// <para>也用于AttributeTree中,进行对DefObject的反馈操作</para>
    /// </summary>
    public class TezAttributeBuilder
        : ITezAttributeBuilder
        , ITezCloseable
    {
        #region Factory
        static List<Tuple<List<ITezAttributeToken>, List<ITezAttributeToken>>> m_Factory = new List<Tuple<List<ITezAttributeToken>, List<ITezAttributeToken>>>();
        static Queue<int> m_FreeIndex = new Queue<int>();

        private static int alloc()
        {
            if (m_FreeIndex.Count > 0)
            {
                return m_FreeIndex.Dequeue();
            }

            var index = m_Factory.Count;
            var temp = new Tuple<List<ITezAttributeToken>, List<ITezAttributeToken>>(new List<ITezAttributeToken>(),
                 new List<ITezAttributeToken>());
            m_Factory.Add(temp);
            return index;
        }

        private static void free(int index)
        {
            m_FreeIndex.Enqueue(index);
            var tuple = m_Factory[index];
            tuple.Item1.Clear();
            tuple.Item2.Clear();
        }

        private static void addPrimaryToken(int index, ITezAttributeToken definitionToken)
        {
            m_Factory[index].Item1.Add(definitionToken);
        }

        private static void addSecondaryToken(int index, ITezAttributeToken definitionToken)
        {
            m_Factory[index].Item2.Add(definitionToken);
        }

        private static List<ITezAttributeToken> getPrimaryTokens(int index)
        {
            return m_Factory[index].Item1;
        }

        private static List<ITezAttributeToken> getSecondaryTokens(int index)
        {
            return m_Factory[index].Item2;
        }
        #endregion

        public TezAttributeDef definition { get; private set; } = null;

        TezEventExtension.Action<ITezAttributeDefObject> m_OnAddDefinitionObject = null;
        TezEventExtension.Action<ITezAttributeDefObject> m_OnRemoveDefinitionObject = null;
        int m_Index = -1;

        public void setListener(TezEventExtension.Action<ITezAttributeDefObject> onAddDefinitionObject, TezEventExtension.Action<ITezAttributeDefObject> onRemoveDefinitionObject)
        {
            m_OnAddDefinitionObject = onAddDefinitionObject;
            m_OnRemoveDefinitionObject = onRemoveDefinitionObject;
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="def_object"></param>
        public void addAttributeDefObject(ITezAttributeDefObject def_object)
        {
            m_OnAddDefinitionObject(def_object);
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="def_object"></param>
        public void removeAttributeDefObject(ITezAttributeDefObject def_object)
        {
            m_OnRemoveDefinitionObject(def_object);
        }

        public void addPrimaryToken(ITezAttributeToken definitionToken)
        {
            addPrimaryToken(m_Index, definitionToken);
        }

        public void addSecondaryToken(ITezAttributeToken definitionToken)
        {
            addSecondaryToken(m_Index, definitionToken);
        }

        public void beginPath()
        {
            m_Index = alloc();
        }

        public void endPath()
        {
            var p = getPrimaryTokens(m_Index);
            var s = getSecondaryTokens(m_Index);

            this.definition = new TezAttributeDef()
            {
                primaryPath = p.Count > 0 ? p.ToArray() : null,
                secondaryPath = s.Count > 0 ? s.ToArray() : null
            };

            free(m_Index);
            m_Index = -1;
        }

        public void close()
        {
            this.definition.close();
            this.definition = null;
            m_OnAddDefinitionObject = null;
            m_OnRemoveDefinitionObject = null;
        }
    }
}