namespace tezcat.Framework.AI
{

    /// <summary>
    /// 队列行为
    /// 
    /// 子行为按顺序执行
    /// 全部执行成功
    /// 本行为返回Success
    /// 
    /// 其中一个执行失败
    /// 中断此行为
    /// 并且返回Failure
    ///
    /// </summary>
    public class TezAISequence : TezAIComposite
    {
        int m_Index = 0;
        TezAIBehaviour m_Current = null;

        public override void enter()
        {

        }

        public override void exit()
        {

        }

        public override bool evaluate(TezAICollection collection)
        {
            if (m_Children[m_Index].evaluate(collection))
            {
                if (m_Current == m_Children[m_Index])
                {
                    return true;
                }

                m_Current?.exit();
                m_Current = m_Children[m_Index];
                m_Current.enter();
                return true;
            }

            if(m_Current)
            {
                m_Current.exit();
                m_Current = null;
                m_Index = 0;
            }

            return false;
        }

        public override TezAIState executing(TezAICollection collection)
        {
            var state = m_Current.executing(collection);
            if (state != TezAIState.Running)
            {
                m_Current.exit();
                m_Current = null;

                m_Index += 1;
                if (state == TezAIState.Success && m_Index < m_Children.Count)
                {
                    state = TezAIState.Running;
                }
                else
                {
                    m_Index = 0;
                }
            }

            return state;
        }
    }
}