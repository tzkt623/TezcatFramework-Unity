using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 强制顺序运行节点
    /// 不论子节点成功失败都会运行
    /// 直到所有节点运行完毕返回Success
    /// </summary>
    public class TezBTForce : TezBTCompositeNode
    {
        List<TezBTNode> m_List = new List<TezBTNode>();
        int m_Index = 0;

        public override void init()
        {
            m_List.TrimExcess();
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init();
            }
        }

        public override void reset()
        {
            m_Index = 0;
        }

        protected override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    break;
                default:
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        this.report(Result.Success);
                    }
                    else
                    {
                        m_List[m_Index].execute();
                    }
                    break;
            }
        }

        protected override void onExecute()
        {
            m_List[m_Index].execute();
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

        public override void addNode(TezBTNode node)
        {
            base.addNode(node);
            m_List.Add(node);
        }

    }
}
