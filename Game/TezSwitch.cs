using System.Collections.Generic;

namespace tezcat
{
    public class TezSwitch
    {
        List<TezEventBus.Action> m_ActionList = new List<TezEventBus.Action>();

        public void register(int state, TezEventBus.Action function)
        {
            while (m_ActionList.Count <= state)
            {
                m_ActionList.Add(null);
            }

            m_ActionList[state] = function;
        }

        public void runCase(int state)
        {
            m_ActionList[state]();
        }

        void clear()
        {
            m_ActionList.Clear();
        }
    }
}