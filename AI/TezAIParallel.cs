using System.Collections.Generic;


namespace tezcat.AI
{

    /// <summary>
    /// 
    /// 并行运行所有子行为
    /// 如果遇到执行不成功
    /// 则中断执行
    /// 
    /// 全部执行成功
    /// 返回成功
    /// 
    /// </summary>
    public class TezAIParallel : TezAIComposite
    {
        List<bool> m_Temp = new List<bool>();

        public override void enter()
        {
            m_Temp.Clear();
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Temp.Add(true);
            }
        }

        public override void exit()
        {

        }

        public override bool evaluate(TezAICollection collection)
        {
            return true;
        }

        public override TezAIState executing(TezAICollection collection)
        {
            int counter = 0;
            TezAIState state = TezAIState.Failure;
            for (int i = 0; i < m_Temp.Count; i++)
            {
                if(m_Temp[i])
                {
                    state = m_Children[i].executing(collection);
                    switch (state)
                    {
                        case TezAIState.Failure:
                            return TezAIState.Failure;
                        case TezAIState.Success:
                            m_Temp[i] = false;
                            break;
                    }
                }
                else
                {
                    counter += 1;
                }
            }

            if(counter == m_Children.Count)
            {
                return TezAIState.Success;
            }

            return TezAIState.Running;
        }
    }
}