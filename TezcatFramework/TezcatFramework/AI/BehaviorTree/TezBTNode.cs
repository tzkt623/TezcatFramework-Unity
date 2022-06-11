using System;
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
                //                 if (category == Category.Action)
                //                 {
                //                     m_Tree.registerAction((TezBTAction)this);
                //                 }
            }
        }

        public TezBTNode parent { get; set; }

        public int deep { get; set; } = 0;
        public int index { get; set; } = 0;
        protected Result m_Result = Result.Running;

        public virtual void close()
        {
            m_Tree = null;
            this.parent = null;
        }

        [Obsolete("暂时废弃,请用[newExecute]")]
        public abstract void execute();

        public abstract Result newExecute();

        public abstract void init();
        public abstract void reset();

        protected virtual void report(Result result)
        {
            this.parent.onReport(this, result);
        }

        public virtual void onReport(TezBTNode node, Result result)
        {

        }

        public virtual void loadConfig(TezReader reader) { }

        public virtual void removeSelfFromTree() { }
    }

}