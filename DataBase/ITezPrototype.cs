namespace tezcat.Framework.DataBase
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