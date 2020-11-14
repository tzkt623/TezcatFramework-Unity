using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// ˳��ڵ�
    /// ˳���������нڵ� ���нڵ�Success �򷵻�Success
    /// ���򷵻�Fail
    /// </summary>
    public class TezBTSequence : TezBTCompositeNode
    {
        List<TezBTNode> m_List = new List<TezBTNode>();
        bool m_Running = true;
        int m_Index = 0;

        public override void init()
        {
            m_List.TrimExcess();
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init();
            }
        }

        /// <summary>
        /// �ӽڵ����Լ���������״̬
        /// </summary>
        protected override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
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
                case Result.Fail:
                    this.reset();
                    this.report(Result.Fail);
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

        public override void addNode(TezBTNode node)
        {
            base.addNode(node);
            m_List.Add(node);
        }
    }
}
