using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public abstract class TezAIBehaviour : ITezCloseable
    {
        public TezAIState state { get; protected set; }

        public int ID { get; protected set; }

        public string name { get; protected set; }

        public abstract void enter();

        public abstract TezAIState executing(TezAICollection collection);

        public abstract void exit();

        public abstract void close();

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

        public override void close()
        {
            foreach (var item in m_Children)
            {
                item.close();
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