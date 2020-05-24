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
        public int actionIndex { get; set; }

        public Result backupResult { get; private set; } = Result.Running;

        public sealed override void execute()
        {
            this.backupResult = onExecute();

            switch (backupResult)
            {
                case Result.Running:
                    if (!m_IsActive)
                    {
                        m_IsActive = true;
                        this.tree.addActionNode(this);
                    }
                    break;
                default:
                    if (m_IsActive)
                    {
                        this.reset();
                        this.report(backupResult);
                    }
                    break;
            }
        }

        protected abstract Result onExecute();


        public override void reset()
        {
            m_IsActive = false;
        }
    }
}
