using System.Collections.Generic;
using tezcat.Event;

namespace tezcat.Core
{
    public class TezState
    {
        static TezState[] Cache = new TezState[33];

        static int IDGiver = 0;
        public int ID { get; private set; } = 0;

        static TezState()
        {
            for (int i = 0; i < Cache.Length; i++)
            {
                Cache[i] = new TezState(i);
            }
        }

        private TezState(int index)
        {
            this.ID = 1 << index;
        }

        public static bool operator == (TezState a, TezState b)
        {
            return a.ID == b.ID;
        }

        public static bool operator != (TezState a, TezState b)
        {
            return a.ID != b.ID;
        }

        public static int operator |(TezState a, TezState b)
        {
            return a.ID | b.ID;
        }

        public static int operator &(TezState a, TezState b)
        {
            return a.ID & b.ID;
        }

        public static int operator ~(TezState state)
        {
            return ~state.ID;
        }

        public static implicit operator int(TezState state)
        {
            return state.ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static TezState register()
        {
            if(IDGiver > 33)
            {
                return null;
            }

            return Cache[IDGiver++];
        }
    }

    public class TezStateController
    {
        public static TezEvent onStateChanged { get; private set; } = new TezEvent();
        static Stack<int> m_PreState = new Stack<int>();

        static int m_States = 0;
        public static int current
        {
            get { return m_States; }
        }

        public static void push(int state)
        {
            m_PreState.Push(m_States);
            m_States = state;
            onStateChanged.invoke();
        }

        public static void pop()
        {
            m_States = m_PreState.Pop();
            onStateChanged.invoke();
        }

        public static void add(int state)
        {
            m_States |= state;
            onStateChanged.invoke();
        }

        public static void remove(int state)
        {
            m_States &= ~state;
            onStateChanged.invoke();
        }

        public static void locking(int state, TezEventBus.Action function)
        {
            if((m_States & state) == state)
            {
                function();
            }
        }

        public static bool locking(int state)
        {
            return (m_States & state) == state;
        }
    }
}