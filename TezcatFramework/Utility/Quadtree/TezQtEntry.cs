using tezcat.Framework.Core;
using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public abstract class TezQtEntry : ITezCloseable
    {
        public abstract TezShape shape { get; }
        public bool dead { get; set; } = false;
        public object usrData { get; set; }

        public void close()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            this.usrData = null;
        }

        public override string ToString()
        {
            return usrData.ToString();
        }
    }
}