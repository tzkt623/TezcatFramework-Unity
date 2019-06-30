namespace tezcat.Framework.AI
{
    public class TezBehaviorTree<Data> where Data : ITezBTData
    {
        public TezBTResult state { get; private set; } = TezBTResult.Empty;

        TezBTNode<Data> m_RootNode = null;

        public void setNode(TezBTNode<Data> node)
        {
            m_RootNode = node;
        }

        public TezBTResult execute(Data data)
        {
            state = m_RootNode.execute(data);
            switch (state)
            {
                case TezBTResult.Fail:
                    this.onFail(data);
                    break;
                case TezBTResult.Success:
                    this.onSuccess(data);
                    break;
                case TezBTResult.Running:
                    this.onRunning(data);
                    break;
                default:
                    break;
            }

            return state;
        }

        public virtual void close()
        {
            m_RootNode.close();
            m_RootNode = null;
        }

        protected virtual void onSuccess(Data data)
        {

        }

        protected virtual void onFail(Data data)
        {

        }

        protected virtual void onRunning(Data data)
        {

        }
    }
}