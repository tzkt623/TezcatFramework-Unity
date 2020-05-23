﻿namespace tezcat.Framework.AI
{
    /// <summary>
    /// 条件节点
    /// 用于和组合节点搭配来提供逻辑判断的节点
    /// </summary>
    public abstract class TezBTConditionNode : TezBTNode
	{
        public override Category category => Category.Condition;

        public sealed override void execute()
        {
            this.onExecute();
        }

        protected abstract void onExecute();
    }
}