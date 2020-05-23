using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 并行节点
    /// 并行运行所有节点
    /// 直到所有节点运行完毕返回Success
    /// 附带运行失败和成功节点的数量
    /// </summary>
    public class TezBTParallel : TezBTCompositeNode
    {
        List<TezBTNode> m_List = new List<TezBTNode>();
        public int failCount { get; private set; } = 0;
        public int successCount { get; private set; } = 0;

        public override void addNode(TezBTNode node)
        {
            base.addNode(node);
            m_List.Add(node);
        }

        public override void init()
        {
            m_List.TrimExcess();
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init();
            }
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);

            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].close();
            }
            m_List.Clear();
            m_List = null;
        }

        protected override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    successCount++;
                    break;
                case Result.Fail:
                    failCount++;
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }

            if (successCount + failCount == m_List.Count)
            {
                this.reset();
                this.report(Result.Success);
            }
        }

        protected override void onExecute()
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].execute();
            }
        }

        public override void reset()
        {
            failCount = 0;
            successCount = 0;
        }
    }
}
