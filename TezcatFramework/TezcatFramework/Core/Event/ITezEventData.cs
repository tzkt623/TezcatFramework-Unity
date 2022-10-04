using tezcat.Framework.Core;

namespace tezcat.Framework.Event
{
    public interface ITezEventData : ITezCloseable
    {
        string name { get; }
    }
}