using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    public abstract class TezBaseTest : ITezCloseable
    {
        public string name { get; }

        public TezBaseTest(string name)
        {
            this.name = name;
        }

        public abstract void run();
        public abstract void init();

        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }
}