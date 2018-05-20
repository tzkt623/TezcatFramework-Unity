using System.Collections.Generic;
using tezcat.Core;


namespace tezcat.AI
{
    public class TezAIBrain : ITezClearable
    {
        HashSet<TezAICondition> m_ConditionSet = new HashSet<TezAICondition>();

        public void clear()
        {

        }

        public void think()
        {

        }

        public void beginBehaviour(string name)
        {

        }

        public void endBehaviour()
        {

        }

        public void addCondition(TezAICondition condition)
        {
            m_ConditionSet.Add(condition);
        }
    }
}
