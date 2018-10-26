using System.Collections.Generic;
using tezcat.Event;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezStateSet
    {
        public static readonly TezStateTag Empty = TezStateTag.register();
        public static readonly TezStateTag Pause = TezStateTag.register();
        public static readonly TezStateTag PickAnItem = TezStateTag.register();
        public static readonly TezStateTag PickAnObject = TezStateTag.register();
    }

    public class TezStateTag
    {
        static TezStateTag[] Cache = new TezStateTag[33];

        static int IDGiver = 0;
        public int ID { get; private set; } = 0;

        static TezStateTag()
        {
            for (int i = 0; i < Cache.Length; i++)
            {
                Cache[i] = new TezStateTag(i);
            }
        }

        private TezStateTag(int index)
        {
            this.ID = 1 << index;
        }

        public static bool operator == (TezStateTag a, TezStateTag b)
        {
            return a.ID == b.ID;
        }

        public static bool operator != (TezStateTag a, TezStateTag b)
        {
            return a.ID != b.ID;
        }

        public static int operator |(TezStateTag a, TezStateTag b)
        {
            return a.ID | b.ID;
        }

        public static int operator &(TezStateTag a, TezStateTag b)
        {
            return a.ID & b.ID;
        }

        public static int operator ~(TezStateTag state)
        {
            return ~state.ID;
        }

        public static implicit operator int(TezStateTag state)
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

        public static TezStateTag register()
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
        public static TezAction onStateChanged { get; private set; } = new TezAction();
        static Stack<int> m_PreState = new Stack<int>();

        public 

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

        public static void locking(int state, TezEventExtension.Action function)
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