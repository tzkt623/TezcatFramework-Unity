using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    public class TezBTSequence : TezBTCompositeNode
    {
        List<TezBTNode> m_List = new List<TezBTNode>();
        bool m_Running = true;
        int m_Index = 0;

        public override void init(ITezBTContext context)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init(context);
            }
        }

        protected override void enter()
        {

        }

        protected override void exit()
        {
            m_Index = 0;
        }

        public override Result execute(ITezBTContext context)
        {
            int count = m_List.Count;
            while (m_Index < count)
            {
                switch (m_List[m_Index].execute(context))
                {
                    case Result.Running:
                        return Result.Running;
                    case Result.Success:
                        m_Index += 1;
                        break;
                    case Result.Fail:
                        this.exit();
                        return Result.Fail;
                }
            }

            this.exit();
            return Result.Success;
        }
        public override void close(bool self_close = true)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].close();
            }
            m_List.Clear();
            m_List = null;
        }

        public override void addNode(TezBTNode node)
        {
            m_List.Add(node);
        }

    }
}
