using tezcat.Core;


namespace tezcat.AI
{
    public class TezAIBrain : ITezClearable
    {
        TezAIBehaviour m_Root = null;

        public void clear()
        {

        }

        public void setBehaviour(TezAIBehaviour behaviour)
        {
            m_Root = behaviour;
        }

        public void executing(TezAICollection collection)
        {
            if(m_Root.evaluate(collection))
            {
                m_Root.executing(collection);
            }
        }
    }
}
