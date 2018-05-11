using tezcat.DataBase;

namespace tezcat.Core
{
    public interface ITezCore
    {
        TezDatabase.GroupType groupType { get; }
        TezDatabase.CategoryType categoryType { get; }
    }

    public interface ITezData : ITezCore
    {

    }
}