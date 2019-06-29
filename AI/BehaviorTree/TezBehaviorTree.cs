namespace tezcat.Framework.AI
{
    public class TezBehaviorTree<Data>
        : TezBTCompositeNode<Data>
        where Data : ITezBTData
    {
        public TezBTResult state { get; private set; } = TezBTResult.Empty;

        TezBTNode<Data> m_Root = null;

        public override void addNode(TezBTNode<Data> node)
        {
            m_Root = node;
        }

        public override TezBTResult execute(Data data)
        {
            state = m_Root.execute(data);
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

        public override void close()
        {
            m_Root.close();
            m_Root = null;
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