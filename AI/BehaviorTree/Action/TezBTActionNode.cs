namespace tezcat.Framework.AI
{
    /// <summary>
    /// ��Ϊ�ڵ�
    /// ��Ϊ�ڵ�������������Ϊ�ĵط�
    /// ���������յ�������Ϊ�����ɴ˽ڵ������
    /// </summary>
    public abstract class TezBTActionNode
        : TezBTNode
    {
        bool m_IsActive = false;

        public override Category category => Category.Action;

        /// <summary>
        /// ����index
        /// </summary>
        public int taskIndex { get; set; }

        public sealed override Result execute()
        {
            var result = onExecute();

            switch (result)
            {
                case Result.Running:
                    if (!m_IsActive)
                    {
                        m_IsActive = true;
                        this.tree.addTask(this);
                    }
                    break;
                default:
                    if (m_IsActive)
                    {
                        this.report(result);
                    }
                    break;
            }

            return result;
        }

        protected abstract Result onExecute();


        public override void reset()
        {
            m_IsActive = false;
        }
    }
}
