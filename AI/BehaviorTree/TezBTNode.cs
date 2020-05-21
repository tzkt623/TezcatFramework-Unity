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

        public abstract void close(bool self_close = true);

        protected abstract void enter();
        protected abstract void exit();

        public abstract Result execute(ITezBTContext context);

        public virtual void init(ITezBTContext context) { }

        public virtual void loadConfig(TezReader reader)
        {

        }
    }

}
