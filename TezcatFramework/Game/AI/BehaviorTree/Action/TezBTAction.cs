using System;

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
        static void defaultFunc(TezBTLazyAction condition) { }

        Action<TezBTLazyAction> mOnInit = defaultFunc;
        Action<TezBTLazyAction> mOnExecute;

        public void setInitFunc(Action<TezBTLazyAction> action)
        {
            mOnInit = action;
        }

        public void setExecuteFunc(Action<TezBTLazyAction> action)
        {
            mOnExecute = action;
        }

        public override void init()
        {
            mOnInit(this);
        }

        protected override void onExecute()
        {
            mOnExecute(this);
        }

        protected override void onClose()
        {
            mOnInit = null;
            mOnExecute = null;
            base.onClose();
        }
    }
}
