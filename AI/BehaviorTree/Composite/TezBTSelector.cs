using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    public class TezBTSelector : TezBTCompositeNode
    {
        int m_Index;
        List<TezBTNode> m_List = new List<TezBTNode>();

        public override void addNode(TezBTNode node)
        {
            m_List.Add(node);
        }

        public override void init(ITezBTContext context)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init(context);
            }
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

        public override Result execute(ITezBTContext context)
        {
            int count = m_List.Count;
            while(m_Index < count)
            {
                switch (m_List[m_Index].execute(context))
                {
                    case Result.Success:
                        this.exit();
                        return Result.Success;
                    case Result.Fail:
                        m_Index += 1;
                        break;
                    case Result.Running:
                        return Result.Running;
                }
            }

            this.exit();
            return Result.Fail;
        }

        protected override void enter()
        {

        }

        protected override void exit()
        {
            m_Index = 0;
        }
    }
}
