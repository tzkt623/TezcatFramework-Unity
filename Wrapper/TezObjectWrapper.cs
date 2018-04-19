using tezcat.DataBase;

namespace tezcat.Wrapper
{
    public interface ITezWrapper
    {
        string name { get; }
        string description { get; }
        void clear();
    }

    public interface ITezObjectWrapper : ITezWrapper
    {

    }

    public interface ITezItemWrapper : ITezWrapper
    {
        ITezItem item { get; }
        int count { get; }

        void showTip();
        void hideTip();
        void onDrop();
    }

    public abstract class TezObjectWrapper : ITezWrapper
    {
        public abstract string name { get; }
        public abstract string description { get; }
        public abstract void clear();
    }
}