using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    public class TezBTParallel : TezBTCompositeNode
    {
        List<TezBTNode> m_List = new List<TezBTNode>();
        int m_Index = 0;

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
            int fail_count = 0;
            int success_count = 0;

            for (int i = 0; i < m_List.Count; i++)
            {
                switch (m_List[i].execute(context))
                {
                    case Result.Success:
                        fail_count += 1;
                        break;
                    case Result.Fail:
                        success_count += 1;
                        break;
                }
            }

            if (fail_count > 0)
            {
                this.exit();
                return Result.Fail;
            }

            if (success_count == m_List.Count)
            {
                this.exit();
                return Result.Success;
            }

            return Result.Running;
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
