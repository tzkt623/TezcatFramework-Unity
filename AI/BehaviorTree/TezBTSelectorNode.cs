using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 选择行为
    /// <para>
    /// 依次执行所有子节点
    /// 直到有一个节点返回Success或者Running
    /// 则停止执行后续的节点
    /// 并且返回当前状态
    /// </para>
    /// <para>
    /// 如果状态为Running
    /// 则记录当前Running节点继续执行
    /// </para>
    /// 否则返回Fail
    /// </summary>
    public class TezBTSelectorNode<Data>
        : TezBTCompositeNode<Data>
        where Data : ITezBTData
    {
        int m_CurrentRunning = -1;
        TezArray<TezBTNode<Data>> m_Nodes = new TezArray<TezBTNode<Data>>(0);

        public override void addNode(TezBTNode<Data> node)
        {
            m_Nodes.add(node);
        }

        public override void close()
        {
            m_Nodes.close();

            m_Nodes = null;
        }

        public override TezBTResult execute(Data data)
        {
            Debug.Log(string.Format("BT : {0}[{1}]", this.name, this.GetType().Name));

            int index = m_CurrentRunning == -1 ? 0 : m_CurrentRunning;
            while (index < m_Nodes.count)
            {
                var state = m_Nodes[index].execute(data);
                switch (state)
                {
                    case TezBTResult.Success:
                        m_CurrentRunning = -1;
                        return TezBTResult.Success;
                    case TezBTResult.Running:
                        m_CurrentRunning = index;
                        return TezBTResult.Running;
                    default:
                        break;
                }
                index += 1;
            }

            m_CurrentRunning = -1;
            return TezBTResult.Fail;
        }
    }
}