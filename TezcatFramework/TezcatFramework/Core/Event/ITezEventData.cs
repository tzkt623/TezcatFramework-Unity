namespace tezcat.Framework.Core
{
    public interface ITezEventData : ITezCloseable
    {
        string name { get; }
    }
}