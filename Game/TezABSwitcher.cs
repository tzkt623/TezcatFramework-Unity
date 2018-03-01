using System.Collections.Generic;
namespace tezcat
{
    public class TezABSwitcher<A, B>
    {
        Dictionary<A, B> m_ToB = new Dictionary<A, B>();
        Dictionary<B, A> m_ToA = new Dictionary<B, A>();

        public void register(A a, B b)
        {
            m_ToA.Add(b, a);
            m_ToB.Add(a, b);
        }

        public A convertToA(B b)
        {
            return m_ToA[b];
        }

        public B convertToB(A a)
        {
            return m_ToB[a];
        }
    }
}
