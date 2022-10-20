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
        public override Category category => Category.Action;

        public Result backupResult { get; private set; } = Result.Running;

        protected override void reportToParent(Result result)
        {
            this.parent.onReport(this, result);
        }

        public override void reset()
        {

        }
    }
}
