using tezcat.DataBase;

namespace tezcat.Wrapper
{
    public interface ITezItemWrapper : ITezWrapper
    {
        TezDataBaseItem myItem { get; }
    }
}