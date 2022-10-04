namespace tezcat.Framework.Database
{
    public interface ITezCategoryObject
    {
        TezCategory category { get; }
        bool compare(ITezCategoryObject other);
    }
}
