using tezcat.Framework.Utility;

namespace tezcat.Framework.AI
{

    /// <summary>
    /// 并行节点
    /// 并行运行所有子行为
    /// <para>
    /// 与模式下
    /// 只要有一个节点返回Fail
    /// 则整个节点停止执行
    /// 并且返回Fail
    /// </para>
    /// <para>
    /// 或模式下
    /// 必须要全部节点返回Fail
    /// 整个节点才返回Fail
    /// 否则依然执行所有节点
    /// </para>
    /// </summary>
    public class TezBTParallelNode<Data>
        : TezBTCompositeNode<Data>
        where Data : ITezBTData
    {
        public sealed override TezBTNodeType nodeType => TezBTNodeType.Parallel;

        public enum Algorithm
        {
            And,    //与
            Or      //或
        }
        public Algorithm algorithm { get; set; } = Algorithm.And;

        TezArray<TezBTNode<Data>> m_Nodes = new TezArray<TezBTNode<Data>>(0);

        public override void close()
        {
            m_Nodes.close();
            m_Nodes = null;
        }

        public override void addNode(TezBTNode<Data> node)
        {
            m_Nodes.add(node);
        }

        public override TezBTResult execute(Data data)
        {
            switch (this.algorithm)
            {
                case Algorithm.And:
                    return this.exeAnd(data);
                case Algorithm.Or:
                    return this.exeOr(data);
                default:
                    return TezBTResult.Fail;
            }
        }

        protected TezBTResult exeAnd(Data data)
        {
            TezBTResult result = TezBTResult.Success;
            for (int i = 0; i < m_Nodes.count; i++)
            {
                var state = m_Nodes[i].execute(data);
                switch (state)
                {
                    case TezBTResult.Fail:
                        return TezBTResult.Fail;
                    case TezBTResult.Success:
                        break;
                    case TezBTResult.Running:
                        result = TezBTResult.Running;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        protected TezBTResult exeOr(Data data)
        {
            TezBTResult result = TezBTResult.Fail;
            for (int i = 0; i < m_Nodes.count; i++)
            {
                var state = m_Nodes[i].execute(data);
                switch (state)
                {
                    case TezBTResult.Fail:
                        break;
                    case TezBTResult.Success:
                        if(result != TezBTResult.Running)
                        {
                            result = TezBTResult.Success;
                        }
                        break;
                    case TezBTResult.Running:
                        result = TezBTResult.Running;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}