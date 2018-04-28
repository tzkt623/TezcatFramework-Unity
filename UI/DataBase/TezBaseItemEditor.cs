using tezcat.DataBase;

namespace tezcat.UI
{
    public abstract class TezBaseItemEditor : TezWindow
    {
        public abstract TezDatabase.CategoryType[] categoryTypes { get; }

        public abstract void bind(TezDatabase.CategoryType category_type);
    }
}