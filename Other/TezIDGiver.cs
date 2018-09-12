using System.Collections.Generic;

namespace tezcat
{
    public class TezIDGiver
    {
        int m_ID = 0;
        Stack<int> m_Free = new Stack<int>();

        public int giveID()
        {
            if(m_Free.Count > 0)
            {
                return m_Free.Pop();
            }
            else
            {
                return m_ID++;
            }
        }

        public void recycleID(int id)
        {
            m_Free.Push(id);
        }
    }
}