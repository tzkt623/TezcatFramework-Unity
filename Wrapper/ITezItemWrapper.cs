using tezcat.DataBase;

namespace tezcat.Wrapper
{
    public interface ITezItemWrapper : ITezWrapper
    {
        TezItem myItem { get; }
    }
}