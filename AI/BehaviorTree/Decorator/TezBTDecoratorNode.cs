namespace tezcat.Framework.AI
{
    /// <summary>
    /// 修饰节点
    /// 用来做辅助功能的节点
    /// </summary>
    public abstract class TezBTDecoratorNode : TezBTNode
    {
        public override Category category => Category.Decorator;

        public sealed override void execute()
        {
            this.onExecute();
        }

        protected abstract void onExecute();
    }
}
