namespace tezcat.Framework.Core
{
    public abstract class TezVersions : ITezService
    {
        public abstract string name { get; }
        public abstract int major { get; }
        public abstract int minor { get; }
        public abstract int build { get; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, build);
        }

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }
}
