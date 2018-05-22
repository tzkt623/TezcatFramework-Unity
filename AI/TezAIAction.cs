﻿using System.Collections.Generic;
using tezcat.Core;

namespace tezcat.AI
{
    public abstract class TezAIBehaviour : ITezClearable
    {
        public TezAIState state { get; protected set; }

        public int ID { get; protected set; }

        public string name { get; protected set; }

        public abstract void enter();

        public abstract TezAIState executing(TezAICollection collection);

        public abstract void exit();

        public abstract void clear();

        public abstract bool evaluate(TezAICollection collection);


        public static bool operator true(TezAIBehaviour obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezAIBehaviour obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezAIBehaviour obj)
        {
            return object.ReferenceEquals(obj, null);
        }
    }


    public abstract class TezAIComposite : TezAIBehaviour
    {
        protected List<TezAIBehaviour> m_Children = new List<TezAIBehaviour>();
        protected TezAIState m_CurrentState = TezAIState.Failure;

        public void add(TezAIBehaviour action)
        {
            m_Children.Add(action);
        }

        public override void clear()
        {
            foreach (var item in m_Children)
            {
                item.clear();
            }

            m_Children.Clear();
        }
    }

    public abstract class TezAIDecorator : TezAIBehaviour
    {
        TezAIBehaviour m_Behaviour = null;

        public void setBehaviour(TezAIBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }
    }
}