namespace tezcat.Framework.Database
{
    public interface ITezPrototype : ITezSerializable
    {
        string prototypeName { get; }
    }

    public interface ITezPrototype<out T> : ITezPrototype
    {
        T clone();
    }
}