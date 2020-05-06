using tezcat.Framework.Utility;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 队列行为
    /// 
    /// <para>
    /// 子节点按顺序执行
    /// 只要有节点返回Fail或者Running则返回
    /// 如果是Running则记录当前节点并继续执行
    /// </para>
    /// </summary>
    public class TezBTSequenceNode<Data>
        : TezBTCompositeNode<Data>
        where Data : ITezBTData
    {
        public sealed override TezBTNodeType nodeType => TezBTNodeType.Sequence;

        int m_CurrentRunning = -1;
        TezArray<TezBTNode<Data>> m_Nodes = new TezArray<TezBTNode<Data>>(0);

        public override void addNode(TezBTNode<Data> node)
        {
            m_Nodes.add(node);
        }

        public override void close(bool self_close = true)
        {
            m_Nodes.close(false);
            m_Nodes = null;
        }

        public override TezBTResult execute(Data data)
        {
            TezBTResult state = TezBTResult.Success;
            int index = m_CurrentRunning == -1 ? 0 : m_CurrentRunning;
            while(index < m_Nodes.count)
            {
                state = m_Nodes[index].execute(data);
                switch (state)
                {
                    case TezBTResult.Success:
                        break;
                    case TezBTResult.Running:
                        m_CurrentRunning = index;
                        return TezBTResult.Running;
                    case TezBTResult.Fail:
                        m_CurrentRunning = -1;
                        return TezBTResult.Fail;
                    default:
                        break;
                }
                index += 1;
            }

            ///全部执行完
            m_CurrentRunning = -1;
            return state;
        }
    }
}