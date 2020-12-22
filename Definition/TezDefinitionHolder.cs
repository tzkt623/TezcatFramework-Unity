using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Definition
{
    public class TezDefinitionHolder
        : ITezDefinitionObjectAndHandler
        , ITezCloseable
    {
        #region Factory
        static List<Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>> m_Factory = new List<Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>>();
        static Queue<int> m_FreeIndex = new Queue<int>();

        private static int alloc()
        {
            if (m_FreeIndex.Count > 0)
            {
                return m_FreeIndex.Dequeue();
            }

            var index = m_Factory.Count;
            var temp = new Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>(new List<ITezDefinitionToken>(),
                 new List<ITezDefinitionToken>());
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

        private static void addPrimaryToken(int index, ITezDefinitionToken definitionToken)
        {
            m_Factory[index].Item1.Add(definitionToken);
        }

        private static void addSecondaryToken(int index, ITezDefinitionToken definitionToken)
        {
            m_Factory[index].Item2.Add(definitionToken);
        }

        private static List<ITezDefinitionToken> getPrimaryTokens(int index)
        {
            return m_Factory[index].Item1;
        }

        private static List<ITezDefinitionToken> getSecondaryTokens(int index)
        {
            return m_Factory[index].Item2;
        }
        #endregion

        public TezDefinition definition { get; private set; } = null;

        TezEventExtension.Action<ITezDefinitionObject> m_OnAddDefinitionObject = null;
        TezEventExtension.Action<ITezDefinitionObject> m_OnRemoveDefinitionObject = null;
        int m_Index = -1;

        public void setListener(TezEventExtension.Action<ITezDefinitionObject> onAddDefinitionObject, TezEventExtension.Action<ITezDefinitionObject> onRemoveDefinitionObject)
        {
            m_OnAddDefinitionObject = onAddDefinitionObject;
            m_OnRemoveDefinitionObject = onRemoveDefinitionObject;
        }

        public void addDefinitionObject(ITezDefinitionObject def_object)
        {
            m_OnAddDefinitionObject(def_object);
        }

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            m_OnRemoveDefinitionObject(def_object);
        }

        public void addPrimaryToken(ITezDefinitionToken definitionToken)
        {
            addPrimaryToken(m_Index, definitionToken);
        }

        public void addSecondaryToken(ITezDefinitionToken definitionToken)
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

            this.definition = new TezDefinition()
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