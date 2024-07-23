using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public abstract class TezBaseFSMBlackboard : ITezCloseable
    {
        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected abstract void onClose();
        public abstract void init();
    }
}