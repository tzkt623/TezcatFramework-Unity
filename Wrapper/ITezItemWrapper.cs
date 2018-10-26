using tezcat.Framework.DataBase;

namespace tezcat.Framework.Wrapper
{
    public interface ITezItemWrapper : ITezWrapper
    {
        TezDataBaseItem myItem { get; }
    }
}