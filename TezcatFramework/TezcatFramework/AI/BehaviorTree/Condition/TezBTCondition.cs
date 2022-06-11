namespace tezcat.Framework.AI
{
    /// <summary>
    /// 条件节点
    /// 用于和组合节点搭配来提供逻辑判断的节点
    /// 条件节点只能返回success和fail
    /// 不能返回running状态
    /// </summary>
    public abstract class TezBTCondition : TezBTNode
    {
        public override Category category => Category.Condition;
        bool m_EnterRunning = false;

        public sealed override void execute()
        {
//             if (!m_EnterRunning)
//             {
//                 m_EnterRunning = true;
//                 this.tree.addRunningNode(this);
//             }

            this.onExecute();
        }

        protected override void report(Result result)
        {
            if(result == Result.Running)
            {
                throw new System.Exception();
            }

            base.report(result);
        }

        protected abstract void onExecute();

        public override void removeSelfFromTree()
        {
            this.tree.removeRunningNode(this);
            m_EnterRunning = false;
        }
    }
}
