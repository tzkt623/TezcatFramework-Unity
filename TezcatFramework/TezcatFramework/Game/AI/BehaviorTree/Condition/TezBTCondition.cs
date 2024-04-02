namespace tezcat.Framework.Game
{
    /// <summary>
    /// 条件节点
    /// 
    /// <para>
    /// 用于和组合节点搭配来提供逻辑判断的节点
    /// 条件节点只能返回success和fail
    /// 不能返回running状态
    /// </para>
    /// 
    /// <para>
    /// !!!注意:条件节点不能有子节点!!!
    /// </para>
    /// </summary>
    public abstract class TezBTCondition : TezBTNode
    {
        public override Category category => Category.Condition;

        protected override void reportToParent(Result result)
        {
            if(result == Result.Running)
            {
                throw new System.Exception();
            }

            base.reportToParent(result);
        }
    }
}
