using System;
using System.Runtime.CompilerServices;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 行为节点
    /// 行为节点是真正运行行为的地方
    /// 整个树最终的运行行为就是由此节点来完成
    /// </summary>
    public abstract class TezBTAction : TezBTNode
    {
        public override Category category => Category.Action;
        public Result backupResult { get; private set; } = Result.Running;
    }


    public class TezBTLazyAction : TezBTAction
    {
        Action<TezBehaviorTree, TezBTLazyAction> mOnInit;
        Action<TezBehaviorTree, TezBTLazyAction> mOnExecute;

        public void setLazyFunction(Action<TezBehaviorTree, TezBTLazyAction> onInit, Action<TezBehaviorTree, TezBTLazyAction> onExecute)
        {
            mOnInit = onInit;
            mOnExecute = onExecute;
        }

        public override void init()
        {
            mOnInit(this.tree, this);
        }

        protected override void onExecute()
        {
            mOnExecute(this.tree, this);
        }

        protected override void onClose()
        {
            mOnInit = null;
            mOnExecute = null;
            base.onClose();
        }
    }
}
