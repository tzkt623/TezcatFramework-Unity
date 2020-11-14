using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 选择节点
    /// 运行直到某一个子节点返回Success 则返回Success
    /// 否则返回Fail
    /// </summary>
    public class TezBTSelector : TezBTCompositeNode
    {
        int m_Index;
        List<TezBTNode> m_List = new List<TezBTNode>();

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

        public override void close()
        {
            base.close();

            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].close();
            }
            m_List.Clear();
            m_List = null;
        }

        /// <summary>
        /// 子节点向自己报告运行状态
        /// </summary>
        protected override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    ///如果有节点运行成功
                    ///像父级报告运行成功
                    ///
                    this.reset();
                    this.report(Result.Success);
                    break;
                case Result.Fail:
                    ///如果有节点运行失败
                    ///测试下一个节点
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        this.report(Result.Fail);
                    }
                    else
                    {
                        m_List[m_Index].execute();
                    }
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            m_List[m_Index].execute();
        }

        public override void reset()
        {
            m_Index = 0;
        }
    }
}
