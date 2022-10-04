using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public abstract class TezBaseFSMBlackboard : ITezCloseable
    {
        public abstract void close();
        public abstract void init();
    }
}