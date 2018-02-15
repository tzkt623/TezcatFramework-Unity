namespace tezcat
{
    public interface IContainer
    {
        void add(TezItem item, int count);
        void remove(TezItem item, int count);
    }
}
