using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTNode : ITezCloseable
    {
        public enum Result : byte
        {
            Success = 0,
            Fail,
            Running
        }

        public enum Category : byte
        {
            Composite = 0,
            Action,
            Condition,
            Decorator
        }

        public abstract Category category { get; }

        TezBehaviorTree mTree = null;
        public TezBehaviorTree tree
        {
            get { return mTree; }
            set { mTree = value; }
        }

        public TezBTNode parent { get; set; } = null;
        public int deep { get; set; } = 0;
        public int index { get; set; } = 0;

        public abstract void init();

        public abstract void reset();

        public virtual void close()
        {
            mTree = null;
            this.parent = null;
        }

        public abstract Result imdExecute();

        public virtual void loadConfig(TezReader reader) { }


        [System.Obsolete("这套系统已经弃用,请使用[imdExecute]函数代替")]
        public virtual void execute()
        {
            throw new System.Exception(string.Format("TezBTComposite : Obsolete Method {0}", nameof(execute)));
        }

        [System.Obsolete("这套系统已经弃用,请使用[imdExecute]函数代替")]
        protected virtual void reportToParent(Result result)
        {
            throw new System.Exception(string.Format("TezBTComposite : Obsolete Method {0}", nameof(reportToParent)));
            this.parent.onReport(this, result);
        }

        [System.Obsolete("这套系统已经弃用,请使用[imdExecute]函数代替")]
        public virtual void onReport(TezBTNode node, Result result)
        {
            throw new System.Exception(string.Format("TezBTComposite : Obsolete Method {0}", nameof(onReport)));
        }
    }

}