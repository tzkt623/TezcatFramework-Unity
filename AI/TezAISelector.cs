namespace tezcat.Framework.AI
{

    /// <summary>
    /// 选择行为
    /// 
    /// 下属中其中一个行为返回Success时返回Success
    /// 否则返回Failure
    /// 
    /// </summary>
    public class TezAISelector : TezAIComposite
    {
        TezAIBehaviour m_Current = null;
        int m_Index = -1;

        public override bool evaluate(TezAICollection collection)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                if (m_Children[i].evaluate(collection))
                {
                    if (m_Index != i)
                    {
                        m_Current?.exit();
                        m_Index = i;
                        m_Current = m_Children[i];
                        m_Current.enter();
                    }

                    return true;
                }
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
                m_Index = -1;
            }

            return state;
        }

        public override void enter()
        {

        }

        public override void exit()
        {

        }
    }
}