using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTNode : ITezCloseable
    {
        public enum Result
        {
            Success,
            Fail,
            Running
        }

        public enum Category
        {
            Composite,
            Action,
            Condition,
            Decorator
        }

        public abstract Category category { get; }

        TezBehaviorTree m_Tree = null;
        public TezBehaviorTree tree
        {
            get { return m_Tree; }
            set
            {
                m_Tree = value;
                if (category == Category.Action)
                {
                    m_Tree.registerAction((TezBTActionNode)this);
                }
            }
        }

        public TezBTNode parent { get; set; }

        public virtual void close(bool self_close = true)
        {
            this.tree = null;
            this.parent = null;
        }

        /// <summary>
        /// 非Action节点使用
        /// </summary>
        public abstract void execute();

        public abstract void init();
        public abstract void reset();

        protected void report(Result result)
        {
            if (this.parent != null)
            {
                this.parent.onReport(this, result);
            }
            else
            {
                this.tree.onReport(this, result);
            }
        }

        protected virtual void onReport(TezBTNode node, Result result)
        {

        }

        public abstract void loadConfig(TezReader reader);
    }

}
