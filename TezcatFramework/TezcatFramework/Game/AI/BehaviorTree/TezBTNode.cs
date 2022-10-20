﻿using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTNode : ITezCloseable
    {
        public enum Result
        {
            Success = 0,
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

        TezBehaviorTree mTree = null;
        public TezBehaviorTree tree
        {
            get { return mTree; }
            set { mTree = value; }
        }

        public TezBTNode parent { get; set; }

        public int deep { get; set; } = 0;
        public int index { get; set; } = 0;
        protected Result mResult = Result.Running;

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
        public virtual void execute() { }

        [System.Obsolete("这套系统已经弃用,请使用[imdExecute]函数代替")]
        protected virtual void reportToParent(Result result)
        {
            this.parent.onReport(this, result);
        }

        [System.Obsolete("这套系统已经弃用,请使用[imdExecute]函数代替")]
        public virtual void onReport(TezBTNode node, Result result)
        {

        }
    }

}