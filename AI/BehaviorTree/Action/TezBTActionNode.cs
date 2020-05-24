namespace tezcat.Framework.AI
{
    /// <summary>
    /// 行为节点
    /// 行为节点是真正运行行为的地方
    /// 整个树最终的运行行为就是由此节点来完成
    /// </summary>
    public abstract class TezBTActionNode
        : TezBTNode
    {
        bool m_IsActive = false;

        public override Category category => Category.Action;

        /// <summary>
        /// 任务index
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
