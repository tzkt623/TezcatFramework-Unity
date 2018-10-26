using tezcat.Framework.Core;


namespace tezcat.Framework.AI
{
    public class TezAIBrain : ITezCloseable
    {
        TezAIBehaviour m_Root = null;

        public void close()
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
