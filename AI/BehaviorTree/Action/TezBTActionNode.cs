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
