namespace tezcat.Framework.Test
{
    public abstract class TezBaseTest
    {
        public string name { get; }

        public TezBaseTest(string name)
        {
            this.name = name;
        }

        public abstract void run();
        public abstract void init();
        public abstract void close();
    }
}
