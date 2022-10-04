using System;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 行为节点
    /// 行为节点是真正运行行为的地方
    /// 整个树最终的运行行为就是由此节点来完成
    /// </summary>
    public abstract class TezBTAction
        : TezBTNode
    {
        bool m_EnterRunning = false;

        public override Category category => Category.Action;

        /// <summary>
        /// 任务index
        /// </summary>
        public int actionIndex { get; set; }

        public Result backupResult { get; private set; } = Result.Running;

        protected override void report(Result result)
        {
            this.parent.onReport(this, result);
        }

        public sealed override void execute()
        {
            var result = this.onExecute();
            ///检测到running状态才会被添加
            ///所以监视并行列表的条件节点不会被添加
            switch (result)
            {
                case Result.Running:
//                     if (!m_EnterRunning)
//                     {
//                         m_EnterRunning = true;
//                         this.tree.addRunningNode(this);
//                     }
                    break;
                default:
                    this.reset();
                    this.report(result);
                    break;
            }
        }

        protected abstract Result onExecute();

        public override void removeSelfFromTree()
        {
            if (m_EnterRunning)
            {
                this.tree.removeRunningNode(this);
                m_EnterRunning = false;
            }
        }

        public override void reset()
        {
            m_EnterRunning = false;
        }
    }
}
