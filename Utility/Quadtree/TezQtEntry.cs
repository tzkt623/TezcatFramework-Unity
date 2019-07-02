using tezcat.Framework.Shape;

namespace tezcat.Framework.Utility
{
    public abstract class TezQtEntry
    {
        public abstract TezShape shape { get; }
        public bool dead { get; set; } = false;
        public object usrData { get; set; }

        public virtual void close()
        {
            this.usrData = null;
        }
    }
}