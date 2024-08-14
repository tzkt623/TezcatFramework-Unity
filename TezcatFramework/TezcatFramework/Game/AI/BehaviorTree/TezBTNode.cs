using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBTNode : ITezCloseable
    {
        void execute();
    }

    public interface ITezBTParentNode
    {
        void addChild(TezBTNode node);
        void childReport(TezBTNode.Result result);
    }

    public abstract class TezBTNode : ITezBTNode
    {
        public enum Result : byte
        {
            Success,
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

        protected ITezBTParentNode mParent = null;
        public ITezBTParentNode parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        public int deep { get; set; } = 0;
        public int index { get; set; } = 0;

        private Result mState = Result.Running;

        public abstract void init();
        public virtual void reset() { }
        public virtual void loadConfig(TezReader reader) { }

        protected void setFail()
        {
            mState = Result.Fail;
        }

        protected void setSuccess()
        {
            mState = Result.Success;
        }

        protected void setRunning()
        {
            mState = Result.Running;
        }

        private void reportState()
        {
            mParent?.childReport(mState);
        }

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            mTree = null;
            mParent = null;
        }

        public void execute()
        {
            mState = Result.Running;
            this.onExecute();
            this.reportState();
        }

        protected abstract void onExecute();
    }
}