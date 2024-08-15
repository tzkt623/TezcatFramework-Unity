using System;

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
    }

    public class TezBTLazyCondition : TezBTCondition
    {
        static void defaultFunc(TezBTLazyCondition condition) { }

        Action<TezBTLazyCondition> mOnInit = defaultFunc;
        Action<TezBTLazyCondition> mOnExecute;

        public override void init()
        {
            mOnInit.Invoke(this);
        }

        public void setInitFunc(Action<TezBTLazyCondition> action)
        {
            mOnInit = action;
        }

        public void setExecuteFunc(Action<TezBTLazyCondition> action)
        {
            mOnExecute = action;
        }

        protected override void onExecute()
        {
            mOnExecute(this);
        }

        protected override void onClose()
        {
            mOnExecute = null;
            mOnInit = null;
            base.onClose();
        }
    }
}
