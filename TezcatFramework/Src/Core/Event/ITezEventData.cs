namespace tezcat.Framework.Core
{
    public interface ITezEventData : ITezCloseable
    {
        string name { get; }
    }


    public interface ITezUIEventData : ITezCloseable
    {
        string name { get; }
    }
}