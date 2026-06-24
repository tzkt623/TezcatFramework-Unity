using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public abstract class TezBaseFSMBlackboard : ITezCloseable
    {
        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
        public abstract void init();
    }
}